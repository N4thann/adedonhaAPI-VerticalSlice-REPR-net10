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

            var wordCategoryLetterIndex = Builders<Word>.IndexKeys
                .Ascending("Categories.CategoryId")
                .Ascending(w => w.InitialLetter)
                .Ascending(w => w.Name);
            await dbContext.Words.Indexes.CreateOneAsync(
                new CreateIndexModel<Word>(wordCategoryLetterIndex), cancellationToken: cancellationToken);

            var wordAdminListIndex = Builders<Word>.IndexKeys.Ascending(w => w.IsActive).Ascending(w => w.Name);
            await dbContext.Words.Indexes.CreateOneAsync(
                new CreateIndexModel<Word>(wordAdminListIndex), cancellationToken: cancellationToken);

            var wordAdminCategoryFilterIndex = Builders<Word>.IndexKeys
                .Ascending(w => w.IsActive)
                .Ascending("Categories.CategoryId")
                .Ascending(w => w.Name);
            await dbContext.Words.Indexes.CreateOneAsync(
                new CreateIndexModel<Word>(wordAdminCategoryFilterIndex), cancellationToken: cancellationToken);

            var categoryAdminListIndex = Builders<Category>.IndexKeys.Ascending(c => c.IsActive).Ascending(c => c.Name);
            await dbContext.Categories.Indexes.CreateOneAsync(
                new CreateIndexModel<Category>(categoryAdminListIndex), cancellationToken: cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
