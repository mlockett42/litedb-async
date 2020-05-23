using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
using System.Threading.Tasks;
using LiteDB.Async;
using LiteDB;

namespace LiteDB.Async.Test
{
    public class IndexSortAndFilterTest : IDisposable
    {
        #region Model

        public class Item
        {
            public string Id { get; set; }
            public string Value { get; set; }
        }

        #endregion

        private LiteCollectionAsync<Item> _collection;
        private TempFile _tempFile;
        private LiteDatabaseAsync _database;

        public IndexSortAndFilterTest()
        {
            _tempFile = new TempFile();
            _database = new LiteDatabaseAsync(_tempFile.Filename);
            _collection = _database.GetCollection<Item>("items");

            var task1 = _collection.UpsertAsync(new Item() { Id = "C", Value = "Value 1" });
            var task2 = _collection.UpsertAsync(new Item() { Id = "A", Value = "Value 2" });
            var task3 = _collection.UpsertAsync(new Item() { Id = "B", Value = "Value 1" });

            Task.WaitAll(task1, task2, task3);

            var task4 =_collection.EnsureIndexAsync("idx_value", x => x.Value);
            task4.Wait();
        }

        public void Dispose()
        {
            _database.Dispose();
            _tempFile.Dispose();
        }

        [Fact]
        public async Task FilterAndSortAscending()
        {
            var result = await _collection.Query()
                .Where(x => x.Value == "Value 1")
                .OrderBy(x => x.Id, Query.Ascending)
                .ToListAsync();

            result[0].Id.Should().Be("B");
            result[1].Id.Should().Be("C");
        }

        [Fact]
        public async Task FilterAndSortDescending()
        {
            var result = await _collection.Query()
                .Where(x => x.Value == "Value 1")
                .OrderBy(x => x.Id, Query.Descending)
                .ToListAsync();

            result[0].Id.Should().Be("C");
            result[1].Id.Should().Be("B");
        }

        [Fact]
        public async Task FilterAndSortAscendingWithoutIndex()
        {
            await _collection.DropIndexAsync("idx_value");

            var result = await _collection.Query()
                .Where(x => x.Value == "Value 1")
                .OrderBy(x => x.Id, Query.Ascending)
                .ToListAsync();

            result[0].Id.Should().Be("B");
            result[1].Id.Should().Be("C");
        }

    }
}