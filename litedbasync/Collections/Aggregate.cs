using System;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace LiteDB.Async
{
    public partial class LiteCollectionAsync<T>
    {
        /// <summary>
        /// Get document count in collection
        /// </summary>
        public Task<int> CountAsync()
        {
            var tcs = new TaskCompletionSource<int>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(GetUnderlyingCollection().Count());
            });
            return tcs.Task;
        }

        /// <summary>
        /// Get document count in collection using predicate filter expression
        /// </summary>
        public Task<int> CountAsync(BsonExpression predicate)
        {
            var tcs = new TaskCompletionSource<int>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(GetUnderlyingCollection().Count(predicate));
            });
            return tcs.Task;
        }

        /// <summary>
        /// Get document count in collection using predicate filter expression
        /// </summary>
        public Task<int> CountAsync(string predicate, BsonDocument parameters)
        {
            var tcs = new TaskCompletionSource<int>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(GetUnderlyingCollection().Count(predicate, parameters));
            });
            return tcs.Task;

        }

        /// <summary>
        /// Count documents matching a query. This method does not deserialize any documents. Needs indexes on query expression
        /// </summary>
        public Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            var tcs = new TaskCompletionSource<int>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(GetUnderlyingCollection().Count(predicate));
            });
            return tcs.Task;
        }

        /// <summary>
        /// Get document count in collection using predicate filter expression
        /// </summary>
        public Task<int> CountAsync(Query query)
        {
            var tcs = new TaskCompletionSource<int>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(GetUnderlyingCollection().Count(query));
            });
            return tcs.Task;
        }

        /// <summary>
        /// Get document count in collection
        /// </summary>
        public Task<long> LongCountAsync()
        {
            var tcs = new TaskCompletionSource<long>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(GetUnderlyingCollection().LongCount());
            });
            return tcs.Task;
        }

        /// <summary>
        /// Get document count in collection using predicate filter expression
        /// </summary>
        public Task<long> LongCountAsync(BsonExpression predicate)
        {
            var tcs = new TaskCompletionSource<long>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(GetUnderlyingCollection().LongCount(predicate));
            });
            return tcs.Task;
        }

        /// <summary>
        /// Get document count in collection using predicate filter expression
        /// </summary>
        public Task<long> LongCountAsync(string predicate, BsonDocument parameters)
        {
            var tcs = new TaskCompletionSource<long>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(GetUnderlyingCollection().LongCount(predicate, parameters));
            });
            return tcs.Task;
        }

        /// <summary>
        /// Get document count in collection using predicate filter expression
        /// </summary>
        public Task<long> LongCountAsync(string predicate, params BsonValue[] args)
        {
            var tcs = new TaskCompletionSource<long>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(GetUnderlyingCollection().LongCount(predicate, args));
            });
            return tcs.Task;
        }

        /// <summary>
        /// Get document count in collection using predicate filter expression
        /// </summary>
        public Task<long> LongCountAsync(Expression<Func<T, bool>> predicate)
        {
            var tcs = new TaskCompletionSource<long>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(GetUnderlyingCollection().LongCount(predicate));
            });
            return tcs.Task;
        }

        /// <summary>
        /// Get document count in collection using predicate filter expression
        /// </summary>
        public Task<long> LongCountAsync(Query query)
        {
            var tcs = new TaskCompletionSource<long>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(GetUnderlyingCollection().LongCount(query));
            });
            return tcs.Task;
        }

        /// <summary>
        /// Get true if collection contains at least 1 document that satisfies the predicate expression
        /// </summary>
        public Task<bool> ExistsAsync(BsonExpression predicate)
        {
            var tcs = new TaskCompletionSource<bool>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(GetUnderlyingCollection().Exists(predicate));
            });
            return tcs.Task;
        }

        /// <summary>
        /// Get true if collection contains at least 1 document that satisfies the predicate expression
        /// </summary>
        public Task<bool> ExistsAsync(string predicate, BsonDocument parameters)
        {
            var tcs = new TaskCompletionSource<bool>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(GetUnderlyingCollection().Exists(predicate, parameters));
            });
            return tcs.Task;
        }

        /// <summary>
        /// Get true if collection contains at least 1 document that satisfies the predicate expression
        /// </summary>
        public Task<bool> ExistsAsync(string predicate, params BsonValue[] args)
        {
            var tcs = new TaskCompletionSource<bool>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(GetUnderlyingCollection().Exists(predicate, args));
            });
            return tcs.Task;
        }

        /// <summary>
        /// Get true if collection contains at least 1 document that satisfies the predicate expression
        /// </summary>
        public Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            var tcs = new TaskCompletionSource<bool>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(GetUnderlyingCollection().Exists(predicate));
            });
            return tcs.Task;
        }

        /// <summary>
        /// Get true if collection contains at least 1 document that satisfies the predicate expression
        /// </summary>
        public Task<bool> ExistsAsync(Query query)
        {
            var tcs = new TaskCompletionSource<bool>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(GetUnderlyingCollection().Exists(query));
            });
            return tcs.Task;
        }

        #region Min/Max

        /// <summary>
        /// Returns the min value from specified key value in collection
        /// </summary>
        public Task<BsonValue> MinAsync(BsonExpression keySelector)
        {
            var tcs = new TaskCompletionSource<BsonValue>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(GetUnderlyingCollection().Min(keySelector));
            });
            return tcs.Task;
        }

        /// <summary>
        /// Returns the min value of _id index
        /// </summary>
        public Task<BsonValue> MinAsync()
        {
            var tcs = new TaskCompletionSource<BsonValue>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(GetUnderlyingCollection().Min());
            });
            return tcs.Task;
        }

        /// <summary>
        /// Returns the min value from specified key value in collection
        /// </summary>
        public Task<K> MinAsync<K>(Expression<Func<T, K>> keySelector)
        {
            var tcs = new TaskCompletionSource<K>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(GetUnderlyingCollection().Min(keySelector));
            });
            return tcs.Task;
        }

        /// <summary>
        /// Returns the max value from specified key value in collection
        /// </summary>
        public Task<BsonValue> MaxAsync(BsonExpression keySelector)
        {
            var tcs = new TaskCompletionSource<BsonValue>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(GetUnderlyingCollection().Max(keySelector));
            });
            return tcs.Task;
        }

        /// <summary>
        /// Returns the max _id index key value
        /// </summary>
        public Task<BsonValue> MaxAsync()
        {
            var tcs = new TaskCompletionSource<BsonValue>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(GetUnderlyingCollection().Max());
            });
            return tcs.Task;
        }

        /// <summary>
        /// Returns the last/max field using a linq expression
        /// </summary>
        public Task<K> MaxAsync<K>(Expression<Func<T, K>> keySelector)
        {
            var tcs = new TaskCompletionSource<K>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(GetUnderlyingCollection().Max(keySelector));
            });
            return tcs.Task;

        }
        #endregion
    }
}
