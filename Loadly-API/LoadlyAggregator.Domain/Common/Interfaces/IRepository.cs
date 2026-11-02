using System.Linq.Expressions;
namespace LoadlyAggregator.Domain.Common.Interfaces;



public interface IRepository<TEntity> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(long id);
    Task<TEntity?> GetByIdAsync(long id, bool includeDeleted);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<IEnumerable<TEntity>> GetAllAsync(bool includeDeleted);
    Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
    Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, bool includeDeleted);
    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool includeDeleted);
    Task AddAsync(TEntity entity);
    Task<TEntity> CreateAsync(TEntity entity);
    Task AddRangeAsync(IEnumerable<TEntity> entities);
    void Update(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task UpdateRangeAsync(IEnumerable<TEntity> entities);
    void Remove(TEntity entity);
    Task DeleteAsync(TEntity entity);
    void RemoveRange(IEnumerable<TEntity> entities);
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate);
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, bool includeDeleted);
    Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null);
    Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate, bool includeDeleted);
    IQueryable<TEntity> Query();
    IQueryable<TEntity> Query(bool includeDeleted);
    Task<int> SaveChangesAsync();
}
