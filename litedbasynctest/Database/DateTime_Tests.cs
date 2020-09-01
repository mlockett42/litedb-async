using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace LiteDB.Async.Test
{
    public class DateTime_Tests : IDisposable
    {
        readonly string _databasePath;
        readonly LiteDatabaseAsync _db;

        bool _disposed;

        public DateTime_Tests()
        {
            _databasePath = Path.Combine(Path.GetTempPath(), $"litedbn-async-testing-{Path.GetRandomFileName()}.db");
            _db = new LiteDatabaseAsync(_databasePath) { UtcDate = true };
        }

        [Fact]
        public async Task TestCanRoundtripUtcDateTime()
        {
            var collection = _db.GetCollection<ThingThatHappened>();
            var now = new DateTime(1979, 3, 19, 12, 00, 00, DateTimeKind.Utc);
            var evt = new ThingThatHappened { Time = now };

            var upsertResult = await collection.UpsertAsync(evt);
            Assert.True(upsertResult);

            var listResult = await collection.Query().ToListAsync();
            Assert.Single(listResult);
            var resultThing = listResult[0];
            Assert.Equal(now, resultThing.Time);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || _disposed) return;

            try
            {
                _db.Dispose();
                File.Delete(_databasePath);
            }
            finally
            {
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}