using System.Threading.Tasks;

namespace LiteDB.Async
{
    public interface ILiteDatabaseTransactionAsync : ILiteDatabaseAsync
    {
        #region Transactions
        /// <summary>
        /// Commit current transaction
        /// </summary>
        Task<bool> CommitAsync();

        /// <summary>
        /// Rollback current transaction
        /// </summary>
        Task<bool> RollbackAsync();

        #endregion
    }
}
