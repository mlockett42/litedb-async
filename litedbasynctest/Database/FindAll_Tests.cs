using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using Xunit;
using System.Threading.Tasks;
using LiteDB.Async;
using LiteDB;

namespace LiteDB.Async.Test
{
    public class FindAll_Tests
    {
        #region Model

        public class Person
        {
            public int Id { get; set; }
            public string Fullname { get; set; }
        }

        #endregion

        [Fact]
        public async Task FindAll()
        {
            using (var f = new TempFile())
            {
                using (var db = new LiteDatabaseAsync(f.Filename))
                {
                    var col = db.GetCollection<Person>("Person");

                    await col.InsertAsync(new Person { Fullname = "John" });
                    await col.InsertAsync(new Person { Fullname = "Doe" });
                    await col.InsertAsync(new Person { Fullname = "Joana" });
                    await col.InsertAsync(new Person { Fullname = "Marcus" });
                }
                // close datafile

                using (var db = new LiteDatabaseAsync(f.Filename))
                {
                    var p = await db.GetCollection<Person>("Person").FindAsync(Query.All("Fullname", Query.Ascending));

                    p.Count().Should().Be(4);
                }
            }

        }
    }
}