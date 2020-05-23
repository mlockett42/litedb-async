using System;
using Xunit;
using LiteDB.Async;
using System.IO;
using System.Threading.Tasks;
using LiteDB;
using FluentAssertions;

namespace LiteDB.Async.Test
{
    public class AutoIdTest
    {

        #region Model

        public class EntityInt
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class EntityLong
        {
            public long Id { get; set; }
            public string Name { get; set; }
        }

        public class EntityGuid
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        public class EntityOid
        {
            public ObjectId Id { get; set; }
            public string Name { get; set; }
        }

        public class EntityString
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }

        #endregion

        [Fact]
        public async Task AutoId_Strong_Typed()
        {
            var mapper = new BsonMapper();

            using (var db = new LiteDatabaseAsync(new MemoryStream(), mapper))
            {
                var cs_int = db.GetCollection<EntityInt>("int");
                var cs_long = db.GetCollection<EntityLong>("long");
                var cs_guid = db.GetCollection<EntityGuid>("guid");
                var cs_oid = db.GetCollection<EntityOid>("oid");
                var cs_str = db.GetCollection<EntityString>("str");

                // int32
                var cint_1 = new EntityInt() { Name = "R1" };
                var cint_2 = new EntityInt() { Name = "R2" };
                var cint_3 = new EntityInt() { Name = "R3" };
                var cint_4 = new EntityInt() { Name = "R4" };

                // long
                var clong_1 = new EntityLong() { Name = "R1" };
                var clong_2 = new EntityLong() { Name = "R2" };
                var clong_3 = new EntityLong() { Name = "R3" };
                var clong_4 = new EntityLong() { Name = "R4" };

                // guid
                var cguid_1 = new EntityGuid() { Name = "R1" };
                var cguid_2 = new EntityGuid() { Name = "R2" };
                var cguid_3 = new EntityGuid() { Name = "R3" };
                var cguid_4 = new EntityGuid() { Name = "R4" };

                // oid
                var coid_1 = new EntityOid() { Name = "R1" };
                var coid_2 = new EntityOid() { Name = "R2" };
                var coid_3 = new EntityOid() { Name = "R3" };
                var coid_4 = new EntityOid() { Name = "R4" };

                // string - there is no AutoId for string
                var cstr_1 = new EntityString() { Id = "a", Name = "R1" };
                var cstr_2 = new EntityString() { Id = "b", Name = "R2" };
                var cstr_3 = new EntityString() { Id = "c", Name = "R3" };
                var cstr_4 = new EntityString() { Id = "d", Name = "R4" };

                // insert first 3 documents
                await cs_int.InsertAsync(new[] { cint_1, cint_2, cint_3 });
                await cs_long.InsertAsync(new[] { clong_1, clong_2, clong_3 });
                await cs_guid.InsertAsync(new[] { cguid_1, cguid_2, cguid_3 });
                await cs_oid.InsertAsync(new[] { coid_1, coid_2, coid_3 });
                await cs_str.InsertAsync(new[] { cstr_1, cstr_2, cstr_3 });

                // change document 2
                cint_2.Name = "Changed 2";
                clong_2.Name = "Changed 2";
                cguid_2.Name = "Changed 2";
                coid_2.Name = "Changed 2";
                cstr_2.Name = "Changed 2";

                // update document 2
                var nu_int = await cs_int.UpdateAsync(cint_2);
                var nu_long = await cs_long.UpdateAsync(clong_2);
                var nu_guid = await cs_guid.UpdateAsync(cguid_2);
                var nu_oid = await cs_oid.UpdateAsync(coid_2);
                var nu_str = await cs_str.UpdateAsync(cstr_2);

                nu_int.Should().BeTrue();
                nu_long.Should().BeTrue();
                nu_guid.Should().BeTrue();
                nu_oid.Should().BeTrue();
                nu_str.Should().BeTrue();

                // change document 3
                cint_3.Name = "Changed 3";
                clong_3.Name = "Changed 3";
                cguid_3.Name = "Changed 3";
                coid_3.Name = "Changed 3";
                cstr_3.Name = "Changed 3";

                // upsert (update) document 3
                var fu_int = await cs_int.UpsertAsync(cint_3);
                var fu_long = await cs_long.UpsertAsync(clong_3);
                var fu_guid = await cs_guid.UpsertAsync(cguid_3);
                var fu_oid = await cs_oid.UpsertAsync(coid_3);
                var fu_str = await cs_str.UpsertAsync(cstr_3);

                fu_int.Should().BeFalse();
                fu_long.Should().BeFalse();
                fu_guid.Should().BeFalse();
                fu_oid.Should().BeFalse();
                fu_str.Should().BeFalse();

                // test if was changed
                (await cs_int.FindOneAsync(x => x.Id == cint_3.Id)).Name.Should().Be(cint_3.Name);
                (await cs_long.FindOneAsync(x => x.Id == clong_3.Id)).Name.Should().Be(clong_3.Name);
                (await cs_guid.FindOneAsync(x => x.Id == cguid_3.Id)).Name.Should().Be(cguid_3.Name);
                (await cs_oid.FindOneAsync(x => x.Id == coid_3.Id)).Name.Should().Be(coid_3.Name);
                (await cs_str.FindOneAsync(x => x.Id == cstr_3.Id)).Name.Should().Be(cstr_3.Name);

                // upsert (insert) document 4
                var tu_int = await cs_int.UpsertAsync(cint_4);
                var tu_long = await cs_long.UpsertAsync(clong_4);
                var tu_guid = await cs_guid.UpsertAsync(cguid_4);
                var tu_oid = await cs_oid.UpsertAsync(coid_4);
                var tu_str = await cs_str.UpsertAsync(cstr_4);

                tu_int.Should().BeTrue();
                tu_long.Should().BeTrue();
                tu_guid.Should().BeTrue();
                tu_oid.Should().BeTrue();
                tu_str.Should().BeTrue();

                // test if was included
                (await cs_int.FindOneAsync(x => x.Id == cint_4.Id)).Name.Should().Be(cint_4.Name);
                (await cs_long.FindOneAsync(x => x.Id == clong_4.Id)).Name.Should().Be(clong_4.Name);
                (await cs_guid.FindOneAsync(x => x.Id == cguid_4.Id)).Name.Should().Be(cguid_4.Name);
                (await cs_oid.FindOneAsync(x => x.Id == coid_4.Id)).Name.Should().Be(coid_4.Name);
                (await cs_str.FindOneAsync(x => x.Id == cstr_4.Id)).Name.Should().Be(cstr_4.Name);

                // count must be 4
                (await cs_int.CountAsync(Query.All())).Should().Be(4);
                (await cs_long.CountAsync(Query.All())).Should().Be(4);
                (await cs_guid.CountAsync(Query.All())).Should().Be(4);
                (await cs_oid.CountAsync(Query.All())).Should().Be(4);
                (await cs_str.CountAsync(Query.All())).Should().Be(4);

                // for Int32 (or Int64) - add "bouble" on sequence
                var cint_10 = new EntityInt { Id = 10, Name = "R10" };
                var cint_11 = new EntityInt { Name = "R11" };
                var cint_7 = new EntityInt { Id = 7, Name = "R7" };
                var cint_12 = new EntityInt { Name = "R12" };

                await cs_int.InsertAsync(cint_10); // "loose" sequente between 5-9
                await cs_int.InsertAsync(cint_11); // insert as 11
                await cs_int.InsertAsync(cint_7); // insert as 7
                await cs_int.InsertAsync(cint_12); // insert as 12

                cint_10.Id.Should().Be(10);
                cint_11.Id.Should().Be(11);
                cint_7.Id.Should().Be(7);
                cint_12.Id.Should().Be(12);
            }
        }

        [Fact]
        public async Task AutoId_BsonDocument()
        {
            using (var db = new LiteDatabaseAsync(new MemoryStream()))
            {
                var col = db.GetCollection("Writers");
                await col.InsertAsync(new BsonDocument { ["Name"] = "Mark Twain" });
                await col.InsertAsync(new BsonDocument { ["Name"] = "Jack London", ["_id"] = 1 });

                // create an index in name field
                await col.EnsureIndexAsync("LowerName", "LOWER($.Name)");

                var mark = await col.FindOneAsync(Query.EQ("LOWER($.Name)", "mark twain"));
                var jack = await col.FindOneAsync(Query.EQ("LOWER($.Name)", "jack london"));

                // checks if auto-id is a ObjectId
                mark["_id"].IsObjectId.Should().BeTrue();
                jack["_id"].IsInt32.Should().BeTrue(); // jack do not use AutoId (fixed in int32)
            }
        }

        [Fact]
        public async Task AutoId_No_Duplicate_After_Delete()
        {
            // using strong type
            using (var db = new LiteDatabaseAsync(new MemoryStream()))
            {
                var col = db.GetCollection<EntityInt>("col1");

                var one = new EntityInt { Name = "One" };
                var two = new EntityInt { Name = "Two" };
                var three = new EntityInt { Name = "Three" };
                var four = new EntityInt { Name = "Four" };

                // insert
                await col.InsertAsync(one);
                await col.InsertAsync(two);

                one.Id.Should().Be(1);
                two.Id.Should().Be(2);

                // now delete first 2 rows
                await col.DeleteAsync(one.Id);
                await col.DeleteAsync(two.Id);

                // and insert new documents
                await col.InsertAsync(new EntityInt[] { three, four });

                three.Id.Should().Be(3);
                four.Id.Should().Be(4);
            }

            // using bsondocument/engine
            using (var db = new LiteDatabaseAsync(new MemoryStream()))
            {
                var one = new BsonDocument { ["Name"] = "One" };
                var two = new BsonDocument { ["Name"] = "Two" };
                var three = new BsonDocument { ["Name"] = "Three" };
                var four = new BsonDocument { ["Name"] = "Four" };

                var col = db.GetCollection("col", BsonAutoId.Int32);

                await col.InsertAsync(one);
                await col.InsertAsync(two);

                one["_id"].AsInt32.Should().Be(1);
                two["_id"].AsInt32.Should().Be(2);

                // now delete first 2 rows
                await col.DeleteAsync(one["_id"].AsInt32);
                await col.DeleteAsync(two["_id"].AsInt32);

                // and insert new documents
                await col.InsertAsync(new BsonDocument[] { three, four });

                three["_id"].AsInt32.Should().Be(3);
                four["_id"].AsInt32.Should().Be(4);
            }
        }

        [Fact]
        public async Task AutoId_Zero_Int()
        {
            using (var db = new LiteDatabaseAsync(":memory:"))
            {
                var test = db.GetCollection("Test", BsonAutoId.Int32);
                var doc = new BsonDocument() { ["_id"] = 0, ["p1"] = 1 };
                await test.InsertAsync(doc); // -> NullReferenceException
            }
        }
    }
}