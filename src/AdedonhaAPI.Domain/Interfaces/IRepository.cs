using AdedonhaAPI.Domain.Common;
using AdedonhaAPI.Domain.Entities;
using System.Linq.Expressions;

namespace AdedonhaAPI.Domain.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        Task AddAsync(T entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(string id, CancellationToken cancellationToken = default);

        Task<PagedResult<T>> GetPagedAsync(
            Expression<Func<T, bool>>? filter,
            Expression<Func<T, object>> orderBy,
            bool ascending,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<T?> GetFirstOrDefaultAsync(
            Expression<Func<T, bool>>? filter,
            Expression<Func<T, object>> orderBy,
            bool ascending,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<T>> GetRandomSampleAsync(
            Expression<Func<T, bool>> filter,
            int sampleSize,
            CancellationToken cancellationToken = default);
    }
}
