using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LiteDB.Engine;
using Xunit;
using LiteDB.Async;
using LiteDB;

namespace Tests.LiteDB.Async
{
    public class Transactions_Tests
    {
        [Fact]
        public async Task Transactions_Allowed_On_Memmory_Streams()
        {
            using (var asyncDb = new LiteDatabaseAsync(new MemoryStream()))
            {
                var transDb = await asyncDb.BeginTransactionAsync();
            }
        }

        [Fact]
        public async Task Transactions__Allowed_On_Direct_Mode_Files()
        {
            var connectionString = new ConnectionString()
            {
                Filename = Path.Combine(Path.GetTempPath(), "litedbn-async-testing-" + Path.GetRandomFileName() + ".db"),
                Connection = ConnectionType.Direct,
                Password = "hunter2"
            };

            using (var asyncDb = new LiteDatabaseAsync(connectionString))
            {
                var transDb = await asyncDb.BeginTransactionAsync();
            }
        }

        [Fact]
        public async Task Transactions_Allowed_On_Shared_Mode_Files()
        {
            var connectionString = new ConnectionString()
            {
                Filename = Path.Combine(Path.GetTempPath(), "litedbn-async-testing-" + Path.GetRandomFileName() + ".db"),
                Connection = ConnectionType.Shared,
                Password = "hunter2"
            };

            using (var asyncDb = new LiteDatabaseAsync(connectionString))
            {
                // Verify this function just does not throw an exception
                using (var transDb = await asyncDb.BeginTransactionAsync())
                {
                    // We need to dispose of transDb before of it's parent othersise strange errors occur
                }
            }
        }

        [Fact]
        public async Task Transaction_Write_Lock_Timeout()
        {
            var data1 = DataGen.Person(1, 100).ToArray();

            var connectionString = new ConnectionString()
            {
                Filename = Path.Combine(Path.GetTempPath(), "litedbn-async-testing-" + Path.GetRandomFileName() + ".db"),
                Connection = ConnectionType.Shared,
                Password = "hunter2"
            };

            using var asyncDb1 = new LiteDatabaseAsync(connectionString);
            using var asyncDb2 = await asyncDb1.BeginTransactionAsync();
            using var asyncDb3 = await asyncDb1.BeginTransactionAsync();

            // small timeout
            await asyncDb1.PragmaAsync(Pragmas.TIMEOUT, 1);
            await asyncDb2.PragmaAsync(Pragmas.TIMEOUT, 1);
            await asyncDb3.PragmaAsync(Pragmas.TIMEOUT, 1);

            var asyncPerson2 = asyncDb2.GetCollection<Person>();
            // Add 100 more records
            await asyncPerson2.InsertAsync(data1);

            // Adding more records while running should timeout
            var asyncPerson3 = asyncDb2.GetCollection<Person>();
            var exception = await Assert.ThrowsAsync<LiteAsyncException>(async () =>
            {
                // Add 100 more records
                await asyncPerson3.InsertAsync(data1);

            });
            Assert.StartsWith("LiteDb encounter an error.", exception.Message);
        }

        [Fact]
        public async Task Transaction_Avoid_Dirty_Read_Rollback()
        {
            var data1 = DataGen.Person(1, 100).ToArray();
            var data2 = DataGen.Person(101, 200).ToArray();

            var connectionString = new ConnectionString()
            {
                Filename = Path.Combine(Path.GetTempPath(), "litedbn-async-testing-" + Path.GetRandomFileName() + ".db"),
                Connection = ConnectionType.Shared,
                Password = "hunter2"
            };

            using var asyncDb = new LiteDatabaseAsync(connectionString);
            using var asyncDb2 = await asyncDb.BeginTransactionAsync();

            var asyncPerson1 = asyncDb.GetCollection<Person>();
            // init person collection with 100 document
            await asyncPerson1.InsertAsync(data1);

            var asyncPerson2 = asyncDb2.GetCollection<Person>();

            // Add 100 more records
            await asyncPerson2.InsertAsync(data2);

            // Attempt to read from the first connection should be nothing there because we are inside a transaction
            Assert.Equal(100, await asyncPerson1.CountAsync());

            await asyncDb2.RollbackAsync();

            // Attempt to read from the second connection again should be 100 records
            asyncPerson1 = asyncDb.GetCollection<Person>();
            Assert.Equal(100, await asyncPerson1.CountAsync());
        }
        
        [Fact]
        public async Task Transaction_Avoid_Dirty_Read()
        {
            var data1 = DataGen.Person(1, 100).ToArray();
            var data2 = DataGen.Person(101, 200).ToArray();

            var connectionString = new ConnectionString()
            {
                Filename = Path.Combine(Path.GetTempPath(), "litedbn-async-testing-" + Path.GetRandomFileName() + ".db"),
                Connection = ConnectionType.Shared,
                Password = "hunter2"
            };

            using var asyncDb = new LiteDatabaseAsync(connectionString);
            using var asyncDb2 = await asyncDb.BeginTransactionAsync();

            var asyncPerson1 = asyncDb.GetCollection<Person>();
            // init person collection with 100 document
            await asyncPerson1.InsertAsync(data1);

            var asyncPerson2 = asyncDb2.GetCollection<Person>();

            // Add 100 more records
            await asyncPerson2.InsertAsync(data2);

            // Attempt to read from the second connection should be nothing there
            Assert.Equal(100, await asyncPerson1.CountAsync());

            await asyncDb2.CommitAsync();

            // Attempt to read from the second connection again should be 100 records
            asyncPerson1 = asyncDb.GetCollection<Person>();
            Assert.Equal(200, await asyncPerson1.CountAsync());
        }

        [Fact]
        public async Task Transaction_Read_Version()
        {
            var data1 = DataGen.Person(1, 100).ToArray();
            var data2 = DataGen.Person(101, 200).ToArray();

            using var asyncDb1 = new LiteDatabaseAsync(new MemoryStream());

            var asyncPerson1 = asyncDb1.GetCollection<Person>();
            // init person collection with 100 document
            await asyncPerson1.InsertAsync(data1);

            //Begin some transactions
            using var asyncDb2 = await asyncDb1.BeginTransactionAsync();
            using var asyncDb3 = await asyncDb1.BeginTransactionAsync();

            var asyncPerson2 = asyncDb2.GetCollection<Person>();
            var asyncPerson3 = asyncDb3.GetCollection<Person>();

            // We should see 100 record inserted before the transactions started
            Assert.Equal(100, await asyncPerson2.CountAsync());
            Assert.Equal(100, await asyncPerson3.CountAsync());

            // Add 100 more records
            await asyncPerson2.InsertAsync(data2);

            // Attempt to read from the second connection should be nothing there
            Assert.Equal(200, await asyncPerson2.CountAsync());

            Assert.Equal(100, await asyncPerson3.CountAsync());

            await asyncDb2.CommitAsync();

            // Attempt to read from the second connection again should be 100 records. We shouldn't see the second lot of records because we are already in a transaction
            Assert.Equal(100, await asyncPerson3.CountAsync());
        }

        [Fact]
        public async Task Test_Transaction_States_Rollback_Closes_Transaction()
        {
            var data1 = DataGen.Person(1, 100).ToArray();
            var data2 = DataGen.Person(101, 200).ToArray();

            var connectionString = new ConnectionString()
            {
                Filename = Path.Combine(Path.GetTempPath(), "litedbn-async-testing-" + Path.GetRandomFileName() + ".db"),
                Connection = ConnectionType.Shared,
                Password = "hunter2"
            };

            using var asyncDb1 = new LiteDatabaseAsync(connectionString);
            using var asyncDb2 = await asyncDb1.BeginTransactionAsync();

            var asyncPerson2 = asyncDb2.GetCollection<Person>();
            await asyncPerson2.InsertAsync(data1);

            await asyncDb2.RollbackAsync();

            var exception = await Assert.ThrowsAsync<LiteAsyncException>(async () =>
            {
                await asyncPerson2.InsertAsync(data1);
            });
            Assert.Equal("Transaction Closed, no further writes are allowed.", exception.Message);
        }

        [Fact]
        public async Task Test_Transaction_States_Commit_Closes_Transaction()
        {
            var data1 = DataGen.Person(1, 100).ToArray();
            var data2 = DataGen.Person(101, 200).ToArray();

            var connectionString = new ConnectionString()
            {
                Filename = Path.Combine(Path.GetTempPath(), "litedbn-async-testing-" + Path.GetRandomFileName() + ".db"),
                Connection = ConnectionType.Shared,
                Password = "hunter2"
            };

            using var asyncDb1 = new LiteDatabaseAsync(connectionString);
            using var asyncDb2 = await asyncDb1.BeginTransactionAsync();

            var asyncPerson2 = asyncDb2.GetCollection<Person>();
            await asyncPerson2.InsertAsync(data1);

            await asyncDb2.CommitAsync();

            var exception = await Assert.ThrowsAsync<LiteAsyncException>(async () =>
            {
                await asyncPerson2.InsertAsync(data1);
            });
            Assert.Equal("Transaction Closed, no further writes are allowed.", exception.Message);
        }

        [Fact]
        public async Task Transactions_Out_Of_Order_Disposes_Are_OK()
        {
            var connectionString = new ConnectionString()
            {
                Filename = Path.Combine(Path.GetTempPath(), "litedbn-async-testing-" + Path.GetRandomFileName() + ".db"),
                Connection = ConnectionType.Shared,
                Password = "hunter2"
            };

            var asyncDb = new LiteDatabaseAsync(connectionString);
            var transDb = await asyncDb.BeginTransactionAsync();

            // Dispose the parent first
            asyncDb.Dispose();
            transDb.Dispose();
        }
    }
}