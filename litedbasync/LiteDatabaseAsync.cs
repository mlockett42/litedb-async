using System;
using System.Threading;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.IO;

namespace LiteDB.Async
{
    public class LiteDatabaseAsync : IDisposable
    {
        private readonly LiteDatabase _liteDB;
        private readonly Thread _backgroundThread;
        private readonly SemaphoreSlim _newTaskArrived = new SemaphoreSlim(initialCount: 0, maxCount: int.MaxValue);
        private readonly CancellationTokenSource _shouldTerminate = new CancellationTokenSource();
        private readonly ConcurrentQueue<LiteAsyncDelegate> _queue = new ConcurrentQueue<LiteAsyncDelegate>();

        /// <summary>
        /// Starts LiteDB database using a connection string for file system database
        /// </summary>
        public LiteDatabaseAsync(string connectionString)
        {
            _liteDB = new LiteDatabase(connectionString);
            _backgroundThread = new Thread(BackgroundLoop);
            _backgroundThread.Start();
        }

        /// <summary>
        /// Starts LiteDB database using a generic Stream implementation (mostly MemoryStrem).
        /// Use another MemoryStrem as LOG file.
        /// </summary>
        public LiteDatabaseAsync(Stream stream, BsonMapper mapper = null)
        {
            _liteDB = new LiteDatabase(stream, mapper);
            _backgroundThread = new Thread(BackgroundLoop);
            _backgroundThread.Start();
        }

        public bool UtcDate
        {
            get => _liteDB.UtcDate;
            set => _liteDB.UtcDate = value;
        }

        public int CheckpointSize
        {
            get => _liteDB.CheckpointSize;
            set => _liteDB.CheckpointSize = value;
        }

        public int UserVersion
        {
            get => _liteDB.UserVersion;
            set => _liteDB.UserVersion = value;
        }

        public TimeSpan Timeout
        {
            get => _liteDB.Timeout;
            set => _liteDB.Timeout = value;
        }

        private void BackgroundLoop()
        {
            var terminationToken = _shouldTerminate.Token;

            try
            {
                while (!terminationToken.IsCancellationRequested)
                {
                    _newTaskArrived.Wait(terminationToken);

                    if (!_queue.TryDequeue(out var function)) continue;

                    function();
                }
            }
            catch (OperationCanceledException) when (terminationToken.IsCancellationRequested)
            {
                // it's OK, we're exiting
            }
        }

        internal void Enqueue<T>(TaskCompletionSource<T> tcs, LiteAsyncDelegate function)
        {
            void Function()
            {
                try
                {
                    function();
                }
                catch (Exception ex)
                {
                    tcs.SetException(new LiteAsyncException("LiteDb encounter an error. Details in the inner exception.", ex));
                }
            }

            _queue.Enqueue(Function);
            _newTaskArrived.Release();
        }

        #region Collections
        public LiteCollectionAsync<T> GetCollection<T>()
        {
            return this.GetCollection<T>(null);
        }

        /// <summary>
        /// Get a collection using a entity class as strong typed document. If collection does not exits, create a new one.
        /// </summary>
        /// <param name="name">Collection name (case insensitive)</param>
        public LiteCollectionAsync<T> GetCollection<T>(string name)
        {
            return new LiteCollectionAsync<T>(_liteDB.GetCollection<T>(name), this);
        }

        /// <summary>
        /// Get a collection using a generic BsonDocument. If collection does not exits, create a new one.
        /// </summary>
        /// <param name="name">Collection name (case insensitive)</param>
        /// <param name="autoId">Define autoId data type (when document contains no _id field)</param>
        public LiteCollectionAsync<BsonDocument> GetCollection(string name, BsonAutoId autoId = BsonAutoId.ObjectId)
        {
            return new LiteCollectionAsync<BsonDocument>(_liteDB.GetCollection(name, autoId), this);
        }
        #endregion

        #region FileStorage

        private ILiteStorageAsync<string> _fs = null;

        /// <summary>
        /// Returns a special collection for storage files/stream inside datafile. Use _files and _chunks collection names. FileId is implemented as string. Use "GetStorage" for custom options
        /// </summary>
        public ILiteStorageAsync<string> FileStorage
        {
            get { return _fs ?? (_fs = this.GetStorage<string>()); }
        }

        /// <summary>
        /// Get new instance of Storage using custom FileId type, custom "_files" collection name and custom "_chunks" collection. LiteDB support multiples file storages (using different files/chunks collection names)
        /// </summary>
        public ILiteStorageAsync<TFileId> GetStorage<TFileId>(string filesCollection = "_files", string chunksCollection = "_chunks")
        {
            return new LiteStorageAsync<TFileId>(this, _liteDB, filesCollection, chunksCollection);
        }

        #endregion

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                using (_liteDB)
                using (_shouldTerminate)
                using (_newTaskArrived)
                {
                    _shouldTerminate.Cancel();
                    
                    // give the thread 5 seconds to exit... must not block forever here
                    _backgroundThread.Join(TimeSpan.FromSeconds(5));
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
