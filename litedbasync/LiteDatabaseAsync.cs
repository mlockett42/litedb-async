using System;
using LiteDB;
using System.Threading;
using System.Collections.Concurrent;
using litedbasync.Tasks;

namespace litedbasync
{
    public class LiteDatabaseAsync
    {
        private readonly LiteDatabase _liteDB;
        private readonly Thread _backgroundThread;
        ManualResetEventSlim _newTaskArrived = new ManualResetEventSlim(false);
        ManualResetEventSlim _shouldTerminate = new ManualResetEventSlim(false);
        ConcurrentQueue<ILiteDbAsyncTask> _queue = new ConcurrentQueue<ILiteDbAsyncTask>();
        private readonly object _queueLock = new object();
        public LiteDatabaseAsync(string connectionString)
        {
            _liteDB = new LiteDatabase(connectionString);
            _backgroundThread = new Thread(() => BackgroundLoop() );
            _backgroundThread.Start();
        }

        private void BackgroundLoop()
        {
            ILiteDbAsyncTask task;
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
                    if (!_queue.TryDequeue(out task))
                    {
                        // reset when queue is empty
                        _newTaskArrived.Reset();
                        continue;
                    }
                }
                task.Execute();          
            }
        }

        internal void Enqueue(ILiteDbAsyncTask task)
        {
            lock (_queueLock)
            {
                _queue.Enqueue(task);
                _newTaskArrived.Set();
            }
        }

        public LiteCollectionAsync<T> GetCollection<T>()
        {
            return this.GetCollection<T>(null);
        }

        public LiteCollectionAsync<T> GetCollection<T>(string name)
        {
            return new LiteCollectionAsync<T>(_liteDB.GetCollection<T>(name), this);
        }
    }
}
