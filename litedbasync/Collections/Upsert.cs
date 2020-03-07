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
        /// Insert or Update a document in this collection.
        /// </summary>
        public Task<bool> UpsertAsync(T entity)
        {
            var tcs = new TaskCompletionSource<bool>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(GetUnderlyingCollection().Upsert(entity));
            });
            return tcs.Task;
        }

        /// <summary>
        /// Insert or Update all documents
        /// </summary>
        public Task<int> UpsertAsync(IEnumerable<T> entities)
        {
            var tcs = new TaskCompletionSource<int>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(GetUnderlyingCollection().Upsert(entities));
            });
            return tcs.Task;
        }

        /// <summary>
        /// Insert or Update a document in this collection.
        /// </summary>
        public Task<bool> UpsertAsync(BsonValue id, T entity)
        {
            var tcs = new TaskCompletionSource<bool>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(GetUnderlyingCollection().Upsert(id, entity));
            });
            return tcs.Task;
        }
    }
}