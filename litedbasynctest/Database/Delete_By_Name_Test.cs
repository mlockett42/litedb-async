using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using Xunit;
using System.Threading.Tasks;
using LiteDB.Async;

namespace LiteDB.Async.Test
{
    public class Delete_By_Name_Tests
    {
        #region Model

        public class Person
        {
            public int Id { get; set; }
            public string Fullname { get; set; }
        }

        #endregion

        [Fact]
        public async Task Delete_By_Name()
        {
            using (var f = new TempFile())
            using (var db = new LiteDatabaseAsync(f.Filename))
            {
                var col = db.GetCollection<Person>("Person");

                await col.InsertAsync(new Person { Fullname = "John" });
                await col.InsertAsync(new Person { Fullname = "Doe" });
                await col.InsertAsync(new Person { Fullname = "Joana" });
                await col.InsertAsync(new Person { Fullname = "Marcus" });

                // lets auto-create index in FullName and delete from a non-pk node
                var del = await col.DeleteManyAsync(x => x.Fullname.StartsWith("J"));

                del.Should().Be(2);
            }
        }
    }
}