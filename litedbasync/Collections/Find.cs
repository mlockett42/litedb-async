using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LiteDB.Async
{
    public partial class LiteCollectionAsync<T>
    {
        #region Find

        /// <summary>
        /// Find documents inside a collection using predicate expression.
        /// </summary>
        public Task<IEnumerable<T>> FindAsync(BsonExpression predicate, int skip = 0, int limit = int.MaxValue)
        {
            VerifyNoClosedTransaction();
            return Database.EnqueueAsync(
                () => UnderlyingCollection.Find(predicate, skip, limit));
        }

        /// <summary>
        /// Find documents inside a collection using query definition.
        /// </summary>
        public Task<IEnumerable<T>> FindAsync(Query query, int skip = 0, int limit = int.MaxValue)
        {
            VerifyNoClosedTransaction();
            return Database.EnqueueAsync(
                () => UnderlyingCollection.Find(query, skip, limit));
        }

        /// <summary>
        /// Find documents inside a collection using predicate expression.
        /// </summary>
        public Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, int skip = 0, int limit = int.MaxValue)
        {
            VerifyNoClosedTransaction();
            return Database.EnqueueAsync(
                () => UnderlyingCollection.Find(predicate, skip, limit));
        }

        #endregion

        #region FindById + One + All

        /// <summary>
        /// Find a document using Document Id. Returns null if not found.
        /// </summary>
        public Task<T> FindByIdAsync(BsonValue id)
        {
            VerifyNoClosedTransaction();
            return Database.EnqueueAsync(
                () => UnderlyingCollection.FindById(id));
        }

        /// <summary>
        /// Find the first document using predicate expression. Returns null if not found
        /// </summary>
        public Task<T> FindOneAsync(BsonExpression predicate)
        {
            VerifyNoClosedTransaction();
            return Database.EnqueueAsync(
                () => UnderlyingCollection.FindOne(predicate));
        }

        /// <summary>
        /// Find the first document using predicate expression. Returns null if not found
        /// </summary>
        public Task<T> FindOneAsync(string predicate, BsonDocument parameters)
        {
            VerifyNoClosedTransaction();
            return Database.EnqueueAsync(
                () => UnderlyingCollection.FindOne(predicate, parameters));
        }

        /// <summary>
        /// Find the first document using predicate expression. Returns null if not found
        /// </summary>
        public Task<T> FindOneAsync(BsonExpression predicate, params BsonValue[] args)
        {
            VerifyNoClosedTransaction();
            return Database.EnqueueAsync(
                () => UnderlyingCollection.FindOne(predicate, args));
        }

        /// <summary>
        /// Find the first document using predicate expression. Returns null if not found
        /// </summary>
        public Task<T> FindOneAsync(Expression<Func<T, bool>> predicate)
        {
            VerifyNoClosedTransaction();
            return Database.EnqueueAsync(
                () => UnderlyingCollection.FindOne(predicate));
        }

        /// <summary>
        /// Find the first document using defined query structure. Returns null if not found
        /// </summary>
        public Task<T> FindOneAsync(Query query)
        {
            VerifyNoClosedTransaction();
            return Database.EnqueueAsync(
                () => UnderlyingCollection.FindOne(query));
        }

        /// <summary>
        /// Returns all documents inside collection order by _id index.
        /// </summary>
        public Task<IEnumerable<T>> FindAllAsync()
        {
            VerifyNoClosedTransaction();
            return Database.EnqueueAsync(
                () => UnderlyingCollection.FindAll());
        }

        #endregion
    }
}
