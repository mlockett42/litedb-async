using System;
using LiteDB;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq.Expressions;

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

        public Task<int> CountAsync(Query query)
        {
            var tcs = new TaskCompletionSource<int>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(GetUnderlyingCollection().Count(query));
            });
            return tcs.Task;
        }

        public Task<bool> UpdateAsync(T entity)
        {
            var tcs = new TaskCompletionSource<bool>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(GetUnderlyingCollection().Update(entity));
            });
            return tcs.Task;
        }

        /// <summary>
        /// Find the first document using predicate expression. Returns null if not found
        /// </summary>
        public Task<T> FindOneAsync(Expression<Func<T, bool>> predicate)
        {
            var tcs = new TaskCompletionSource<T>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(GetUnderlyingCollection().FindOne(predicate));
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

        public Task<int> InsertAsync(IEnumerable<T> entities)
        {
            var tcs = new TaskCompletionSource<int>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(GetUnderlyingCollection().Insert(entities));
            });
            return tcs.Task;
        }

        public LiteQueryableAsync<T> Query()
        {
            return new LiteQueryableAsync<T>(GetUnderlyingCollection().Query(), _liteDatabaseAsync);
        }
    }
}