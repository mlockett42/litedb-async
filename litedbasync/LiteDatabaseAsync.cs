using System;
using System.Threading;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.IO;

namespace LiteDB.Async
{
    public class LiteDatabaseAsync : IDisposable
    {
        private readonly ILiteDatabase _liteDB;
        private readonly Thread _backgroundThread;
        private readonly ManualResetEventSlim _newTaskArrived = new ManualResetEventSlim(false);
        private readonly ManualResetEventSlim _shouldTerminate = new ManualResetEventSlim(false);
        private readonly ConcurrentQueue<LiteAsyncDelegate> _queue = new ConcurrentQueue<LiteAsyncDelegate>();
        private readonly object _queueLock = new object();
        private readonly bool _disposeOfWrappedDatabase = true;

        /// <summary>
        /// Starts LiteDB database using a connection string for file system database
        /// </summary>
        public LiteDatabaseAsync(string connectionString, BsonMapper mapper = null)
        {
            _liteDB = new LiteDatabase(connectionString, mapper);
            _backgroundThread = new Thread(BackgroundLoop);
            _backgroundThread.Start();
        }

        /// <summary>
        /// Starts LiteDB database using a generic Stream implementation (mostly MemoryStrem).
        /// Use another MemoryStrem as LOG file.
        /// </summary>
        /// <param name="stream">DataStream reference </param>
        /// <param name="mapper">BsonMapper mapper reference</param>
        /// <param name="logStream">LogStream reference </param>
        public LiteDatabaseAsync(Stream stream, BsonMapper mapper = null, Stream logStream = null)
        {
            _liteDB = new LiteDatabase(stream, mapper, logStream);
            _backgroundThread = new Thread(BackgroundLoop);
            _backgroundThread.Start();
        }

        /// <summary>
        /// Starts LiteDB database wrapping the passed in LiteDatabase instance
        /// </summary>
        /// <param name="wrappedDatabase">ILiteDatabase reference </param>
        /// <param name="bool">disposeOfWrappedDatabase iff true dispose of the wrappedDatabase when this object is disposed</param>
        public LiteDatabaseAsync(ILiteDatabase wrappedDatabase, bool disposeOfWrappedDatabase = true)
        {
            _liteDB = wrappedDatabase ?? throw new ArgumentNullException($"{nameof(wrappedDatabase)} cannot be null");
            _backgroundThread = new Thread(BackgroundLoop);
            _backgroundThread.Start();
            _disposeOfWrappedDatabase = disposeOfWrappedDatabase;
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
            var waitHandles = new[] { _newTaskArrived.WaitHandle, _shouldTerminate.WaitHandle };

            while (true)
            {
                var triggerEvent = WaitHandle.WaitAny(waitHandles);
                if (triggerEvent == 1)
                {
                    return;
                }

                LiteAsyncDelegate function;

                lock (_queueLock)
                {
                    if (!_queue.TryDequeue(out function))
                    {
                        // reset when queue is empty
                        _newTaskArrived.Reset();
                        continue;
                    }
                }

                function();
            }
        }

        internal void Enqueue<T>(TaskCompletionSource<T> tcs, LiteAsyncDelegate function)
        {
            lock (_queueLock)
            {
                _queue.Enqueue(() =>
                {
                    try
                    {
                        function();
                    }
                    catch (Exception ex)
                    {
                        tcs.SetException(new LiteAsyncException("LiteDb encounter an error. Details in the inner exception.", ex));
                    }
                });
                _newTaskArrived.Set();
            }
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
                _shouldTerminate.Set();
                _backgroundThread.Join();
                if (_disposeOfWrappedDatabase)
                {
                    _liteDB.Dispose();
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
