using System;
using LiteDB;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace litedbasync
{
    public class LiteCollectionAsync<T>
    {
        private readonly ILiteCollection<T> _liteCollection;
        internal LiteCollectionAsync(ILiteCollection<T> liteCollection)
        {
            _liteCollection = liteCollection;
        }

        public async Task<bool> UpsertAsync(T entity)
        {
            return await Task.FromResult<bool>(true);
        }

        public async Task<List<T>> ToListAsync()
        {
            return new List<T>();
        }
    }
}