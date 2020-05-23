namespace LiteDB.Async
{
    /// <summary>
    /// Wraps a LiteCollection which will only be queried in the background thread
    /// </summary>
    public partial class LiteCollectionAsync<T>
    {
        private readonly ILiteCollection<T> _liteCollection;
        private readonly LiteDatabaseAsync _liteDatabaseAsync;
        internal LiteCollectionAsync(ILiteCollection<T> liteCollection, LiteDatabaseAsync liteDatabaseAsync)
        {
            _liteCollection = liteCollection;
            _liteDatabaseAsync = liteDatabaseAsync;
        }

        /// <summary>
        /// The database this collection belongs to
        /// </summary>
        public LiteDatabaseAsync Database {
            get
            {
                return _liteDatabaseAsync;
            }
        }

        /// <summary>
        /// The underlying ILiteCollection we wrap
        /// </summary>
        public ILiteCollection<T> GetUnderlyingCollection()
        {
            return _liteCollection;
        }

        /// <summary>
        /// Return a new LiteQueryableAsync to build more complex queries
        /// </summary>
        public LiteQueryableAsync<T> Query()
        {
            return new LiteQueryableAsync<T>(GetUnderlyingCollection().Query(), _liteDatabaseAsync);
        }
    }
}