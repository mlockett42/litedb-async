using System.Linq;
using FluentAssertions;
using Xunit;
using System.Threading.Tasks;
using LiteDB.Async;
using LiteDB;

namespace LiteDB.Async.Test
{
    public class OrderBy_Tests : Person_Tests
    {
        [Fact]
        public async Task Query_OrderBy_Using_Index()
        {
            await _collection.EnsureIndexAsync(x => x.Name);

            var r0 = local
                .OrderBy(x => x.Name)
                .Select(x => new {x.Name})
                .ToArray();

            var r1 = await _collection.Query()
                .OrderBy(x => x.Name)
                .Select(x => new {x.Name})
                .ToArrayAsync();

            r0.Should().Equal(r1);
        }

        [Fact]
        public async Task Query_OrderBy_Using_Index_Desc()
        {
            await _collection.EnsureIndexAsync(x => x.Name);

            var r0 = local
                .OrderByDescending(x => x.Name)
                .Select(x => new {x.Name})
                .ToArray();

            var r1 = await _collection.Query()
                .OrderByDescending(x => x.Name)
                .Select(x => new {x.Name})
                .ToArrayAsync();

            r0.Should().Equal(r1);
        }

        [Fact]
        public async Task Query_OrderBy_With_Func()
        {
            await _collection.EnsureIndexAsync(x => x.Date.Day);

            var r0 = local
                .OrderBy(x => x.Date.Day)
                .Select(x => new {d = x.Date.Day})
                .ToArray();

            var r1 = await _collection.Query()
                .OrderBy(x => x.Date.Day)
                .Select(x => new {d = x.Date.Day})
                .ToArrayAsync();

            r0.Should().Equal(r1);
        }

        [Fact]
        public async Task Query_OrderBy_With_Offset_Limit()
        {
            // no index

            var r0 = local
                .OrderBy(x => x.Date.Day)
                .Select(x => new { d = x.Date.Day })
                .Skip(5)
                .Take(10)
                .ToArray();

            var r1 = await _collection.Query()
                .OrderBy(x => x.Date.Day)
                .Select(x => new { d = x.Date.Day })
                .Offset(5)
                .Limit(10)
                .ToArrayAsync();

            r0.Should().Equal(r1);
        }

        [Fact]
        public async Task Query_Asc_Desc()
        {
            var asc = (await _collection.FindAsync(Query.All(Query.Ascending))).ToArray();
            var desc = (await _collection.FindAsync(Query.All(Query.Descending))).ToArray();

            asc[0].Id.Should().Be(1);
            desc[0].Id.Should().Be(1000);

        }
    }
}