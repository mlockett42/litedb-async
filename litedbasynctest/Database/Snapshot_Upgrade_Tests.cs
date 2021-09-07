using LiteDB;
using LiteDB.Async;
using System.Threading.Tasks;
using Xunit;

namespace Tests.LiteDB.Async
{
    public class Snapshot_Upgrade_Tests
    {
        [Fact(Skip = "Temporaily skip never exits")]
        public async Task Transaction_Update_Upsert()
        {
            using var db = new LiteDatabaseAsync(":memory:");

            // Transactions not supported ATM by Async
            var db2 = await db.BeginTransactionAsync();
            var col = db.GetCollection("test");

            int updatedDocs = await  col.UpdateManyAsync("{name: \"xxx\"}", BsonExpression.Create("_id = 1"));
            Assert.Equal(0, updatedDocs);

            await col.UpsertAsync(new BsonDocument() { ["_id"] = 1, ["name"] = "xxx" });
            var result = await col.FindByIdAsync(1);
            Assert.NotNull(result);
        }
    }
}
