using System;
using System.IO;
using System.Threading.Tasks;
using LiteDB;
using LiteDB.Async;
using Xunit;

namespace Tests.LiteDB.Async
{
    public class ConnectionStringObjectTest : IDisposable
    {
        private readonly ConnectionString _connectionString;
        private readonly LiteDatabaseAsync _db;
        public ConnectionStringObjectTest()
        {
            _connectionString = new ConnectionString()
            {
                Filename = Path.Combine(Path.GetTempPath(), "litedbn-async-testing-" + Path.GetRandomFileName() + ".db"),
                Connection = ConnectionType.Shared,
                Password = "hunter2"
            };
            _db = new LiteDatabaseAsync(_connectionString);
        }

        [Fact]
        public async Task TestCanUpsertAndGetList()
        {
            var collection = _db.GetCollection<SimplePerson>();

            var person = new SimplePerson()
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Smith"
            };

            var upsertResult = await collection.UpsertAsync(person);
            Assert.True(upsertResult);

            var listResult = await collection.Query().ToListAsync();
            Assert.Single(listResult);
            var resultPerson = listResult[0];
            Assert.Equal(person.Id, resultPerson.Id);
            Assert.Equal(person.FirstName, resultPerson.FirstName);
            Assert.Equal(person.LastName, resultPerson.LastName);
        }
 
 
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {      
                _db.Dispose();
                File.Delete(_connectionString.Filename);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}