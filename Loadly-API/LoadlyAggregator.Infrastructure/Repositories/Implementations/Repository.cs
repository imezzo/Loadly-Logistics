using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public virtual async Task<TEntity?> GetByIdAsync(long id) =>
        await GetByIdAsync(id, includeDeleted: false);

    public virtual async Task<TEntity?> GetByIdAsync(long id, bool includeDeleted) =>
        await (includeDeleted ? _dbSet.IgnoreQueryFilters() : _dbSet.AsQueryable())
            .FirstOrDefaultAsync(e => (e as IEntity)!.Id == id);

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync() =>
        await GetAllAsync(includeDeleted: false);

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(bool includeDeleted) =>
        await (includeDeleted ? _dbSet.IgnoreQueryFilters() : _dbSet.AsQueryable())
            .ToListAsync();

    public virtual async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate) =>
        await FindAsync(predicate, includeDeleted: false);

    public virtual async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, bool includeDeleted) =>
        await (includeDeleted ? _dbSet.Where(predicate).IgnoreQueryFilters() : _dbSet.Where(predicate))
            .ToListAsync();

    public virtual async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate) =>
        await FirstOrDefaultAsync(predicate, includeDeleted: false);

    public virtual async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool includeDeleted) =>
        await (includeDeleted ? _dbSet.Where(predicate).IgnoreQueryFilters() : _dbSet.Where(predicate))
            .FirstOrDefaultAsync();

    public virtual async Task AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public virtual async Task<TEntity> CreateAsync(TEntity entity)
    {
        await AddAsync(entity);
        return entity;
    }

    public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }

    public virtual void Update(TEntity entity)
    {
        _dbSet.Update(entity);
    }

    public virtual Task UpdateAsync(TEntity entity)
    {
        Update(entity);
        return Task.CompletedTask;
    }
    public virtual void UpdateRange(IEnumerable<TEntity> entities)
    {
        _dbSet.UpdateRange(entities);
    }
    public virtual Task UpdateRangeAsync(IEnumerable<TEntity> entities)
    {
        UpdateRange(entities);
        return Task.CompletedTask;
    }
    public virtual void Remove(TEntity entity)
    {
        (entity as ISoftDeletable)?.SetDeletedAt(DateTime.UtcNow);
        ((entity as ISoftDeletable) != null ? new Action(() => _dbSet.Update(entity)) : new Action(() => _dbSet.Remove(entity)))();
    }

    public virtual Task DeleteAsync(TEntity entity)
    {
        Remove(entity);
        return Task.CompletedTask;
    }

    public virtual void RemoveRange(IEnumerable<TEntity> entities)
    {
        entities.OfType<ISoftDeletable>().ToList().ForEach(e => e.SetDeletedAt(DateTime.UtcNow));
        (entities.OfType<ISoftDeletable>().Any() ? new Action(() => _dbSet.UpdateRange(entities)) : new Action(() => _dbSet.RemoveRange(entities)))();
    }

    public virtual async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate) =>
        await ExistsAsync(predicate, includeDeleted: false);

    public virtual async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, bool includeDeleted) =>
        await (includeDeleted ? _dbSet.Where(predicate).IgnoreQueryFilters() : _dbSet.Where(predicate))
            .AnyAsync();

    public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null) =>
        await CountAsync(predicate, includeDeleted: false);

    public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate, bool includeDeleted) =>
        await (includeDeleted
            ? (predicate != null ? _dbSet.Where(predicate).IgnoreQueryFilters() : _dbSet.IgnoreQueryFilters())
            : (predicate != null ? _dbSet.Where(predicate) : _dbSet.AsQueryable()))
            .CountAsync();

    public virtual IQueryable<TEntity> Query() =>
        Query(includeDeleted: false);

    public virtual IQueryable<TEntity> Query(bool includeDeleted) =>
        includeDeleted ? _dbSet.IgnoreQueryFilters() : _dbSet.AsQueryable();

    public virtual async Task<int> SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}

