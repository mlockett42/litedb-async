using System;
using LiteDB;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LiteDB.Async
{
    public partial class LiteCollectionAsync<T>
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

        public LiteQueryableAsync<T> Query()
        {
            return new LiteQueryableAsync<T>(GetUnderlyingCollection().Query(), _liteDatabaseAsync);
        }
    }
}