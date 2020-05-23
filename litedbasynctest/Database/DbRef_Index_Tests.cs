using System.IO;
using LiteDB;
using FluentAssertions;
using Xunit;
using System.Threading.Tasks;
using LiteDB.Async;

namespace LiteDB.Async.Test
{
    public class DbRef_Index_Tests
    {
        #region Model

        public class Customer
        {
            public string Login { get; set; }
            public string Name { get; set; }
        }

        public class Order
        {
            public int OrderNumber { get; set; }
            public Customer Customer { get; set; }
        }

        #endregion

        [Fact]
        public async Task DbRef_Index()
        {
            var mapper = new BsonMapper();

            mapper.Entity<Customer>()
                .Id(x => x.Login)
                .Field(x => x.Name, "customer_name");

            mapper.Entity<Order>()
                .Id(x => x.OrderNumber)
                .Field(x => x.Customer, "cust")
                .DbRef(x => x.Customer, "customers");

            using (var db = new LiteDatabaseAsync(new MemoryStream(), mapper))
            {
                var customer = new Customer { Login = "jd", Name = "John Doe" };
                var order = new Order { Customer = customer };

                var customers = db.GetCollection<Customer>("Customers");
                var orders = db.GetCollection<Order>("Orders");

                await customers.InsertAsync(customer);
                await orders.InsertAsync(order);

                // create an index in Customer.Id ref
                // x.Customer.Login == "Customer.$id"
                await orders.EnsureIndexAsync(x => x.Customer.Login);

                var query = await orders
                    .Include(x => x.Customer)
                    .FindOneAsync(x => x.Customer.Login == "jd");

                query.Customer.Name.Should().Be(customer.Name);
            }
        }
    }
}