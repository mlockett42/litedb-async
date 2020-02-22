using System;
using LiteDB;


namespace litedbasync
{
    public class LiteDatabaseAsync
    {
        private readonly LiteDatabase _liteDB;
        public LiteDatabaseAsync(string connectionString)
        {
            _liteDB = new LiteDatabase(connectionString);
        }

        public LiteCollectionAsync<T> GetCollection<T>()
        {
            return this.GetCollection<T>(null);
        }

        public LiteCollectionAsync<T> GetCollection<T>(string name)
        {
            return new LiteCollectionAsync<T>(_liteDB.GetCollection<T>(name));
        }
    }
}
