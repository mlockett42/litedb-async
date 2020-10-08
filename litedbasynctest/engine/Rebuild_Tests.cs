using FluentAssertions;
using LiteDB.Engine;
using System;
using Xunit;
using LiteDB.Async;
using System.Threading;
using System.Threading.Tasks;
using LiteDB;

namespace Tests.LiteDB.Async
{
    public class Rebuild_Tests
    {
        [Fact]
        public async Task Rebuild_After_DropCollection()
        {
            using (var file = new TempFile())
            using (var db = new LiteDatabaseAsync(file.Filename))
            {
                var col = db.GetCollection<Zip>("zip");

                await col.InsertAsync(DataGen.Zip());

                await db.DropCollectionAsync("zip");

                await db.CheckpointAsync();

                // full disk usage
                var size = file.Size;

                var r = await db.RebuildAsync();

                // only header page
                Assert.Equal(8192, size - r);
            }
        }

        [Fact]
        public async Task Rebuild_Large_Files()
        {
            // do some tests
            async Task DoTestAsync(ILiteDatabaseAsync db, ILiteCollectionAsync<Zip> col)
            {
                Assert.Equal(1, await col.CountAsync());
                Assert.Equal(99, db.UserVersion);
            };

            using (var file = new TempFile())
            {
                using (var db = new LiteDatabaseAsync(file.Filename))
                {
                    var col = db.GetCollection<Zip>();

                    db.UserVersion = 99;

                    await col.EnsureIndexAsync("city", false);

                    var inserted = await col.InsertAsync(DataGen.Zip()); // 29.353 docs
                    var deleted = await col.DeleteManyAsync(x => x.Id != "01001"); // delete 29.352 docs

                    Assert.Equal(29353, inserted);
                    Assert.Equal(29352, deleted);

                    Assert.Equal(1, await col.CountAsync());

                    // must checkpoint
                    await db.CheckpointAsync();

                    // file still large than 5mb (even with only 1 document)
                    Assert.True(file.Size > 5 * 1024 * 1024);

                    // reduce datafile
                    var reduced = await db.RebuildAsync();

                    // now file are small than 50kb
                    Assert.True(file.Size < 50 * 1024);

                    await DoTestAsync(db, col);
                }

                // re-open and rebuild again
                using (var db = new LiteDatabaseAsync(file.Filename))
                {
                    var col = db.GetCollection<Zip>();

                    await DoTestAsync(db, col);

                    await db.RebuildAsync();

                    await DoTestAsync(db, col);
                }
            }
        }

        [Fact]
        public async Task Rebuild_Change_Culture_Error()
        {
            using (var file = new TempFile())
            using (var db = new LiteDatabaseAsync(file.Filename))
            {
                // remove string comparer ignore case
                await db.RebuildAsync(new RebuildOptions { Collation = new Collation("en-US/None") });

                // insert 2 documents with different ID in case sensitive
                await db.GetCollection("col1").InsertAsync(new BsonDocument[]
                {
                    new BsonDocument { ["_id"] = "ana" },
                    new BsonDocument { ["_id"] = "ANA" }
                });

                // try migrate to ignorecase
                this.Invoking(async x =>
                {
                    await db.RebuildAsync(new RebuildOptions { Collation = new Collation("en-US/IgnoreCase") });

                }).Should().Throw<LiteAsyncException>();

                // test if current pragma still with collation none
                (await db.PragmaAsync(Pragmas.COLLATION)).AsString.Should().Be("en-US/None");
            }
        }
    }
}

