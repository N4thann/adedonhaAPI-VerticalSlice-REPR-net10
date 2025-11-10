using AdedonhaAPI.Shared.Domain;
using MongoDB.Driver;

namespace AdedonhaAPI.Shared.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IMongoClient client, string databaseName) 
        { 
            if (client == null)
                throw new ArgumentNullException(nameof(client));
            if (string.IsNullOrEmpty(databaseName))
                throw new ArgumentNullException(nameof(databaseName));

            _database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<Word> Words =>
            _database.GetCollection<Word>("Words");

        public IMongoCollection<Category> Categories =>
            _database.GetCollection<Category>("Categories");
    }
}
