using System.Linq;
using FluentAssertions;
using Xunit;
using LiteDB.Async;
using LiteDB;
using System.Threading;
using System.Threading.Tasks;

namespace Tests.LiteDB.Async
{
    public class DropCollection_Tests
    {
        [Fact]
        public async Task DropCollection()
        {
            using (var file = new TempFile())
            using (var db = new LiteDatabaseAsync(file.Filename))
            {
                (await db.GetCollectionNamesAsync()).Should().NotContain("col");

                var col = db.GetCollection("col");

                await col.InsertAsync(new BsonDocument { ["a"] = 1 });

                (await db.GetCollectionNamesAsync()).Should().Contain("col");

                await db.DropCollectionAsync("col");

                (await db.GetCollectionNamesAsync()).Should().NotContain("col");
            }
        }

        [Fact]
        public async Task InsertDropCollection()
        {
            using (var file = new TempFile())
            {
                using (var db = new LiteDatabaseAsync(file.Filename))
                {
                    var col = db.GetCollection("test");
                    await col.InsertAsync(new BsonDocument { ["_id"] = 1 });
                    await db.DropCollectionAsync("test");
                    await db.RebuildAsync();
                }

                using (var db = new LiteDatabaseAsync(file.Filename))
                {
                    var col = db.GetCollection("test");
                    await col.InsertAsync(new BsonDocument { ["_id"] = 1 });
                }
            }
        }
    }
}