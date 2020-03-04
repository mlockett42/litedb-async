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
        /// Delete a single document on collection based on _id index. Returns true if document was deleted
        /// </summary>
        public Task<bool> DeleteAsync(BsonValue id)
        {
            var tcs = new TaskCompletionSource<bool>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(GetUnderlyingCollection().Delete(id));
            });
            return tcs.Task;
        }

        /// <summary>
        /// Delete all documents based on predicate expression. Returns how many documents was deleted
        /// </summary>
        public Task<int> DeleteManyAsync(BsonExpression predicate)
        {
            var tcs = new TaskCompletionSource<int>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(GetUnderlyingCollection().DeleteMany(predicate));
            });
            return tcs.Task;
        }

        /// <summary>
        /// Delete all documents based on predicate expression. Returns how many documents was deleted
        /// </summary>
        public Task<int> DeleteManyAsync(string predicate, BsonDocument parameters)
        {
            var tcs = new TaskCompletionSource<int>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(GetUnderlyingCollection().DeleteMany(predicate, parameters));
            });
            return tcs.Task;
        }

        /// <summary>
        /// Delete all documents based on predicate expression. Returns how many documents was deleted
        /// </summary>
        public Task<int> DeleteManyAsync(string predicate, params BsonValue[] args)
        {
            var tcs = new TaskCompletionSource<int>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(GetUnderlyingCollection().DeleteMany(predicate, args));
            });
            return tcs.Task;
        }

        /// <summary>
        /// Delete all documents based on predicate expression. Returns how many documents was deleted
        /// </summary>
        public Task<int> DeleteManyAsync(Expression<Func<T, bool>> predicate)
        {
            var tcs = new TaskCompletionSource<int>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(GetUnderlyingCollection().DeleteMany(predicate));
            });
            return tcs.Task;
        }
    }
}