using System;
using LiteDB;
using System.Threading;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace litedbasync
{
    public class LiteQueryableAsync<T>
    {
        private readonly ILiteQueryable<T> _wrappedQuery;
        private readonly LiteDatabaseAsync _liteDatabaseAsync;

        public LiteQueryableAsync(ILiteQueryable<T> wrappedQuery, LiteDatabaseAsync liteDatabaseAsync)
        {
            _wrappedQuery = wrappedQuery;
            _liteDatabaseAsync = liteDatabaseAsync;
        }

        public Task<List<T>> ToListAsync()
        {
            var tcs = new TaskCompletionSource<List<T>>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(_wrappedQuery.ToList());
            });
            return tcs.Task;
        }

    }
}
