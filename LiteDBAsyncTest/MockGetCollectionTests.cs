using LiteDB;
using LiteDB.Async;
using Moq;
using System.Threading.Tasks;
using Tests.LiteDB.Async;
using Xunit;

namespace Tests.LiteDB.Async
{
    public class MockGetCollectionTests
    {
        [Fact]
        public async Task TestWeCanReturnAMockedCollectionFromGetCollection()
        {
            var mockCollection = new Mock<ILiteCollectionAsync<SimplePerson>>();
            mockCollection.Setup(col => col.CountAsync()).Returns(Task.FromResult(12));
            var mockDatabaseAsync = new Mock<ILiteDatabaseAsync>();
            mockDatabaseAsync.Setup(d => d.GetCollection<SimplePerson>()).Returns(mockCollection.Object);

            var collection = mockDatabaseAsync.Object.GetCollection<SimplePerson>();
            Assert.Equal(12, await collection.CountAsync());
        }
    }
}
