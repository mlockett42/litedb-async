using System;
using LiteDB;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace litedbasync.Tasks
{
     internal class ToListAsyncTask<T> : ILiteDbAsyncTask
    {
        private readonly LiteCollectionAsync<T> _collectionAsync;
        private TaskCompletionSource<List<T>> _tcs;
        public ToListAsyncTask(LiteCollectionAsync<T> collectionAsync, TaskCompletionSource<List<T>> tcs)
        {
            _collectionAsync = collectionAsync;
            _tcs = tcs;
        }

        public void Execute()
        {
            var result = _collectionAsync.GetUnderlyingCollection().Query().ToList();
            _tcs.SetResult(result);
        }
    }
}
