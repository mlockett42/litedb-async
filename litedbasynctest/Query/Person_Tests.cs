using System;
using System.Linq;
using LiteDB.Async;

namespace LiteDB.Async.Test
{
    public class Person_Tests : IDisposable
    {
        protected readonly Person[] local;

        protected LiteDatabaseAsync _db;
        protected LiteCollectionAsync<Person> _collection;

        public Person_Tests()
        {
            this.local = DataGen.Person().ToArray();

            _db = new LiteDatabaseAsync(":memory:");
            _collection = _db.GetCollection<Person>("person");
            var task = _collection.InsertAsync(this.local);
            task.Wait();
        }

        public void Dispose()
        {
            _db?.Dispose();
        }
    }
}