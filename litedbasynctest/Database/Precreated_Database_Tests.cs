using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Xunit;
using System.Threading.Tasks;
using LiteDB.Async;
using LiteDB;
using Moq;

namespace Tests.LiteDB.Async
{
    public class Precreated_Database_Tests
    {
        #region Model 

        public class User
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        #endregion

        [Fact]
        public async Task Insert_With_AutoId()
        {
            using (var wrappedDb = new LiteDatabase(new MemoryStream()) )
            using (var db = new LiteDatabaseAsync(wrappedDb))
            {
                var users = db.GetCollection<User>("users");

                var u1 = new User { Name = "John" };
                var u2 = new User { Name = "Zarlos" };
                var u3 = new User { Name = "Ana" };

                // insert ienumerable
                await users.InsertAsync(new User[] { u1, u2 });

                await users.InsertAsync(u3);

                // test auto-id
                u1.Id.Should().Be(1);
                u2.Id.Should().Be(2);
                u3.Id.Should().Be(3);

                // adding without autoId
                var u4 = new User { Id = 20, Name = "Marco" };

                await users.InsertAsync(u4);

                // adding more auto id after fixed id
                var u5 = new User { Name = "Julio" };

                await users.InsertAsync(u5);

                u5.Id.Should().Be(21);
            }
        }

        [Fact]
        public void Disposes_Wrapped_LiteDb()
        {
            var mockWrappedDb = new Mock<ILiteDatabase>();
            using (var db = new LiteDatabaseAsync(mockWrappedDb.Object))
            {
                // Do nothing
            }
            mockWrappedDb.Verify(x => x.Dispose(), Times.Once);
        }

        [Fact]
        public void Dont_Dispose_Wrapped_LiteDb()
        // Don't dispose of the wrapped LiteDB instance if the correct parameter is passed in
        {
            var mockWrappedDb = new Mock<ILiteDatabase>();
            using (var db = new LiteDatabaseAsync(mockWrappedDb.Object, false))
            {
                // Do nothing
            }
            mockWrappedDb.Verify(x => x.Dispose(), Times.Never);
        }

        [Fact]
        public void Only_One_LiteDbAsync_Per_LiteDb()
        {
            using (var wrappedDb = new LiteDatabase(new MemoryStream()) )
            using (var db1 = new LiteDatabaseAsync(wrappedDb)) {
                var exception = Assert.Throws<LiteAsyncException>(() => { var db1 = new LiteDatabaseAsync(wrappedDb); });
                Assert.Equal("You can only have one LiteDatabaseAsync per LiteDatabase.", exception.Message);
            }
        }
    }
}
