using System;
using LiteDB;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace litedbasync
{
    public partial class LiteCollectionAsync<T>
    {
        /// <summary>
        /// Insert a new entity to this collection. Document Id must be a new value in collection - Returns document Id
        /// </summary>
        public Task<BsonValue> InsertAsync(T entity)
        {
            var tcs = new TaskCompletionSource<BsonValue>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(GetUnderlyingCollection().Insert(entity));
            });
            return tcs.Task;
        }

        /// <summary>
        /// Insert a new document to this collection using passed id value.
        /// </summary>
        public Task InsertAsync(BsonValue id, T entity)
        {
            var tcs = new TaskCompletionSource<bool>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                GetUnderlyingCollection().Insert(id, entity);
                tcs.SetResult(true);
            });
            return tcs.Task;
        }

        /// <summary>
        /// Insert an array of new documents to this collection. Document Id must be a new value in collection. Can be set buffer size to commit at each N documents
        /// </summary>
        public Task<int> InsertAsync(IEnumerable<T> entities)
        {
            var tcs = new TaskCompletionSource<int>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(GetUnderlyingCollection().Insert(entities));
            });
            return tcs.Task;
        }
    }
}
