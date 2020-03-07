using System;
using LiteDB;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LiteDB.Async
{
    public partial class LiteCollectionAsync<T>
    {
        /// <summary>
        /// Update a document in this collection. Returns false if not found document in collection
        /// </summary>
        public Task<bool> UpdateAsync(T entity)
        {
            var tcs = new TaskCompletionSource<bool>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(GetUnderlyingCollection().Update(entity));
            });
            return tcs.Task;

        }

        /// <summary>
        /// Update a document in this collection. Returns false if not found document in collection
        /// </summary>
        public Task<bool> UpdateAsync(BsonValue id, T entity)
        {
            var tcs = new TaskCompletionSource<bool>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(GetUnderlyingCollection().Update(id, entity));
            });
            return tcs.Task;
        }

        /// <summary>
        /// Update all documents
        /// </summary>
        public Task<int> UpdateAsync(IEnumerable<T> entities)
        {
            var tcs = new TaskCompletionSource<int>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(GetUnderlyingCollection().Update(entities));
            });
            return tcs.Task;
        }

        /// <summary>
        /// Update many documents based on transform expression. This expression must return a new document that will be replaced over current document (according with predicate).
        /// Eg: col.UpdateMany("{ Name: UPPER($.Name), Age }", "_id > 0")
        /// </summary>
        public Task<int> UpdateManyAsync(BsonExpression transform, BsonExpression predicate)
        {
            var tcs = new TaskCompletionSource<int>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(GetUnderlyingCollection().UpdateMany(transform, predicate));
            });
            return tcs.Task;
        }

        /// <summary>
        /// Update many document based on merge current document with extend expression. Use your class with initializers. 
        /// Eg: col.UpdateMany(x => new Customer { Name = x.Name.ToUpper(), Salary: 100 }, x => x.Name == "John")
        /// </summary>
        public Task<int> UpdateManyAsync(Expression<Func<T, T>> extend, Expression<Func<T, bool>> predicate)
        {
            var tcs = new TaskCompletionSource<int>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(GetUnderlyingCollection().UpdateMany(extend, predicate));
            });
            return tcs.Task;
        }
    }
}
