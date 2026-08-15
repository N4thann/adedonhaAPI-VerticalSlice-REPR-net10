using AdedonhaAPI.Domain.Entities;

namespace AdedonhaAPI.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Word> Words { get; }
        IRepository<Category> Categories { get; }

        Task<bool> CommitAsync(CancellationToken cancellationToken = default);
    }
}
