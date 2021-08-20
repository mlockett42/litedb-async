using System;
using System.Diagnostics;
using Xunit;
using LiteDB.Async;
using System.IO;
using System.Threading.Tasks;

namespace Tests.LiteDB.Async
{
    public class BackgroundThreadTest
    {
        [Fact]
        public void TestCanOpenThenCloseThenReopenDatabase()
        {
            string databasePath = Path.Combine(Path.GetTempPath(), "litedbn-async-testing-" + Path.GetRandomFileName() + ".db");
            using (var db1 = new LiteDatabaseAsync(databasePath))
            {

            }
            using (var db2 = new LiteDatabaseAsync(databasePath))
            {
                
            }
            File.Delete(databasePath);
        }

        [Fact]
        public async Task TestCanDisposeDatabaseTimely()
        {
            string databasePath = Path.Combine(Path.GetTempPath(),
                "litedbn-async-testing-" + Path.GetRandomFileName() + ".db");
            var db = new LiteDatabaseAsync(databasePath);

            // Ensure to do some stuff to open the database
            var collection = db.GetCollection<SimplePerson>();
            await collection.InsertAsync(new SimplePerson()).ConfigureAwait(false);

            var stopwatch = Stopwatch.StartNew();
            db.Dispose();
            stopwatch.Stop();

            Assert.InRange(stopwatch.Elapsed.TotalSeconds, 0, 4.99);
        }

        [Fact]
        public void TestOpeningDatabaseASecondTimeCausesAnException()
        {
            string databasePath = Path.Combine(Path.GetTempPath(), "litedbn-async-testing-" + Path.GetRandomFileName() + ".db");
            using(var db1 = new LiteDatabaseAsync(databasePath))
            {
                Assert.Throws<IOException>(() => {
                    var db2 = new LiteDatabaseAsync(databasePath);
                });
            }
            File.Delete(databasePath);
        }
    }
}