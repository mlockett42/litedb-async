using FluentAssertions;
using System.Linq;
using Xunit;
using System.Threading.Tasks;
using LiteDB.Async;

namespace LiteDB.Async.Test
{
    public class Where_Tests : Person_Tests
    {
        class Entity
        {
            public string Name { get; set; }
            public int Size { get; set; }
        }

        [Fact]
        public async Task Query_Where_With_Parameter()
        {
            var r0 = local
                .Where(x => x.Address.State == "FL")
                .ToArray();

            var r1 = await _collection.Query()
                .Where(x => x.Address.State == "FL")
                .ToArrayAsync();

            AssertEx.ArrayEqual(r0, r1, true);
        }

        [Fact]
        public async Task Query_Multi_Where_With_Like()
        {
            var r0 = local
                .Where(x => x.Age >= 10 && x.Age <= 40)
                .Where(x => x.Name.StartsWith("Ge"))
                .ToArray();

            var r1 = await _collection.Query()
                .Where(x => x.Age >= 10 && x.Age <= 40)
                .Where(x => x.Name.StartsWith("Ge"))
                .ToArrayAsync();

            AssertEx.ArrayEqual(r0, r1, true);
        }

        [Fact]
        public async Task Query_Single_Where_With_And()
        {
            var r0 = local
                .Where(x => x.Age == 25 && x.Active)
                .ToArray();

            var r1 = await _collection.Query()
                .Where("age = 25 AND active = true")
                .ToArrayAsync();

            AssertEx.ArrayEqual(r0, r1, true);
        }

        [Fact]
        public async Task Query_Single_Where_With_Or_And_In()
        {
            var r0 = local
                .Where(x => x.Age == 25 || x.Age == 26 || x.Age == 27)
                .ToArray();

            var r1 = await _collection.Query()
                .Where("age = 25 OR age = 26 OR age = 27")
                .ToArrayAsync();

            var r2 = await _collection.Query()
                .Where("age IN [25, 26, 27]")
                .ToArrayAsync();

            AssertEx.ArrayEqual(r0, r1, true);
            AssertEx.ArrayEqual(r1, r2, true);
        }

        [Fact]
        public async Task Query_With_Array_Ids()
        {
            var ids = new int[] { 1, 2, 3 };

            var r0 = local
                .Where(x => ids.Contains(x.Id))
                .ToArray();

            var r1 = await _collection.Query()
                .Where(x => ids.Contains(x.Id))
                .ToArrayAsync();

            AssertEx.ArrayEqual(r0, r1, true);
        }
    }
}