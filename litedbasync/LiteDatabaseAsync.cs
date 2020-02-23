using System;
using LiteDB;
using System.Threading;
using System.Collections.Concurrent;


namespace litedbasync
{
    public class LiteDatabaseAsync
    {
        private readonly LiteDatabase _liteDB;
        private readonly Thread _backgroundThread;
        ManualResetEventSlim _newTaskArrived = new ManualResetEventSlim(false);
        ManualResetEventSlim _shouldTerminate = new ManualResetEventSlim(false);
        ConcurrentQueue<ILiteDbAsyncTask> _Queue = new ConcurrentQueue<ILiteDbAsyncTask>();
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
                if (_Queue.TryDequeue(out task))
                {
                    task.Execute();
                }
                else
                {
                    // reset when queue is empty
                    _newTaskArrived.Reset();
                    //break;
                }            
            }
        }

        internal void Enqueue(ILiteDbAsyncTask task)
        {
            _Queue.Enqueue(task);
            _newTaskArrived.Set();
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
