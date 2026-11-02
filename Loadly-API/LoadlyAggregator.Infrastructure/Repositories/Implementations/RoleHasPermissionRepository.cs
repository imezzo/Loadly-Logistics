using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class RoleHasPermissionRepository : Repository<RoleHasPermission>, IRoleHasPermissionRepository
{
    public RoleHasPermissionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public Task<RoleHasPermission?> GetByRoleIdAndPermissionIdAsync(long roleId, long permissionId) =>
        _dbSet
            .Include(rhp => rhp.Role)
            .Include(rhp => rhp.Permission)
            .FirstOrDefaultAsync(rhp => rhp.RoleId == roleId && rhp.PermissionId == permissionId);

    public Task<List<RoleHasPermission>> GetByRoleIdAndPermissionIdsAsync(long roleId, List<long> permissionIds) =>
        _dbSet
            .Include(rhp => rhp.Role)
            .Include(rhp => rhp.Permission)
            .Where(rhp => rhp.RoleId == roleId && permissionIds.Contains(rhp.PermissionId))
            .ToListAsync();

    public Task<List<RoleHasPermission>> GetByRoleIdAsync(long roleId) =>
        _dbSet
            .Include(rhp => rhp.Role)
            .Include(rhp => rhp.Permission)
            .Where(rhp => rhp.RoleId == roleId)
            .ToListAsync();

    public Task<List<RoleHasPermission>> GetByPermissionIdAsync(long permissionId) =>
        _dbSet
            .Include(rhp => rhp.Role)
            .Include(rhp => rhp.Permission)
            .Where(rhp => rhp.PermissionId == permissionId)
            .ToListAsync();

    public override Task DeleteAsync(RoleHasPermission roleHasPermission)
    {
        _dbSet.Remove(roleHasPermission);
        return Task.CompletedTask;
    }

    public override IQueryable<RoleHasPermission> Query() =>
        _dbSet.Include(rhp => rhp.Role).Include(rhp => rhp.Permission).AsQueryable();

    public Task<List<Permission>> GetNonDeletedPermissionsByRoleIdAsync(long roleId) =>
        _dbSet
            .Where(rhp => rhp.RoleId == roleId)
            .Include(rhp => rhp.Permission)
            .Select(rhp => rhp.Permission)
            .ToListAsync();

    public Task<List<long>> GetPermissionIdsByRoleIdAsync(long roleId) =>
        _dbSet.Where(rhp => rhp.RoleId == roleId).Select(rhp => rhp.PermissionId).ToListAsync();
}
