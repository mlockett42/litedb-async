using System;
using System.Collections.Generic;
using Xunit;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using LiteDB.Async;

namespace Tests.LiteDB.Async
{
    public class SimpleRepositoryTest : IDisposable
    {
        private readonly string _databasePath;
        private readonly LiteRepositoryAsync _repo;
        public SimpleRepositoryTest()
        {
            _databasePath = Path.Combine(Path.GetTempPath(), "litedbn-async-testing-" + Path.GetRandomFileName() + ".db");
            _repo = new LiteRepositoryAsync(new LiteDatabaseAsync(_databasePath));
        }

        [Fact]
        public async Task UpsertAsync()
        {

            var person = new SimplePerson()
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Smith"
            };

            var upsertResult = await _repo.UpsertAsync(person);
            Assert.True(upsertResult);

            var listResult = await _repo.Query<SimplePerson>().ToListAsync();
            Assert.Single(listResult);
            var resultPerson = listResult[0];
            Assert.Equal(person.Id, resultPerson.Id);
            Assert.Equal(person.FirstName, resultPerson.FirstName);
            Assert.Equal(person.LastName, resultPerson.LastName);
        }
        
        [Fact]
        public async Task UpsertAsyncList()
        {
            await InsertAsyncList();

            var list = await _repo.Query<SimplePerson>().ToListAsync();
            
            list[0].FirstName = "Hallo";
            list[0].LastName = "Helga";
            list[1].LastName = "byby";
            list[1].LastName = "Roger";
            await _repo.UpsertAsync<SimplePerson>(list);

            var listResult = await _repo.Query<SimplePerson>().ToListAsync();
            listResult.Count.Should().Be(2);
            var resultPerson = listResult.FirstOrDefault(x=>x.Id==list[0].Id);
            var resultPerson2 = listResult.FirstOrDefault(x=>x.Id==list[1].Id);
            Assert.Equal(list[0].FirstName, resultPerson.FirstName);
            Assert.Equal(list[0].LastName, resultPerson.LastName);
            Assert.Equal(list[1].FirstName, resultPerson2.FirstName);
            Assert.Equal(list[1].LastName, resultPerson2.LastName);
            

        }


        [Fact]
        public async Task<Guid> InsertAsync()
        {

            var person = new SimplePerson()
            {
                FirstName = "John",
                LastName = "Smith"
            };

            await _repo.InsertAsync(person);

            var resultPerson = await _repo.SingleByIdAsync<SimplePerson>(person.Id);
            Assert.Equal(person.Id, resultPerson.Id);
            Assert.Equal(person.FirstName, resultPerson.FirstName);
            Assert.Equal(person.LastName, resultPerson.LastName);
            return resultPerson.Id;
        }
        
        
        [Fact]
        public async Task InsertAsyncList()
        {
            
            var list = GetListOflist();

            int insertResult = await _repo.InsertAsync<SimplePerson>(list);
            insertResult.Should().Be(2);

            var listResult = await _repo.Query<SimplePerson>().ToListAsync();
            listResult.Count.Should().Be(2);
            var resultPerson = listResult.FirstOrDefault(x=>x.Id==list[0].Id);
            var resultPerson2 = listResult.FirstOrDefault(x=>x.Id==list[1].Id);
            Assert.Equal(list[0].FirstName, resultPerson.FirstName);
            Assert.Equal(list[0].LastName, resultPerson.LastName);
            Assert.Equal(list[1].FirstName, resultPerson2.FirstName);
            Assert.Equal(list[1].LastName, resultPerson2.LastName);
        }
        
        [Fact]
        public async Task UpdateAsync()
        {

            var id = await InsertAsync();
            var person = await _repo.FirstOrDefaultAsync<SimplePerson>(x=>x.Id==id);

            person.FirstName = "Hallo";
            person.LastName = "Helga";
            await _repo.UpdateAsync(person);

            SimplePerson resultPerson = await _repo.SingleOrDefaultAsync<SimplePerson>(x=>x.Id == person.Id);
            Assert.Equal(person.Id, resultPerson.Id);
            Assert.Equal( person.FirstName, resultPerson.FirstName);
            Assert.Equal( person.LastName , resultPerson.LastName);
        }
        
        
        [Fact]
        public async Task UpdateAsyncList()
        {
            await InsertAsyncList();
            var list = await _repo.Query<SimplePerson>().ToListAsync();

            list[0].FirstName = "Hallo";
            list[0].LastName = "Helga";
            list[1].LastName = "byby";
            list[1].LastName = "Roger";
            await _repo.UpdateAsync<SimplePerson>(list);

            var listResult = await _repo.Query<SimplePerson>().ToListAsync();
            listResult.Count.Should().Be(2);
            var resultPerson = listResult.FirstOrDefault(x=>x.Id==list[0].Id);
            var resultPerson2 = listResult.FirstOrDefault(x=>x.Id==list[1].Id);
            Assert.Equal(list[0].FirstName, resultPerson.FirstName);
            Assert.Equal(list[0].LastName, resultPerson.LastName);
            Assert.Equal(list[1].FirstName, resultPerson2.FirstName);
            Assert.Equal(list[1].LastName, resultPerson2.LastName);
        }

        [Fact]
        public async Task TestInsertingSameRecordTwiceRaisesException()
        {
            var person = new SimplePerson()
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Smith"
            };

            await _repo.InsertAsync(person);
            await Assert.ThrowsAnyAsync<LiteAsyncException>(async () => await _repo.InsertAsync(person));
        }

        
        [Fact]
        public async Task DeleteAsync()
        {
            
            var id = await InsertAsync();
            var simplePerson = await _repo.SingleAsync<SimplePerson>(x=>x.Id==id);
            bool deleteResult = await _repo.DeleteAsync<SimplePerson>(simplePerson.Id);
            deleteResult.Should().BeTrue();
            
            var listResult = await _repo.Query<SimplePerson>().ToListAsync();
            listResult.Count.Should().Be(0);
        }
        
        [Fact]
        public async Task DeleteManyAsync()
        {
            
            await InsertAsyncList();
            var listResultBefore = await _repo.Query<SimplePerson>().ToListAsync();
            listResultBefore.Count.Should().Be(2);
            var idsBefore = listResultBefore.Select(x => x.Id);
            int deleteResult = await _repo.DeleteManyAsync<SimplePerson>(x=>idsBefore.Contains(x.Id));
            deleteResult.Should().Be(2);
            
            var listResult = await _repo.Query<SimplePerson>().ToListAsync();
            listResult.Count.Should().Be(0);
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _repo.Dispose();
                File.Delete(_databasePath);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        private static List<SimplePerson> GetListOflist()
        {
            var list = new List<SimplePerson>()
            {
                new SimplePerson()
                {
                    FirstName = "John",
                    LastName = "Smith"
                },
                new SimplePerson()
                {
                    FirstName = "Max",
                    LastName = "Mustermann"
                },
            };
            return list;
        }
    }
}
