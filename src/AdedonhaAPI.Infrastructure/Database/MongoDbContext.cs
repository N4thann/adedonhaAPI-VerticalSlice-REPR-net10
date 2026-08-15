using AdedonhaAPI.Domain.Entities;
using MongoDB.Driver;

namespace AdedonhaAPI.Infrastructure.Database
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IMongoClient client, string databaseName)
        {
            ArgumentNullException.ThrowIfNull(client);
            ArgumentException.ThrowIfNullOrEmpty(databaseName);
            _database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<Word> Words => _database.GetCollection<Word>("Words");
        public IMongoCollection<Category> Categories => _database.GetCollection<Category>("Categories");

        public IMongoCollection<T> GetCollection<T>(string name) => _database.GetCollection<T>(name);
    }
}
