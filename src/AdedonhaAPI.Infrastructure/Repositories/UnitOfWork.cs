using AdedonhaAPI.Domain.Entities;
using AdedonhaAPI.Domain.Interfaces;
using AdedonhaAPI.Infrastructure.Database;

namespace AdedonhaAPI.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MongoDbContext _context;
        private IRepository<Word>? _words;
        private IRepository<Category>? _categories;
        private IRepository<AboutSiteContent>? _aboutSite;

        public UnitOfWork(MongoDbContext context) => _context = context;

        public IRepository<Word> Words => _words ??= new MongoRepository<Word>(_context, "Words");
        public IRepository<Category> Categories => _categories ??= new MongoRepository<Category>(_context, "Categories");
        public IRepository<AboutSiteContent> AboutSite => _aboutSite ??= new MongoRepository<AboutSiteContent>(_context, "AboutSite");

        public Task<bool> CommitAsync(CancellationToken cancellationToken = default) => Task.FromResult(true);

        public void Dispose() => GC.SuppressFinalize(this);
    }
}
