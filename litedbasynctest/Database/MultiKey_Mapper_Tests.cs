using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Xunit;
using LiteDB;
using System.Threading.Tasks;
using LiteDB.Async;

namespace LiteDB.Async.Test
{
    public class MultiKey_Mapper_Tests
    {
        #region Model

        public class MultiKeyDoc
        {
            public int Id { get; set; }
            public int[] Keys { get; set; }
            public List<Customer> Customers { get; set; }
        }

        public class Customer
        {
            public string Login { get; set; }
            public string Name { get; set; }
        }

        #endregion

        [Fact]
        public async Task MultiKey_Mapper()
        {
            using (var db = new LiteDatabaseAsync(":memory:"))
            {
                var col = db.GetCollection<MultiKeyDoc>("col");

                await col.InsertAsync(new MultiKeyDoc
                {
                    Id = 1,
                    Keys = new int[] { 1, 2, 3 },
                    Customers = new List<Customer>()
                    {
                        new Customer { Name = "John" },
                        new Customer { Name = "Ana" },
                        new Customer { Name = "Doe" },
                        new Customer { Name = "Dante" }
                    }
                });

                await col.InsertAsync(new MultiKeyDoc
                {
                    Id = 2,
                    Keys = new int[] { 2 },
                    Customers = new List<Customer>()
                    {
                        new Customer { Name = "Ana" }
                    }
                });

                await col.EnsureIndexAsync(x => x.Keys);
                await col.EnsureIndexAsync(x => x.Customers.Select(z => z.Name));

                // Query.EQ("Keys", 2)
                (await col.CountAsync(Query.EQ("Keys", 2))).Should().Be(2);
                (await col.CountAsync(x => x.Keys.Contains(2))).Should().Be(2);

                (await col.CountAsync(Query.StartsWith("Customers[*].Name ANY", "Ana"))).Should().Be(2);
                (await col.CountAsync(x => x.Customers.Select(z => z.Name).Any(z => z.StartsWith("Ana")))).Should().Be(2);

                (await col.CountAsync(Query.StartsWith("Customers[*].Name ANY", "D"))).Should().Be(1);
                (await col.CountAsync(x => x.Customers.Select(z => z.Name).Any(z => z.StartsWith("D")))).Should().Be(1);
            }
        }
    }
}