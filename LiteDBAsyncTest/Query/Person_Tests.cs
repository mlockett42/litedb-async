using System;
using System.Linq;
using LiteDB;
using LiteDB.Async;

namespace Tests.LiteDB.Async
{
    public class Person_Tests : IDisposable
    {
        protected readonly Person[] local;

        protected LiteDatabaseAsync _db;
        protected ILiteCollectionAsync<Person> _collection;

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