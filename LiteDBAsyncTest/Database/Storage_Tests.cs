﻿using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using FluentAssertions;
using LiteDB;
using LiteDB.Async;
using Tests.LiteDB.Async;
using Xunit;

namespace Tests.LiteDB.Async
{
    public class Storage_Tests
    {
        private Random _rnd = new Random();
        private byte[] _smallFile;
        private byte[] _bigFile;
        private string _smallHash;
        private string _bigHash;

        public Storage_Tests()
        {
            _smallFile = new byte[_rnd.Next(100000, 200000)];
            _bigFile = new byte[_rnd.Next(400000, 600000)];

            _rnd.NextBytes(_smallFile);
            _rnd.NextBytes(_bigFile);

            _smallHash = this.HashFile(_smallFile);
            _bigHash = this.HashFile(_bigFile);
        }

        [Fact]
        public async Task Storage_Upload_Download()
        {
            using (var f = new TempFile())
            using (var db = new LiteDatabaseAsync(f.Filename))
            //using (var db = new LiteDatabase(@"c:\temp\file.db"))
            {
                var fs = db.GetStorage<int>("_files", "_chunks");

                var small = await fs.UploadAsync(10, "photo_small.png", new MemoryStream(_smallFile));
                var big = await fs.UploadAsync(100, "photo_big.png", new MemoryStream(_bigFile));

                _smallFile.Length.Should().Be((int)small.Length);
                _bigFile.Length.Should().Be((int)big.Length);

                var f0 = (await fs.FindAsync(x => x.Filename == "photo_small.png")).First();
                var f1 = (await fs.FindAsync(x => x.Filename == "photo_big.png")).First();

                this.HashFile(f0.OpenRead()).Should().Be(_smallHash);
                this.HashFile(f1.OpenRead()).Should().Be(_bigHash);

                // now replace small content with big-content
                var repl = await fs.UploadAsync(10, "new_photo.jpg", new MemoryStream(_bigFile));

                (await fs.ExistsAsync(10)).Should().BeTrue();

                var nrepl = await fs.FindByIdAsync(10);

                nrepl.Chunks.Should().Be(repl.Chunks);

                // update metadata
                await fs.SetMetadataAsync(100, new BsonDocument { ["x"] = 100, ["y"] = 99 });

                // find using metadata
                var md = (await fs.FindAsync(x => x.Metadata["x"] == 100)).FirstOrDefault();

                md.Metadata["y"].AsInt32.Should().Be(99);
            }
        }

        private string HashFile(Stream stream)
        {
            var m = new MemoryStream();
            stream.CopyTo(m);
            return this.HashFile(m.ToArray());
        }

        private string HashFile(byte[] input)
        {
            using (var md5 = MD5.Create())
            {
                var bytes = md5.ComputeHash(input);
                return Convert.ToBase64String(bytes);
            }
        }
    }
}