using AdedonhaAPI.Shared.Domain;
using MongoDB.Driver;

namespace AdedonhaAPI.Shared.Data
{
    public class MongoDbIndexService : IHostedService
    {
        private readonly IServiceProvider  _serviceProvider;

        public MongoDbIndexService(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<Context>();

                // Índice 1: Para buscar categorias pelo 'Slug' (nome-url)
                var categorySlugIndex = Builders<Category>.IndexKeys
                    .Ascending(c => c.Slug);
                var categoryIndexModel = new CreateIndexModel<Category>(categorySlugIndex,
                    new CreateIndexOptions { Unique = true });
                await dbContext.Categories.Indexes.CreateOneAsync(categoryIndexModel,
                    cancellationToken: cancellationToken);

                // Índice 2: Para buscar palavras pelo 'Slug' (nome-url)
                var wordSlugIndex = Builders<Word>.IndexKeys
                    .Ascending(w => w.Slug);
                var wordIndexModel = new CreateIndexModel<Word>(wordSlugIndex,
                    new CreateIndexOptions { Unique = true });
                await dbContext.Words.Indexes.CreateOneAsync(wordIndexModel, 
                    cancellationToken: cancellationToken);

                // Índice 3: Para a tabela de paginação, todas as palavras de uma categoria
                var wordCategoryIndex = Builders<Word>.IndexKeys
                    .Ascending("Categories.Slug");
                var wordCategory = new CreateIndexModel<Word>(wordCategoryIndex);
                await dbContext.Words.Indexes.CreateOneAsync(wordCategory, 
                    cancellationToken: cancellationToken);

                // Índice 4: Para a página principal. Pega 10 palavras de uma letra y da categoria x
                var mainPageSearchIndex = Builders<Word>.IndexKeys
                    .Ascending(w => w.InitialLetter)
                    .Ascending("Categories.Slug");
                var mainPageSeach = new CreateIndexModel<Word>(mainPageSearchIndex);
                await dbContext.Words.Indexes.CreateOneAsync(mainPageSeach,
                    cancellationToken: cancellationToken);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
