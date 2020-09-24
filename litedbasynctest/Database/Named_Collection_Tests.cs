using LiteDB.Async;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Tests.LiteDB.Async
{
    public class Named_Collection_Tests
    {
        #region Model 

        public class User
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        #endregion

        [Fact]
        public async Task Insert_With_AutoId_And_Reread()
        {
            using (var db = new LiteDatabaseAsync(new MemoryStream()))
            {
                var users = db.GetCollection<User>("TEST");

                var u1 = new User { Name = "John" };
                var u2 = new User { Name = "Zarlos" };
                var u3 = new User { Name = "Ana" };

                // insert ienumerable
                await users.InsertAsync(new User[] { u1, u2, u3 });

                // Reopen the collection
                var users2 = db.GetCollection<User>("TEST");
                var result = users2.FindAllAsync().GetAwaiter().GetResult();

                Assert.Equal(3, result.Count());
                var resultList = result.Select(u => u.Name).ToList();
                Assert.Contains("John", resultList);
                Assert.Contains("Zarlos", resultList);
                Assert.Contains("Ana", resultList);

            }
        }
    }
}
