using System;
using Xunit;
using LiteDB.Async;
using System.IO;
using System.Threading.Tasks;

namespace LiteDB.Async.Test
{
    public class ConnectionStringTest : IDisposable
    {
        private readonly string _databasePath;
        private readonly LiteDatabaseAsync _db;
        public ConnectionStringTest()
        {
            _databasePath = Path.Combine(Path.GetTempPath(), "litedbn-async-testing-" + Path.GetRandomFileName() + ".db");
            _db = new LiteDatabaseAsync("Filename=" + _databasePath + ";Connection=shared;Password=hunter2");
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
                File.Delete(_databasePath);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
   }
}