using System;
using System.IO;
using System.Linq;
using LiteDB;
using FluentAssertions;
using Xunit;
using System.Threading.Tasks;
using LiteDB.Async;

namespace LiteDB.Async.Test
{
    public class DeleteMany_Tests
    {
        [Fact]
        public async Task DeleteMany_With_Arguments()
        {
            using (var db = new LiteDatabaseAsync(":memory:"))
            {
                var c1 = db.GetCollection("Test");

                var d1 = new BsonDocument() { ["_id"] = 1, ["p1"] = 1 };
                await c1.InsertAsync(d1);

                (await c1.CountAsync()).Should().Be(1);

                // try BsonExpression predicate with argument - not deleted
                var e1 = BsonExpression.Create("$._id = @0", 1);
                var r1 = await c1.DeleteManyAsync(e1);

                r1.Should().Be(1);

                // the same BsonExpression predicate works fine in FindOne
                var r = await c1.FindOneAsync(e1);

            }
        }
    }
}