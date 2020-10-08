using LiteDB;
using LiteDB.Async;
using System.Threading.Tasks;
using Xunit;

namespace Tests.LiteDB.Async
{
    public class Snapshot_Upgrade_Tests
    {
        [Fact]
        public async Task Transaction_Update_Upsert()
        {
            using var db = new LiteDatabaseAsync(":memory:");
            var col = db.GetCollection("test");

            // Transactions not supported ATM by Async
            bool transactionCreated = await db.BeginTransAsync();
            Assert.True(transactionCreated);

            int updatedDocs = await  col.UpdateManyAsync("{name: \"xxx\"}", BsonExpression.Create("_id = 1"));
            Assert.Equal(0, updatedDocs);

            await col.UpsertAsync(new BsonDocument() { ["_id"] = 1, ["name"] = "xxx" });
            var result = await col.FindByIdAsync(1);
            Assert.NotNull(result);
        }
    }
}
