using System;
using LiteDB;
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
        ManualResetEventSlim _newTaskArrived = new ManualResetEventSlim(false);
        ManualResetEventSlim _shouldTerminate = new ManualResetEventSlim(false);
        ConcurrentQueue<LiteAsyncDelegate> _queue = new ConcurrentQueue<LiteAsyncDelegate>();
        private readonly object _queueLock = new object();
        public LiteDatabaseAsync(string connectionString)
        {
            _liteDB = new LiteDatabase(connectionString);
            _backgroundThread = new Thread(() => BackgroundLoop() );
            _backgroundThread.Start();
        }

        /// <summary>
        /// Starts LiteDB database using a generic Stream implementation (mostly MemoryStrem).
        /// Use another MemoryStrem as LOG file.
        /// </summary>
        public LiteDatabaseAsync(Stream stream, BsonMapper mapper = null)
        {
            _liteDB = new LiteDatabase(stream, mapper);
            _backgroundThread = new Thread(() => BackgroundLoop() );
            _backgroundThread.Start();
        }

        private void BackgroundLoop()
        {
            LiteAsyncDelegate function;
            var waitHandles = new WaitHandle[] { _newTaskArrived.WaitHandle, _shouldTerminate.WaitHandle };
            while (true)
            {
                var triggerEvent = WaitHandle.WaitAny(waitHandles);
                if (triggerEvent == 1)
                {
                    return;
                }
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
                _queue.Enqueue(() => {
                    try
                    {
                        function();
                    }
                    catch (LiteException ex)
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

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {            
                _shouldTerminate.Set();
                _backgroundThread.Join();
                _liteDB.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
