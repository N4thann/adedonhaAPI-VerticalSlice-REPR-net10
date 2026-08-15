using AdedonhaAPI.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;

namespace AdedonhaAPI.Infrastructure.Database
{
    public class MongoDbIndexService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public MongoDbIndexService(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<MongoDbContext>();

            var categorySlugIndex = Builders<Category>.IndexKeys.Ascending(c => c.Slug);
            await dbContext.Categories.Indexes.CreateOneAsync(
                new CreateIndexModel<Category>(categorySlugIndex, new CreateIndexOptions { Unique = true }),
                cancellationToken: cancellationToken);

            var wordSlugIndex = Builders<Word>.IndexKeys.Ascending(w => w.Slug);
            await dbContext.Words.Indexes.CreateOneAsync(
                new CreateIndexModel<Word>(wordSlugIndex, new CreateIndexOptions { Unique = true }),
                cancellationToken: cancellationToken);

            var wordCategoryIndex = Builders<Word>.IndexKeys.Ascending("Categories.Slug");
            await dbContext.Words.Indexes.CreateOneAsync(
                new CreateIndexModel<Word>(wordCategoryIndex), cancellationToken: cancellationToken);

            var mainPageSearchIndex = Builders<Word>.IndexKeys
                .Ascending(w => w.InitialLetter)
                .Ascending("Categories.Slug");
            await dbContext.Words.Indexes.CreateOneAsync(
                new CreateIndexModel<Word>(mainPageSearchIndex), cancellationToken: cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
