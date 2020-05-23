using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Xunit;
using System.Threading.Tasks;
using LiteDB.Async;

namespace LiteDB.Async.Test
{
    public class MissingIdDocTest
    {
        #region Model

        public class MissingIdDoc
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }

        #endregion

        [Fact]
        public async Task MissingIdDoc_Test()
        {
            using (var file = new TempFile())
            using (var db = new LiteDatabaseAsync(file.Filename))
            {
                var col = db.GetCollection<MissingIdDoc>("col");

                var p = new MissingIdDoc { Name = "John", Age = 39 };

                // ObjectID will be generated 
                var id = await col.InsertAsync(p);

                p.Age = 41;

                await col.UpdateAsync(id, p);

                var r = await col.FindByIdAsync(id);

                r.Name.Should().Be(p.Name);
            }
        }
    }
}