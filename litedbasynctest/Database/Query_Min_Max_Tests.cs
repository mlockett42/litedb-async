using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using Xunit;
using System.Threading.Tasks;
using LiteDB.Async;

namespace LiteDB.Async.Test
{
    public class Query_Min_Max_Tests
    {
        #region Model

        public class EntityMinMax
        {
            public int Id { get; set; }
            public byte ByteValue { get; set; }
            public int IntValue { get; set; }
            public uint UintValue { get; set; }
            public long LongValue { get; set; }
        }

        #endregion

        [Fact]
        public async Task Query_Min_Max()
        {
            using (var f = new TempFile())
            using (var db = new LiteDatabaseAsync(f.Filename))
            {
                var c = db.GetCollection<EntityMinMax>("col");

                await c.InsertAsync(new EntityMinMax { });
                await c.InsertAsync(new EntityMinMax
                {
                    ByteValue = 200,
                    IntValue = 443500,
                    LongValue = 443500,
                    UintValue = 443500
                });

                await c.EnsureIndexAsync(x => x.ByteValue);
                await c.EnsureIndexAsync(x => x.IntValue);
                await c.EnsureIndexAsync(x => x.LongValue);
                await c.EnsureIndexAsync(x => x.UintValue);

                (await c.MaxAsync(x => x.ByteValue)).Should().Be(200);
                (await c.MaxAsync(x => x.IntValue)).Should().Be(443500);
                (await c.MaxAsync(x => x.LongValue)).Should().Be(443500);
                (await c.MaxAsync(x => x.UintValue)).Should().Be(443500);

            }
        }
    }
}