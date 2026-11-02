using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class RoleRepository : Repository<Role>, IRoleRepository
{
    public RoleRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<Role?> GetByIdAsync(long id) =>
        await _dbSet
            .Include(r => r.RolePermissions)
                .ThenInclude(rhp => rhp.Permission)
            .FirstOrDefaultAsync(r => r.Id == id);

    public async Task<Role?> GetByNameAndGuardAsync(string name, string guardName) =>
        await _dbSet
            .Include(r => r.RolePermissions)
                .ThenInclude(rhp => rhp.Permission)
            .FirstOrDefaultAsync(r => r.Name.ToLower() == name.ToLower() && r.GuardName == guardName);

    public async Task<Role?> GetByNameAndGuardAndCompanyAsync(string name, string guardName, long? companyId) =>
        await _dbSet
            .Include(r => r.RolePermissions)
                .ThenInclude(rhp => rhp.Permission)
            .FirstOrDefaultAsync(r =>
                r.Name.ToLower() == name.ToLower() &&
                r.GuardName == guardName &&
                r.CompanyId == companyId);

    public async Task<List<Role>> GetByGuardAsync(string guardName) =>
        await _dbSet
            .Where(r => r.GuardName == guardName)
            .OrderBy(r => r.Name)
            .ToListAsync();

    public async Task<List<Role>> GetByIdsAsync(List<long> ids) =>
        await _dbSet
            .Include(r => r.RolePermissions)
                .ThenInclude(rhp => rhp.Permission)
            .Where(r => ids.Contains(r.Id))
            .ToListAsync();

    public override Task UpdateAsync(Role role)
    {
        _dbSet.Update(role);
        return Task.CompletedTask;
    }

    public override Task DeleteAsync(Role role)
    {
        role.DeletedAt = DateTime.UtcNow;
        _dbSet.Update(role);
        return Task.CompletedTask;
    }

    public override IQueryable<Role> Query() =>
        _dbSet
            .Include(r => r.RolePermissions)
                .ThenInclude(rhp => rhp.Permission)
            .AsQueryable();

    private IQueryable<Role> ApplyListFilters(IQueryable<Role> query, string? search, string? guardName, long? companyId)
    {
        var normalizedSearch = search?.Trim().ToLower();

        return query
            .Where(r => string.IsNullOrWhiteSpace(normalizedSearch) ||
                r.Name.ToLower().Contains(normalizedSearch) ||
                (r.NameAr != null && r.NameAr.ToLower().Contains(normalizedSearch)) ||
                r.GuardName.ToLower().Contains(normalizedSearch))
            .Where(r => string.IsNullOrWhiteSpace(guardName) || r.GuardName == guardName)
            .Where(r => !companyId.HasValue || r.CompanyId == companyId.Value);
    }

    public async Task<(List<Role> Roles, int TotalCount)> ListAsync(string? search, string? guardName, int page, int pageSize, long? companyId = null)
    {
        var filtered = ApplyListFilters(_dbSet.AsQueryable(), search, guardName, companyId);
        var totalCount = await filtered.CountAsync();
        var roles = await filtered
            .AsNoTracking()
            .Include(r => r.RolePermissions)
                .ThenInclude(rhp => rhp.Permission)
            .OrderByDescending(r => r.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (roles, totalCount);
    }

    public async Task<Role> GetByIdOrThrowAsync(long id) =>
        await _dbSet
            .Include(r => r.RolePermissions)
                .ThenInclude(rhp => rhp.Permission)
            .FirstOrDefaultAsync(r => r.Id == id)
        ?? throw new NotFoundException("roles.not_found_with_id", id);

    public async Task<bool> HasAssignedUsersAsync(long roleId) =>
        await _context.Set<ModelHasRole>()
            .AnyAsync(mhr => mhr.RoleId == roleId);
}

