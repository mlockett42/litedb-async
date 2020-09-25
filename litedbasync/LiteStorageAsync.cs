using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LiteDB.Async
{
    public class LiteStorageAsync<TFileId> : ILiteStorageAsync<TFileId>
    {
        private readonly ILiteStorage<TFileId> _wrappedStorage;
        private readonly LiteDatabaseAsync _liteDatabaseAsync;

        public LiteStorageAsync(LiteDatabaseAsync liteDatabaseAsync, ILiteDatabase liteDb, string filesCollection = "_files", string chunksCollection = "_chunks")
        {
            _wrappedStorage = new LiteStorage<TFileId>(liteDb, filesCollection, chunksCollection);
            _liteDatabaseAsync = liteDatabaseAsync;
        }

        /// <summary>
        /// Find a file inside datafile and returns LiteFileInfo instance. Returns null if not found
        /// </summary>
        public Task<LiteFileInfo<TFileId>> FindByIdAsync(TFileId id)
        {
            var tcs = new TaskCompletionSource<LiteFileInfo<TFileId>>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(_wrappedStorage.FindById(id));
            });
            return tcs.Task;
        }

        /// <summary>
        /// Find all files that match with predicate expression.
        /// </summary>
        public Task<IEnumerable<LiteFileInfo<TFileId>>> FindAsync(BsonExpression predicate)
        {
            var tcs = new TaskCompletionSource<IEnumerable<LiteFileInfo<TFileId>>>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(_wrappedStorage.Find(predicate));
            });
            return tcs.Task;
        }

        /// <summary>
        /// Find all files that match with predicate expression.
        /// </summary>
        public Task<IEnumerable<LiteFileInfo<TFileId>>> FindAsync(string predicate, BsonDocument parameters)
        {
            var tcs = new TaskCompletionSource<IEnumerable<LiteFileInfo<TFileId>>>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(_wrappedStorage.Find(predicate, parameters));
            });
            return tcs.Task;
        }

        /// <summary>
        /// Find all files that match with predicate expression.
        /// </summary>
        public Task<IEnumerable<LiteFileInfo<TFileId>>> FindAsync(string predicate, params BsonValue[] args)
        {
            var tcs = new TaskCompletionSource<IEnumerable<LiteFileInfo<TFileId>>>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(_wrappedStorage.Find(predicate, args));
            });
            return tcs.Task;
        }

        /// <summary>
        /// Find all files that match with predicate expression.
        /// </summary>
        public Task<IEnumerable<LiteFileInfo<TFileId>>> FindAsync(Expression<Func<LiteFileInfo<TFileId>, bool>> predicate)
        {
            var tcs = new TaskCompletionSource<IEnumerable<LiteFileInfo<TFileId>>>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(_wrappedStorage.Find(predicate));
            });
            return tcs.Task;
        }

        /// <summary>
        /// Find all files inside file collections
        /// </summary>
        public Task<IEnumerable<LiteFileInfo<TFileId>>> FindAllAsync()
        {
            var tcs = new TaskCompletionSource<IEnumerable<LiteFileInfo<TFileId>>>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(_wrappedStorage.FindAll());
            });
            return tcs.Task;
        }

        /// <summary>
        /// Returns if a file exisits in database
        /// </summary>
        public Task<bool> ExistsAsync(TFileId id)
        {
            var tcs = new TaskCompletionSource<bool>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(_wrappedStorage.Exists(id));
            });
            return tcs.Task;
        }

        /// <summary>
        /// Open/Create new file storage and returns linked Stream to write operations.
        /// </summary>
        public Task<LiteFileStream<TFileId>> OpenWriteAsync(TFileId id, string filename, BsonDocument metadata = null)
        {
            var tcs = new TaskCompletionSource<LiteFileStream<TFileId>>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(_wrappedStorage.OpenWrite(id, filename, metadata));
            });
            return tcs.Task;
        }

        /// <summary>
        /// Upload a file based on stream data
        /// </summary>
        public Task<LiteFileInfo<TFileId>> UploadAsync(TFileId id, string filename, Stream stream, BsonDocument metadata = null)
        {
            var tcs = new TaskCompletionSource<LiteFileInfo<TFileId>>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(_wrappedStorage.Upload(id, filename, stream, metadata));
            });
            return tcs.Task;
        }

        /// <summary>
        /// Upload a file based on file system data
        /// </summary>
        public Task<LiteFileInfo<TFileId>> UploadAsync(TFileId id, string filename)
        {
            var tcs = new TaskCompletionSource<LiteFileInfo<TFileId>>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(_wrappedStorage.Upload(id, filename));
            });
            return tcs.Task;
        }

        /// <summary>
        /// Update metadata on a file. File must exist.
        /// </summary>
        public Task<bool> SetMetadataAsync(TFileId id, BsonDocument metadata)
        {
            var tcs = new TaskCompletionSource<bool>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(_wrappedStorage.SetMetadata(id, metadata));
            });
            return tcs.Task;
        }

        /// <summary>
        /// Load data inside storage and returns as Stream
        /// </summary>
        public Task<LiteFileStream<TFileId>> OpenReadAsync(TFileId id)
        {
            var tcs = new TaskCompletionSource<LiteFileStream<TFileId>>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(_wrappedStorage.OpenRead(id));
            });
            return tcs.Task;
        }

        /// <summary>
        /// Copy all file content to a steam
        /// </summary>
        public Task<LiteFileInfo<TFileId>> DownloadAsync(TFileId id, Stream stream)
        {
            var tcs = new TaskCompletionSource<LiteFileInfo<TFileId>>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(_wrappedStorage.Download(id, stream));
            });
            return tcs.Task;
        }

        /// <summary>
        /// Copy all file content to a file
        /// </summary>
        public Task<LiteFileInfo<TFileId>> DownloadAsync(TFileId id, string filename, bool overwritten)
        {
            var tcs = new TaskCompletionSource<LiteFileInfo<TFileId>>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(_wrappedStorage.Download(id, filename, overwritten));
            });
            return tcs.Task;
        }

        /// <summary>
        /// Delete a file inside datafile and all metadata related
        /// </summary>
        public Task<bool> DeleteAsync(TFileId id)
        {
            var tcs = new TaskCompletionSource<bool>();
            _liteDatabaseAsync.Enqueue(tcs, () => {
                tcs.SetResult(_wrappedStorage.Delete(id));
            });
            return tcs.Task;
        }
    }
}
