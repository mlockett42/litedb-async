using System.Threading.Tasks;
using System.Collections.Generic;

namespace LiteDB.Async
{
    public partial class LiteCollectionAsync<T>
    {
        /// <summary>
        /// Insert a new entity to this collection. Document Id must be a new value in collection - Returns document Id
        /// </summary>
        public Task<BsonValue> InsertAsync(T entity)
        {
            var tcs = new TaskCompletionSource<BsonValue>();
            Database.Enqueue(tcs, () => {
                tcs.SetResult(UnderlyingCollection.Insert(entity));
            });
            return tcs.Task;
        }

        /// <summary>
        /// Insert a new document to this collection using passed id value.
        /// </summary>
        public Task InsertAsync(BsonValue id, T entity)
        {
            var tcs = new TaskCompletionSource<bool>();
            Database.Enqueue(tcs, () => {
                UnderlyingCollection.Insert(id, entity);
                tcs.SetResult(true);
            });
            return tcs.Task;
        }

        /// <summary>
        /// Insert an array of new documents to this collection. Document Id must be a new value in collection. Can be set buffer size to commit at each N documents
        /// </summary>
        public Task<int> InsertAsync(IEnumerable<T> entities)
        {
            var tcs = new TaskCompletionSource<int>();
            Database.Enqueue(tcs, () => {
                tcs.SetResult(UnderlyingCollection.Insert(entities));
            });
            return tcs.Task;
        }

        /// <summary>
        /// Implements bulk insert documents in a collection. Usefull when need lots of documents.
        /// </summary>
        public Task<int> InsertBulkAsync(IEnumerable<T> entities, int batchSize = 5000)
        {
            var tcs = new TaskCompletionSource<int>();
            Database.Enqueue(tcs, () => {
                tcs.SetResult(UnderlyingCollection.InsertBulk(entities, batchSize));
            });
            return tcs.Task;
        }
    }
}
