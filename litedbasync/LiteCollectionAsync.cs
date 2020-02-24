using System;
using LiteDB;
using System.Threading.Tasks;
using System.Collections.Generic;

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
            var tcs = new TaskCompletionSource<bool>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(GetUnderlyingCollection().Upsert(entity));
            });
            return tcs.Task;
        }

        public Task<List<T>> ToListAsync()
        {
            var tcs = new TaskCompletionSource<List<T>>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(GetUnderlyingCollection().Query().ToList());
            });
            return tcs.Task;
        }

        public Task<BsonValue> InsertAsync(T entity)
        {
            var tcs = new TaskCompletionSource<BsonValue>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(GetUnderlyingCollection().Insert(entity));
            });
            return tcs.Task;
        }
    }
}