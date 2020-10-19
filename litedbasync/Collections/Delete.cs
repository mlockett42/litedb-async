using System;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace LiteDB.Async
{
    public partial class LiteCollectionAsync<T>
    {
        /// <summary>
        /// Delete a single document on collection based on _id index. Returns true if document was deleted
        /// </summary>
        public Task<bool> DeleteAsync(BsonValue id)
        {
            var tcs = new TaskCompletionSource<bool>();
            Database.Enqueue(tcs, () => {
                tcs.SetResult(UnderlyingCollection.Delete(id));
            });
            return tcs.Task;
        }

        /// <summary>
        /// Delete all documents based on predicate expression. Returns how many documents was deleted
        /// </summary>
        public Task<int> DeleteManyAsync(BsonExpression predicate)
        {
            var tcs = new TaskCompletionSource<int>();
            Database.Enqueue(tcs, () => {
                tcs.SetResult(UnderlyingCollection.DeleteMany(predicate));
            });
            return tcs.Task;
        }

        /// <summary>
        /// Delete all documents based on predicate expression. Returns how many documents was deleted
        /// </summary>
        public Task<int> DeleteManyAsync(string predicate, BsonDocument parameters)
        {
            var tcs = new TaskCompletionSource<int>();
            Database.Enqueue(tcs, () => {
                tcs.SetResult(UnderlyingCollection.DeleteMany(predicate, parameters));
            });
            return tcs.Task;
        }

        /// <summary>
        /// Delete all documents based on predicate expression. Returns how many documents was deleted
        /// </summary>
        public Task<int> DeleteManyAsync(string predicate, params BsonValue[] args)
        {
            var tcs = new TaskCompletionSource<int>();
            Database.Enqueue(tcs, () => {
                tcs.SetResult(UnderlyingCollection.DeleteMany(predicate, args));
            });
            return tcs.Task;
        }

        /// <summary>
        /// Delete all documents based on predicate expression. Returns how many documents was deleted
        /// </summary>
        public Task<int> DeleteManyAsync(Expression<Func<T, bool>> predicate)
        {
            var tcs = new TaskCompletionSource<int>();
            Database.Enqueue(tcs, () => {
                tcs.SetResult(UnderlyingCollection.DeleteMany(predicate));
            });
            return tcs.Task;
        }
 
         /// <summary>
        /// Delete all documents inside collection. Returns how many documents was deleted. Run inside current transaction
        /// </summary>
        public Task<int> DeleteAllAsync()
        {
            var tcs = new TaskCompletionSource<int>();
            Database.Enqueue(tcs, () => {
                tcs.SetResult(UnderlyingCollection.DeleteAll());
            });
            return tcs.Task;
        }
   }
}