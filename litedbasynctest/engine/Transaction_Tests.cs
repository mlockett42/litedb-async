using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LiteDB.Engine;
using Xunit;
using LiteDB.Async;
using LiteDB;

namespace Tests.LiteDB.Async
{
    public class Transactions_Tests
    {
        [Fact]
        public async Task Transaction_Write_Lock_Timeout()
        {
            var data1 = DataGen.Person(1, 100).ToArray();
            var data2 = DataGen.Person(101, 200).ToArray();

            using (var db = new LiteDatabase("filename=:memory:"))
            using (var asyncDb = new LiteDatabaseAsync(db, false))
            {
                // small timeout
                await asyncDb.PragmaAsync(Pragmas.TIMEOUT, 1);

                var asyncPerson = asyncDb.GetCollection<Person>();
                var person = db.GetCollection<Person>();

                // init person collection with 100 document
                await asyncPerson.InsertAsync(data1);

                var taskASemaphore = new SemaphoreSlim(0, 1);
                var taskBSemaphore = new SemaphoreSlim(0, 1);

                // task A will open transaction and will insert +100 documents 
                // but will commit only 2s later
                var ta = Task.Run(async () =>
                {
                    await asyncDb.BeginTransAsync();

                    await asyncPerson.InsertAsync(data2);

                    taskBSemaphore.Release();
                    taskASemaphore.Wait();

                    var count = await asyncPerson.CountAsync();

                    count.Should().Be(data1.Length + data2.Length);

                    await asyncDb.CommitAsync();
                });

                // task B will try delete all documents but will be locked during 1 second
                var tb = Task.Run(() =>
                {
                    taskBSemaphore.Wait();

                    db.BeginTrans();
                    person
                        .Invoking(personCol => personCol.DeleteMany("1 = 1"))
                        .Should()
                        .Throw<LiteException>()
                        .Where(ex => ex.ErrorCode == LiteException.LOCK_TIMEOUT);

                    taskASemaphore.Release();
                });

                await Task.WhenAll(ta, tb);
            }
        }


        [Fact]
        public async Task Transaction_Avoid_Dirty_Read()
        {
            var data1 = DataGen.Person(1, 100).ToArray();
            var data2 = DataGen.Person(101, 200).ToArray();

            using (var db = new LiteDatabase(new MemoryStream()))
            using (var asyncDb = new LiteDatabaseAsync(db, false))
            {
                var asyncPerson = asyncDb.GetCollection<Person>();
                var person = db.GetCollection<Person>();

                // init person collection with 100 document
                await asyncPerson.InsertAsync(data1);

                var taskASemaphore = new SemaphoreSlim(0, 1);
                var taskBSemaphore = new SemaphoreSlim(0, 1);

                // task A will open transaction and will insert +100 documents 
                // but will commit only 1s later - this plus +100 document must be visible only inside task A
                var ta = Task.Run(async () =>
                {
                    await asyncDb.BeginTransAsync();

                    await asyncPerson.InsertAsync(data2);

                    taskBSemaphore.Release();
                    await taskASemaphore.WaitAsync();

                    var count = await asyncPerson.CountAsync();

                    count.Should().Be(data1.Length + data2.Length);

                    await asyncDb.CommitAsync();
                    taskBSemaphore.Release();
                });

                // task B will not open transaction and will wait 250ms before and count collection - 
                // at this time, task A already insert +100 document but here I can't see (are not committed yet)
                // after task A finish, I can see now all 200 documents
                var tb = Task.Run(() =>
                {
                    taskBSemaphore.Wait();

                    var count = person.Count();

                    // read 100 documents
                    count.Should().Be(data1.Length);

                    taskASemaphore.Release();
                    taskBSemaphore.Wait();

                    // read 200 documents
                    count = person.Count();

                    count.Should().Be(data1.Length + data2.Length);
                });

                await Task.WhenAll(ta, tb);
            }
        }

        [Fact]
        public async Task Transaction_Read_Version()
        {
            var data1 = DataGen.Person(1, 100).ToArray();
            var data2 = DataGen.Person(101, 200).ToArray();

            using (var db = new LiteDatabase(new MemoryStream()))
            using (var asyncDb = new LiteDatabaseAsync(db, false))
            {
                var asyncPerson = asyncDb.GetCollection<Person>();
                var person = db.GetCollection<Person>();

                // init person collection with 100 document
                await asyncPerson.InsertAsync(data1);

                var taskASemaphore = new SemaphoreSlim(0, 1);
                var taskBSemaphore = new SemaphoreSlim(0, 1);

                // task A will insert more 100 documents but will commit only 1s later
                var ta = Task.Run(async () =>
                {
                    await asyncDb.BeginTransAsync();

                    await asyncPerson.InsertAsync(data2);

                    taskBSemaphore.Release();
                    taskASemaphore.Wait();

                    await asyncDb.CommitAsync();

                    taskBSemaphore.Release();
                });

                // task B will open transaction too and will count 100 original documents only
                // but now, will wait task A finish - but is in transaction and must see only initial version
                var tb = Task.Run(() =>
                {
                    db.BeginTrans();

                    taskBSemaphore.Wait();

                    var count = person.Count();

                    // read 100 documents
                    count.Should().Be(data1.Length);

                    taskASemaphore.Release();
                    taskBSemaphore.Wait();

                    // keep reading 100 documents because i'm still in same transaction
                    count = person.Count();

                    count.Should().Be(data1.Length);
                });

                await Task.WhenAll(ta, tb);
            }
        }

        [Fact]
        public async Task Test_Transaction_States()
        {
            var data0 = DataGen.Person(1, 10).ToArray();
            var data1 = DataGen.Person(11, 20).ToArray();

            using (var db = new LiteDatabaseAsync(new MemoryStream()))
            {
                var person = db.GetCollection<Person>();

                // first time transaction will be opened
                (await db.BeginTransAsync()).Should().BeTrue();

                // but in second type transaction will be same
                (await db.BeginTransAsync()).Should().BeFalse();

                await person.InsertAsync(data0);

                // must commit transaction
                (await db.CommitAsync()).Should().BeTrue();

                // no transaction to commit
                (await db.CommitAsync()).Should().BeFalse();

                // no transaction to rollback;
                (await db.RollbackAsync()).Should().BeFalse();

                (await db.BeginTransAsync()).Should().BeTrue();

                // no page was changed but ok, let's rollback anyway
                (await db.RollbackAsync()).Should().BeTrue();

                // auto-commit
                await person.InsertAsync(data1);

                (await person.CountAsync()).Should().Be(20);
            }
        }
    }
}