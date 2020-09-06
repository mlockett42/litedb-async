using System;
using Xunit;
using LiteDB.Async;
using System.IO;

namespace Tests.LiteDB.Async
{
    public class SimplePerson
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}