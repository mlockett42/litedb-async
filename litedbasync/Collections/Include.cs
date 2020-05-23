using System;
using System.Linq.Expressions;

namespace LiteDB.Async
{
    public partial class LiteCollectionAsync<T>
    {
        /// <summary>
        /// Run an include action in each document returned by Find(), FindById(), FindOne() and All() methods to load DbRef documents
        /// Returns a new Collection with this action included
        /// </summary>
        public LiteCollectionAsync<T> Include<K>(Expression<Func<T, K>> keySelector)
        {
            return new LiteCollectionAsync<T>(GetUnderlyingCollection().Include(keySelector), _liteDatabaseAsync);
        }

        /// <summary>
        /// Run an include action in each document returned by Find(), FindById(), FindOne() and All() methods to load DbRef documents
        /// Returns a new Collection with this action included
        /// </summary>
        public LiteCollectionAsync<T> Include(BsonExpression keySelector)
        {
            return new LiteCollectionAsync<T>(GetUnderlyingCollection().Include(keySelector), _liteDatabaseAsync);
        }
    }
}
