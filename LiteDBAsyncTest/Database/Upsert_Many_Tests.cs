using LiteDB.Async;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Tests.LiteDB.Async
{
    #region Model 

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    #endregion

    public class Upsert_Many_Tests
    {
        [Fact]
        public async Task UpsertManyAsync()
        {
            using (var db = new LiteDatabaseAsync(new MemoryStream()))
            {
                var manyUsers = Enumerable.Range(1, 100).Select(i => new User() { Id = i, Name = $"Example {i}" }).ToList();

                var users = db.GetCollection<User>("users");

                Assert.Equal(100, await users.UpsertAsync(manyUsers));

                Assert.Equal(100, await users.CountAsync());

                // Read back from the database
                var users2 = await db.GetCollection<User>("users").Query().ToListAsync();

                // Are the id values the expected ones
                Assert.True(Enumerable.Range(1, 100).ToHashSet().SetEquals(users2.Select(u => u.Id)));

                // Are the names correct?
                foreach(var user in users2)
                {
                    Assert.Equal($"Example {user.Id}", user.Name);
                }

                // Update with new names
                manyUsers = Enumerable.Range(1, 100).Select(i => new User() { Id = i, Name = $"User {i}" }).ToList();

                await users.UpsertAsync(manyUsers);

                Assert.Equal(100, await users.CountAsync());

                // Read back from the database
                users2 = await db.GetCollection<User>("users").Query().ToListAsync();

                // Are the id values the expected ones
                Assert.True(Enumerable.Range(1, 100).ToHashSet().SetEquals(users2.Select(u => u.Id)));

                // Are the names correct?
                foreach (var user in users2)
                {
                    Assert.Equal($"User {user.Id}", user.Name);
                }
            }
        }
    }
}

