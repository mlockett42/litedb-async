using System.Linq;
using FluentAssertions;
using Xunit;
using System.Threading.Tasks;
using LiteDB.Async;

namespace LiteDB.Async.Test
{
    public class Select_Tests : Person_Tests
    {
        [Fact]
        public async Task Query_Select_Key_Only()
        {
            await _collection.EnsureIndexAsync(x => x.Address.City);

            // must orderBy mem data because index will be sorted
            var r0 = local
                .OrderBy(x => x.Address.City)
                .Select(x => x.Address.City)
                .ToArray();

            // this query will not deserialize document, using only index key
            var r1 = await _collection.Query()
                .OrderBy(x => x.Address.City)
                .Select(x => x.Address.City)
                .ToArrayAsync();

            r0.Should().Equal(r1);
        }

        [Fact]
        public async Task Query_Select_New_Document()
        {
            var r0 = local
                .Select(x => new {city = x.Address.City.ToUpper(), phone0 = x.Phones[0], address = new Address {Street = x.Name}})
                .ToArray();

            var r1 = await _collection.Query()
                .Select(x => new {city = x.Address.City.ToUpper(), phone0 = x.Phones[0], address = new Address {Street = x.Name}})
                .ToArrayAsync();

            foreach (var r in r0.Zip(r1, (l, r) => new {left = l, right = r}))
            {
                r.right.city.Should().Be(r.left.city);
                r.right.phone0.Should().Be(r.left.phone0);
                r.right.address.Street.Should().Be(r.left.address.Street);
            }
        }
    }
}