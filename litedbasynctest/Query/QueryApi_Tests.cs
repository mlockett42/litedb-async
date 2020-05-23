using FluentAssertions;
using System;
using System.Linq;
using Xunit;
using LiteDB;
using System.Threading.Tasks;
using LiteDB.Async;

namespace LiteDB.Async.Test
{
    public class QueryApi_Tests : Person_Tests
    {
        [Fact]
        public async Task Query_And()
        {
            var r0 = local.Where(x => x.Age == 22 && x.Active == true).ToArray();

            var r1 = (await _collection.FindAsync(Query.And(Query.EQ("Age", 22), Query.EQ("Active", true)))).ToArray();

            AssertEx.ArrayEqual(r0, r1, true);
        }

        [Fact]
        public async Task Query_And_Same_Field()
        {
            var r0 = local.Where(x => x.Age > 22 && x.Age < 25).ToArray();

            var r1 = (await _collection.FindAsync(Query.And(Query.GT("Age", 22), Query.LT("Age", 25)))).ToArray();

            AssertEx.ArrayEqual(r0, r1, true);
        }
    }
}