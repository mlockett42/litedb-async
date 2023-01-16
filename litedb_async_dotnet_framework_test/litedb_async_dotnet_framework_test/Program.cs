using LiteDB.Async;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace litedb_async_dotnet_framework_test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var db = new LiteDatabaseAsync(new MemoryStream());

            Console.WriteLine("Hello world");
            Console.ReadLine();
        }
    }
}
