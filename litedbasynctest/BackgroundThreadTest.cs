using System;
using Xunit;
using LiteDB.Async;
using System.IO;
using System.Threading.Tasks;

namespace LiteDB.Async.Test
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