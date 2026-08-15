using AdedonhaAPI.Domain.Common;
using AdedonhaAPI.Domain.Entities;
using AdedonhaAPI.Domain.Interfaces;
using AdedonhaAPI.Infrastructure.Database;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace AdedonhaAPI.Infrastructure.Repositories
{
    public class MongoRepository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly IMongoCollection<T> _collection;

        public MongoRepository(MongoDbContext dbContext, string collectionName)
        {
            _collection = dbContext.GetCollection<T>(collectionName);
        }

        public async Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default) =>
            await _collection.Find(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);

        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default) =>
            await _collection.Find(_ => true).ToListAsync(cancellationToken);

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) =>
            await _collection.Find(predicate).ToListAsync(cancellationToken);

        public async Task AddAsync(T entity, CancellationToken cancellationToken = default) =>
            await _collection.InsertOneAsync(entity, cancellationToken: cancellationToken);

        public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default) =>
            await _collection.ReplaceOneAsync(x => x.Id == entity.Id, entity, cancellationToken: cancellationToken);

        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default) =>
            await _collection.DeleteOneAsync(x => x.Id == id, cancellationToken: cancellationToken);

        public async Task<PagedResult<T>> GetPagedAsync(
            Expression<Func<T, bool>>? filter, Expression<Func<T, object>> orderBy, bool ascending,
            int page, int pageSize, CancellationToken cancellationToken = default)
        {
            Expression<Func<T, bool>> effectiveFilter = filter ?? (_ => true);
            var find = _collection.Find(effectiveFilter);
            find = ascending ? find.SortBy(orderBy) : find.SortByDescending(orderBy);
            var items = await find.Skip((page - 1) * pageSize).Limit(pageSize).ToListAsync(cancellationToken);
            var total = await _collection.CountDocumentsAsync(effectiveFilter, cancellationToken: cancellationToken);
            return new PagedResult<T>(items, total, page, pageSize);
        }

        public async Task<T?> GetFirstOrDefaultAsync(
            Expression<Func<T, bool>>? filter, Expression<Func<T, object>> orderBy, bool ascending,
            CancellationToken cancellationToken = default)
        {
            var find = _collection.Find(filter ?? (_ => true));
            find = ascending ? find.SortBy(orderBy) : find.SortByDescending(orderBy);
            return await find.Limit(1).FirstOrDefaultAsync(cancellationToken);
        }
    }
}
