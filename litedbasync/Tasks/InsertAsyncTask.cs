using System;
using LiteDB;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace litedbasync.Tasks
{
    internal class InsertAsyncTask<T> : ILiteDbAsyncTask
    {
        private readonly LiteCollectionAsync<T> _collectionAsync;
        private readonly T _entity;
        private TaskCompletionSource<BsonValue> _tcs;
        public InsertAsyncTask(LiteCollectionAsync<T> collectionAsync, TaskCompletionSource<BsonValue> tcs, T entity)
        {
            _collectionAsync = collectionAsync;
            _entity = entity;
            _tcs = tcs;
        }

        public void Execute()
        {
            try
            {
                var result = _collectionAsync.GetUnderlyingCollection().Insert(_entity);
                _tcs.SetResult(result);
            }
            catch (LiteException ex)
            {
                _tcs.SetException(new LiteAsyncException("LiteDb encounter an error. Details in the inner exception.", ex));
            }
        }
    }
}
