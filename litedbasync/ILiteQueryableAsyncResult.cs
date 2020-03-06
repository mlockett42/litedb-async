using System.Threading.Tasks;
using System.Collections.Generic;
using LiteDB;

namespace litedbasync
{
    public interface ILiteQueryableAsyncResult<T>
    {
        ILiteQueryableAsyncResult<T> Limit(int limit);
        ILiteQueryableAsyncResult<T> Skip(int offset);
        ILiteQueryableAsyncResult<T> Offset(int offset);
        ILiteQueryableAsyncResult<T> ForUpdate();

        //Task<BsonDocument> GetPlanAsync();
        //Task<IBsonDataReader> ExecuteReaderAsync();
        Task<IEnumerable<BsonDocument>> ToDocumentsAsync();
        Task<IEnumerable<T>> ToEnumerableAsync();
        Task<List<T>> ToListAsync();
        Task<T[]> ToArrayAsync();

        Task<int> IntoAsync(string newCollection, BsonAutoId autoId = BsonAutoId.ObjectId);

        Task<T> FirstAsync();
        Task<T> FirstOrDefaultAsync();
        Task<T> SingleAsync();
        Task<T> SingleOrDefaultAsync();

        Task<int> CountAsync();
        Task<long> LongCountAsync();
        Task<bool> ExistsAsync();
    }

}
