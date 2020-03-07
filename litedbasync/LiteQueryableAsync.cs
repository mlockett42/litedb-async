using System;
using LiteDB;
using System.Threading;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace litedbasync
{
    public class LiteQueryableAsync<T> : ILiteQueryableAsync<T>
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
        public ILiteQueryableAsync<T> Include<K>(Expression<Func<T, K>> path)
        {
            // Note the wrapped function in LiteDB mutates the ILiteQueryable object
            _wrappedQuery.Include(path);
            return this;
        }

        /// <summary>
        /// Load cross reference documents from path expression (DbRef reference)
        /// </summary>
        public ILiteQueryableAsync<T> Include(BsonExpression path)
        {
            _wrappedQuery.Include(path);
            return this;
        }

        /// <summary>
        /// Load cross reference documents from path expression (DbRef reference)
        /// </summary>
        public ILiteQueryableAsync<T> Include(List<BsonExpression> paths)
        {
            _wrappedQuery.Include(paths);
            return this;
        }

        #endregion

        #region Where

        /// <summary>
        /// Filters a sequence of documents based on a predicate expression
        /// </summary>
        public ILiteQueryableAsync<T> Where(BsonExpression predicate)
        {
            _wrappedQuery.Where(predicate);
            return this;
        }

        /// <summary>
        /// Filters a sequence of documents based on a predicate expression
        /// </summary>
        public ILiteQueryableAsync<T> Where(string predicate, BsonDocument parameters)
        {
            _wrappedQuery.Where(predicate, parameters);
            return this;
        }

        /// <summary>
        /// Filters a sequence of documents based on a predicate expression
        /// </summary>
        public ILiteQueryableAsync<T> Where(string predicate, params BsonValue[] args)
        {
            _wrappedQuery.Where(predicate, args);
            return this;
        }

        /// <summary>
        /// Filters a sequence of documents based on a predicate expression
        /// </summary>
        public ILiteQueryableAsync<T> Where(Expression<Func<T, bool>> predicate)
        {
            _wrappedQuery.Where(predicate);
            return this;
        }

        #endregion

        #region OrderBy

        /// <summary>
        /// Sort the documents of resultset in ascending (or descending) order according to a key (support only one OrderBy)
        /// </summary>
        public ILiteQueryableAsync<T> OrderBy(BsonExpression keySelector, int order = Query.Ascending)
        {
            _wrappedQuery.OrderBy(keySelector, order);
            return this;
        }

        /// <summary>
        /// Sort the documents of resultset in ascending (or descending) order according to a key (support only one OrderBy)
        /// </summary>
        public ILiteQueryableAsync<T> OrderBy<K>(Expression<Func<T, K>> keySelector, int order = Query.Ascending)
        {
            _wrappedQuery.OrderBy(keySelector, order);
            return this;
        }

        /// <summary>
        /// Sort the documents of resultset in descending order according to a key (support only one OrderBy)
        /// </summary>
        public ILiteQueryableAsync<T> OrderByDescending(BsonExpression keySelector)
        {
            _wrappedQuery.OrderByDescending(keySelector);
            return this;
        }

        /// <summary>
        /// Sort the documents of resultset in descending order according to a key (support only one OrderBy)
        /// </summary>
        public ILiteQueryableAsync<T> OrderByDescending<K>(Expression<Func<T, K>> keySelector)
        {
            _wrappedQuery.OrderByDescending(keySelector);
            return this;
        }

        #endregion

        #region GroupBy

        /// <summary>
        /// Groups the documents of resultset according to a specified key selector expression (support only one GroupBy)
        /// </summary>
        public ILiteQueryableAsync<T> GroupBy(BsonExpression keySelector)
        {
            _wrappedQuery.GroupBy(keySelector);
            return this;
        }

        #endregion

        #region Having

        /// <summary>
        /// Filter documents after group by pipe according to predicate expression (requires GroupBy and support only one Having)
        /// </summary>
        public ILiteQueryableAsync<T> Having(BsonExpression predicate)
        {
            _wrappedQuery.Having(predicate);
            return this;
        }

        #endregion

        #region Select

        /// <summary>
        /// Transform input document into a new output document. Can be used with each document, group by or all source
        /// </summary>
        public ILiteQueryableAsyncResult<BsonDocument> Select(BsonExpression selector)
        {
            return new LiteQueryableAsync<BsonDocument>((ILiteQueryable<BsonDocument>)_wrappedQuery.Select(selector), _liteDatabaseAsync);
        }

        /// <summary>
        /// Project each document of resultset into a new document/value based on selector expression
        /// </summary>
        public ILiteQueryableAsyncResult<K> Select<K>(Expression<Func<T, K>> selector)
        {
            return new LiteQueryableAsync<K>((ILiteQueryable<K>)_wrappedQuery.Select(selector), _liteDatabaseAsync);
        }

        #endregion

        #region Offset/Limit/ForUpdate

        /// <summary>
        /// Execute query locking collection in write mode. This is avoid any other thread change results after read document and before transaction ends
        /// </summary>
        public ILiteQueryableAsyncResult<T> ForUpdate()
        {
            _wrappedQuery.ForUpdate();
            return this;
        }

        /// <summary>
        /// Bypasses a specified number of documents in resultset and retun the remaining documents (same as Skip)
        /// </summary>
        public ILiteQueryableAsyncResult<T> Offset(int offset)
        {
            _wrappedQuery.Offset(offset);
            return this;
        }

        /// <summary>
        /// Bypasses a specified number of documents in resultset and retun the remaining documents (same as Offset)
        /// </summary>
        public ILiteQueryableAsyncResult<T> Skip(int offset) => this.Offset(offset);

        /// <summary>
        /// Return a specified number of contiguous documents from start of resultset
        /// </summary>
        public ILiteQueryableAsyncResult<T> Limit(int limit)
        {
            _wrappedQuery.Limit(limit);
            return this;
        }

        #endregion

        #region Execute Result

        /*
        /// <summary>
        /// Execute query and returns resultset as generic BsonDataReader
        /// </summary>
        public IBsonDataReader ExecuteReader()
        {
            _query.ExplainPlan = false;

            return _engine.Query(_collection, _query);
        }
        */

        /// <summary>
        /// Execute query and return resultset as IEnumerable of documents
        /// </summary>
        public Task<IEnumerable<BsonDocument>> ToDocumentsAsync()
        {
            var tcs = new TaskCompletionSource<IEnumerable<BsonDocument>>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(_wrappedQuery.ToDocuments());
            });
            return tcs.Task;
        }

        /// <summary>
        /// Execute query and return resultset as IEnumerable of T. If T is a ValueType or String, return values only (not documents)
        /// </summary>
        public Task<IEnumerable<T>> ToEnumerableAsync()
        {
            var tcs = new TaskCompletionSource<IEnumerable<T>>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(_wrappedQuery.ToEnumerable());
            });
            return tcs.Task;
        }

        /// <summary>
        /// Execute query and return results as a List
        /// </summary>
        public Task<List<T>> ToListAsync()
        {
            var tcs = new TaskCompletionSource<List<T>>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(_wrappedQuery.ToList());
            });
            return tcs.Task;
        }

        /// <summary>
        /// Execute query and return results as an Array
        /// </summary>
        public Task<T[]> ToArrayAsync()
        {
            var tcs = new TaskCompletionSource<T[]>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(_wrappedQuery.ToArray());
            });
            return tcs.Task;
        }

        /// <summary>
        /// Get execution plan over current query definition to see how engine will execute query
        /// </summary>
        /*public BsonDocument GetPlan()
        {
            _query.ExplainPlan = true;

            using (var reader = _engine.Query(_collection, _query))
            {
                return reader.Current.AsDocument;
            }
        }*/

        #endregion

        #region Execute Single/First

        /// <summary>
        /// Returns the only document of resultset, and throw an exception if there not exactly one document in the sequence
        /// </summary>
        public Task<T> SingleAsync()
        {
            var tcs = new TaskCompletionSource<T>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(_wrappedQuery.Single());
            });
            return tcs.Task;
        }

        /// <summary>
        /// Returns the only document of resultset, or null if resultset are empty; this method throw an exception if there not exactly one document in the sequence
        /// </summary>
        public Task<T> SingleOrDefaultAsync()
        {
            var tcs = new TaskCompletionSource<T>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(_wrappedQuery.SingleOrDefault());
            });
            return tcs.Task;
        }

        /// <summary>
        /// Returns first document of resultset
        /// </summary>
        public Task<T> FirstAsync()
        {
            var tcs = new TaskCompletionSource<T>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(_wrappedQuery.First());
            });
            return tcs.Task;
        }

        /// <summary>
        /// Returns first document of resultset or null if resultset are empty
        /// </summary>
        public Task<T> FirstOrDefaultAsync()
        {
            var tcs = new TaskCompletionSource<T>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(_wrappedQuery.FirstOrDefault());
            });
            return tcs.Task;
        }

        #endregion

        #region Execute Count

        /// <summary>
        /// Execute Count methos in filter query
        /// </summary>
        public Task<int> CountAsync()
        {
            var tcs = new TaskCompletionSource<int>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(_wrappedQuery.Count());
            });
            return tcs.Task;
        }

        /// <summary>
        /// Execute Count methos in filter query
        /// </summary>
        public Task<long> LongCountAsync()
        {
            var tcs = new TaskCompletionSource<long>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(_wrappedQuery.LongCount());
            });
            return tcs.Task;
        }

        /// <summary>
        /// Returns true/false if query returns any result
        /// </summary>
        public Task<bool> ExistsAsync()
        {
            var tcs = new TaskCompletionSource<bool>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(_wrappedQuery.Exists());
            });
            return tcs.Task;
        }

        #endregion

        #region Execute Into

        public Task<int> IntoAsync(string newCollection, BsonAutoId autoId = BsonAutoId.ObjectId)
        {
            var tcs = new TaskCompletionSource<int>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(_wrappedQuery.Into(newCollection, autoId));
            });
            return tcs.Task;
        }

        #endregion
    }
}
