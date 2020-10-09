using System.Globalization;
using System.Threading.Tasks;
using LiteDB;
using LiteDB.Async;
using LiteDB.Engine;
using Tests.LiteDB.Async;
using Xunit;

namespace litedbasynctest.Database
{
    public class Rebuild_Underlying_Database
    {
        [Fact]
        public async Task CanRebuildDatabaseWithCaseSensitiveCultureInvariantCollation()
        {
            using var file = new TempFile();
            using var database = new LiteDatabaseAsync(file.Filename);

            var collation = new Collation(CultureInfo.InvariantCulture.LCID, CompareOptions.Ordinal);
            
            database.GetUnderlyingDatabase().Rebuild(new RebuildOptions {Collation = collation});

            var docs = database.GetCollection<BsonDocument>("test");

            await docs.InsertAsync(new BsonDocument { { "_id", "some-key" } });
            await docs.InsertAsync(new BsonDocument { { "_id", "some-KEY" } });
        }
    }
}