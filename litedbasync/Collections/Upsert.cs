using System.Threading.Tasks;
using System.Collections.Generic;

namespace LiteDB.Async
{
    public partial class LiteCollectionAsync<T>
    {
        /// <summary>
        /// Insert or Update a document in this collection.
        /// </summary>
        public Task<bool> UpsertAsync(T entity)
        {
            return Database.Enqueue(
                () => UnderlyingCollection.Upsert(entity));
        }

        /// <summary>
        /// Insert or Update all documents
        /// </summary>
        public Task<int> UpsertAsync(IEnumerable<T> entities)
        {
            return Database.Enqueue(
                () => UnderlyingCollection.Upsert(entities));
        }

        /// <summary>
        /// Insert or Update a document in this collection.
        /// </summary>
        public Task<bool> UpsertAsync(BsonValue id, T entity)
        {
            return Database.Enqueue(
                () => UnderlyingCollection.Upsert(id, entity));
        }
    }
}