using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class PermissionRepository : Repository<Permission>, IPermissionRepository
{
    public PermissionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override Task<Permission?> GetByIdAsync(long id) =>
        _dbSet.FirstOrDefaultAsync(p => p.Id == id);

    public Task<Permission?> GetByNameAndGuardAsync(string name, string guardName) =>
        _dbSet.FirstOrDefaultAsync(p => p.Name == name && p.GuardName == guardName);

    public Task<List<Permission>> GetByGuardAsync(string guardName) =>
        _dbSet
            .Include(p => p.PermissionGroup)
            .Where(p => p.GuardName == guardName)
            .OrderBy(p => p.Name)
            .ToListAsync();

    public Task<List<Permission>> GetByIdsAsync(List<long> ids) =>
        _dbSet.Where(p => ids.Contains(p.Id)).ToListAsync();

    public async Task<Permission> GetByIdOrThrowAsync(long id) =>
        await _dbSet.FirstOrDefaultAsync(p => p.Id == id) ?? throw new NotFoundException("permissions.not_found_with_id", id);

    public override async Task<IEnumerable<Permission>> GetAllAsync() =>
        await _dbSet.OrderBy(p => p.GuardName).ThenBy(p => p.Name).ToListAsync();

    public override async Task UpdateAsync(Permission permission)
    {
        _dbSet.Update(permission);
        await _context.SaveChangesAsync();
    }

    public override async Task DeleteAsync(Permission permission)
    {
        permission.DeletedAt = DateTime.UtcNow;
        _dbSet.Update(permission);
        await _context.SaveChangesAsync();
    }
}
