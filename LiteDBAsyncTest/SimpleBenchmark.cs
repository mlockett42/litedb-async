using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LiteDB.Async;
using Xunit;
using Xunit.Abstractions;

namespace Tests.LiteDB.Async
{
    public class SimpleBenchmark
    {
        const int Iterations = 10;
        const int InsertCount = 1000;
        const int QueryCount = 1000;

        readonly ITestOutputHelper _testOutputHelper;

        public SimpleBenchmark(ITestOutputHelper testOutputHelper) => _testOutputHelper = testOutputHelper;

        [Fact]
        [Description("Simple benchmark to give an indication of comparable performance before/after removing work queue lock")]
        public void TakeTime()
        {
            var measurements = Enumerable.Range(0, Iterations)
                .Select(n => MeasureTime(iteration: n).GetAwaiter().GetResult())
                .ToList();

            var averageMilliseconds = measurements.Average(t => t.TotalMilliseconds);

            _testOutputHelper.WriteLine($"Performed {InsertCount} inserts and {QueryCount} queries in {averageMilliseconds:0.0} ms on average");
        }

        async Task<TimeSpan> MeasureTime(int iteration)
        {
            using var tempFile = new TempFile();
            using var db = new LiteDatabaseAsync(tempFile.Filename);

            var collection = db.GetCollection<Something>();

            _testOutputHelper.WriteLine($"Iteration {iteration}/{Iterations}");

            var stopwatch = Stopwatch.StartNew();

            foreach (var doc in Enumerable.Range(0, InsertCount).Select(n => new Something { Text = Guid.NewGuid().ToString() }))
            {
                await collection.InsertAsync(doc);
            }

            var randomStringToQueryBy = Guid.NewGuid().ToString();

            for (var counter = 0; counter < QueryCount; counter++)
            {
                var @null = await collection.FindOneAsync(s => s.Text == randomStringToQueryBy);
            }

            return stopwatch.Elapsed;
        }

        class Something
        {
            public string Text { get; set; }
        }
    }
}