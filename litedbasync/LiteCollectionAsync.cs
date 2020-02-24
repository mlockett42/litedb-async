using System;
using LiteDB;
using System.Threading.Tasks;
using System.Collections.Generic;
using litedbasync.Tasks;

namespace litedbasync
{
    public class LiteCollectionAsync<T>
    {
        private readonly ILiteCollection<T> _liteCollection;
        private readonly LiteDatabaseAsync _liteDatabaseAsync;
        internal LiteCollectionAsync(ILiteCollection<T> liteCollection, LiteDatabaseAsync liteDatabaseAsync)
        {
            _liteCollection = liteCollection;
            _liteDatabaseAsync = liteDatabaseAsync;
        }

        public LiteDatabaseAsync Database {
            get
            {
                return _liteDatabaseAsync;
            }
        }

        public ILiteCollection<T> GetUnderlyingCollection()
        {
            return _liteCollection;
        }

        public Task<bool> UpsertAsync(T entity)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            //UpsertAsyncTask<T> task = new UpsertAsyncTask<T>(this, tcs, entity);
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(GetUnderlyingCollection().Upsert(entity));
            });
            return tcs.Task;
        }

        public Task<List<T>> ToListAsync()
        {
            TaskCompletionSource<List<T>> tcs = new TaskCompletionSource<List<T>>();
            //ToListAsyncTask<T> task = new ToListAsyncTask<T>(this, tcs);
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(GetUnderlyingCollection().Query().ToList());
            });
            return tcs.Task;
        }

        public Task<BsonValue> InsertAsync(T entity)
        {
            TaskCompletionSource<BsonValue> tcs = new TaskCompletionSource<BsonValue>();
            //InsertAsyncTask<T> task = new InsertAsyncTask<T>(this, tcs, entity);
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(GetUnderlyingCollection().Insert(entity));
            });
            return tcs.Task;
        }
    }
}