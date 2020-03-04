using System;
using LiteDB;
using System.Threading;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace litedbasync
{
    public class LiteQueryableAsync<T>
    {
        private readonly ILiteQueryable<T> _wrappedQuery;
        private readonly LiteDatabaseAsync _liteDatabaseAsync;

        public LiteQueryableAsync(ILiteQueryable<T> wrappedQuery, LiteDatabaseAsync liteDatabaseAsync)
        {
            _wrappedQuery = wrappedQuery;
            _liteDatabaseAsync = liteDatabaseAsync;
        }

        #region Includes

        /// <summary>
        /// Load cross reference documents from path expression (DbRef reference)
        /// </summary>
        public LiteQueryableAsync<T> Include<K>(Expression<Func<T, K>> path)
        {
            // Note the wrapped function in LiteDB mutates the ILiteQueryable object
            _wrappedQuery.Include(path);
            return this;
        }

        /// <summary>
        /// Load cross reference documents from path expression (DbRef reference)
        /// </summary>
        public LiteQueryableAsync<T> Include(BsonExpression path)
        {
            _wrappedQuery.Include(path);
            return this;
        }

        /// <summary>
        /// Load cross reference documents from path expression (DbRef reference)
        /// </summary>
        public LiteQueryableAsync<T> Include(List<BsonExpression> paths)
        {
            _wrappedQuery.Include(paths);
            return this;
        }

        #endregion

        #region Where

        /// <summary>
        /// Filters a sequence of documents based on a predicate expression
        /// </summary>
        public LiteQueryableAsync<T> Where(BsonExpression predicate)
        {
            _wrappedQuery.Where(predicate);
            return this;
        }

        /// <summary>
        /// Filters a sequence of documents based on a predicate expression
        /// </summary>
        public LiteQueryableAsync<T> Where(string predicate, BsonDocument parameters)
        {
            _wrappedQuery.Where(predicate, parameters);
            return this;
        }

        /// <summary>
        /// Filters a sequence of documents based on a predicate expression
        /// </summary>
        public LiteQueryableAsync<T> Where(string predicate, params BsonValue[] args)
        {
            _wrappedQuery.Where(predicate, args);
            return this;
        }

        /// <summary>
        /// Filters a sequence of documents based on a predicate expression
        /// </summary>
        public LiteQueryableAsync<T> Where(Expression<Func<T, bool>> predicate)
        {
            _wrappedQuery.Where(predicate);
            return this;
        }

        #endregion

        #region OrderBy

        /// <summary>
        /// Sort the documents of resultset in ascending (or descending) order according to a key (support only one OrderBy)
        /// </summary>
        public LiteQueryableAsync<T> OrderBy(BsonExpression keySelector, int order = Query.Ascending)
        {
            _wrappedQuery.OrderBy(keySelector, order);
            return this;
        }

        /// <summary>
        /// Sort the documents of resultset in ascending (or descending) order according to a key (support only one OrderBy)
        /// </summary>
        public LiteQueryableAsync<T> OrderBy<K>(Expression<Func<T, K>> keySelector, int order = Query.Ascending)
        {
            _wrappedQuery.OrderBy(keySelector, order);
            return this;
        }

        /// <summary>
        /// Sort the documents of resultset in descending order according to a key (support only one OrderBy)
        /// </summary>
        public LiteQueryableAsync<T> OrderByDescending(BsonExpression keySelector)
        {
            _wrappedQuery.OrderByDescending(keySelector);
            return this;
        }

        /// <summary>
        /// Sort the documents of resultset in descending order according to a key (support only one OrderBy)
        /// </summary>
        public LiteQueryableAsync<T> OrderByDescending<K>(Expression<Func<T, K>> keySelector)
        {
            _wrappedQuery.OrderByDescending(keySelector);
            return this;
        }

        #endregion

        public Task<List<T>> ToListAsync()
        {
            var tcs = new TaskCompletionSource<List<T>>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(_wrappedQuery.ToList());
            });
            return tcs.Task;
        }

    }
}
