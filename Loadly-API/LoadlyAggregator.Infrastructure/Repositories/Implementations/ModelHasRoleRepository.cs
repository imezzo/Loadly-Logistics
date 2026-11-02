using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class ModelHasRoleRepository : Repository<ModelHasRole>, IModelHasRoleRepository
{
    public ModelHasRoleRepository(ApplicationDbContext context) : base(context)
    {
    }

    public Task<ModelHasRole?> GetByRoleIdAndModelIdAndModelTypeAsync(long roleId, long modelId, string modelType) =>
        _dbSet
            .Include(mhr => mhr.Role)
            .FirstOrDefaultAsync(mhr => mhr.RoleId == roleId && mhr.ModelId == modelId && mhr.ModelType == modelType);

    public Task<List<ModelHasRole>> GetByModelIdAndModelTypeAsync(long modelId, string modelType) =>
        _dbSet
            .Include(mhr => mhr.Role)
            .Where(mhr => mhr.ModelId == modelId && mhr.ModelType == modelType)
            .ToListAsync();

    public Task<List<ModelHasRole>> GetByRoleIdAsync(long roleId) =>
        _dbSet
            .Include(mhr => mhr.Role)
            .Where(mhr => mhr.RoleId == roleId)
            .ToListAsync();

    public override Task DeleteAsync(ModelHasRole modelHasRole)
    {
        _dbSet.Remove(modelHasRole);
        return Task.CompletedTask;
    }

    public override IQueryable<ModelHasRole> Query() =>
        _dbSet.Include(mhr => mhr.Role).AsQueryable();

    public Task<List<ModelHasRole>> GetByModelIdAndModelTypeWithRolePermissionsAsync(
        long modelId,
        string modelType,
        long? excludeRoleId = null) =>
        _dbSet
            .Where(mhr => mhr.ModelId == modelId && mhr.ModelType == modelType)
            .Where(mhr => !excludeRoleId.HasValue || mhr.RoleId != excludeRoleId.Value)
            .Include(mhr => mhr.Role)
                .ThenInclude(r => r.RolePermissions)
            .ToListAsync();

    public async Task<(List<ModelHasRole> Items, int TotalCount)> ListAsync(long? roleId, long? modelId, string? modelType, int page, int pageSize) =>
        (await _dbSet
            .AsNoTracking()
            .Include(mhr => mhr.Role)
            .Where(mhr => !roleId.HasValue || mhr.RoleId == roleId.Value)
            .Where(mhr => !modelId.HasValue || mhr.ModelId == modelId.Value)
            .Where(mhr => string.IsNullOrWhiteSpace(modelType) || mhr.ModelType == modelType)
            .OrderBy(mhr => mhr.RoleId)
            .ThenBy(mhr => mhr.ModelId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(),
        await _dbSet
            .Where(mhr => !roleId.HasValue || mhr.RoleId == roleId.Value)
            .Where(mhr => !modelId.HasValue || mhr.ModelId == modelId.Value)
            .Where(mhr => string.IsNullOrWhiteSpace(modelType) || mhr.ModelType == modelType)
            .CountAsync());

    public async Task<ModelHasRole> GetByRoleIdAndModelIdAndModelTypeOrThrowAsync(long roleId, long modelId, string modelType) =>
        await GetByRoleIdAndModelIdAndModelTypeAsync(roleId, modelId, modelType)
            ?? throw new NotFoundException("model_has_roles.not_found_with_composite", roleId, modelId, modelType);
}
