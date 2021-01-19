using System.Threading.Tasks;

namespace LiteDB.Async
{
    public class LiteDatabaseTransactionAsync : LiteDatabaseAsync, ILiteDatabaseTransactionAsync
    {
        /// <summary>
        /// Starts new database with everything in a new transaction
        /// </summary>
        public LiteDatabaseTransactionAsync(string connectionString, BsonMapper mapper = null):base(connectionString, mapper)
        {
            _ = BeginTransInternalAsync();
        }

        private Task<bool> BeginTransInternalAsync()
        {
            var tcs = new TaskCompletionSource<bool>();
            Enqueue(tcs, () => {
                tcs.SetResult(UnderlyingDatabase.BeginTrans());
            });
            return tcs.Task;
        }

        /// <summary>
        /// Commit current transaction
        /// </summary>
        public Task<bool> CommitAsync()
        {
            var tcs = new TaskCompletionSource<bool>();
            Enqueue(tcs, () =>
            {
                tcs.SetResult(UnderlyingDatabase.Commit());
            });
            return tcs.Task;
        }

        /// <summary>
        /// Rollback current transaction
        /// </summary>
        public Task<bool> RollbackAsync()
        {
            var tcs = new TaskCompletionSource<bool>();
            Enqueue(tcs, () =>
            {
                tcs.SetResult(UnderlyingDatabase.Rollback());
            });
            return tcs.Task;
        }
    }
}
