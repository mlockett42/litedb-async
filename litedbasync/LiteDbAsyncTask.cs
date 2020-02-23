using System;
using LiteDB;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace litedbasync
{
    internal interface ILiteDbAsyncTask
    {
        void Execute();
    }
    internal class UpsertAsyncTask<T> : ILiteDbAsyncTask
    {
        private readonly LiteCollectionAsync<T> _collectionAsync;
        private readonly T _entity;
        private TaskCompletionSource<bool> _tcs;
        public UpsertAsyncTask(LiteCollectionAsync<T> collectionAsync, TaskCompletionSource<bool> tcs, T entity)
        {
            _collectionAsync = collectionAsync;
            _entity = entity;
            _tcs = tcs;
        }

        public void Execute()
        {
            var result = _collectionAsync.GetUnderlyingCollection().Upsert(_entity);
            _tcs.SetResult(result);
        }
    }

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
