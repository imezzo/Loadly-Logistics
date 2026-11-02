using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class ModelHasPermissionRepository : Repository<ModelHasPermission>, IModelHasPermissionRepository
{
    public ModelHasPermissionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public Task<ModelHasPermission?> GetByPermissionIdAndModelIdAndModelTypeAsync(long permissionId, long modelId, string modelType) =>
        _dbSet
            .Include(mhp => mhp.Permission)
            .FirstOrDefaultAsync(mhp => mhp.PermissionId == permissionId && mhp.ModelId == modelId && mhp.ModelType == modelType);

    public Task<List<ModelHasPermission>> GetByModelIdAndModelTypeAsync(long modelId, string modelType) =>
        _dbSet
            .Include(mhp => mhp.Permission)
            .Where(mhp => mhp.ModelId == modelId && mhp.ModelType == modelType)
            .ToListAsync();

    public Task<List<ModelHasPermission>> GetByPermissionIdAsync(long permissionId) =>
        _dbSet
            .Include(mhp => mhp.Permission)
            .Where(mhp => mhp.PermissionId == permissionId)
            .ToListAsync();

    public override Task DeleteAsync(ModelHasPermission modelHasPermission)
    {
        _dbSet.Remove(modelHasPermission);
        return Task.CompletedTask;
    }

    public override IQueryable<ModelHasPermission> Query() =>
        _dbSet.Include(mhp => mhp.Permission).AsQueryable();

    public async Task<(List<ModelHasPermission> Items, int TotalCount)> ListAsync(long? permissionId, long? modelId, string? modelType, int page, int pageSize) =>
        (await _dbSet
            .AsNoTracking()
            .Include(mhp => mhp.Permission)
            .Where(mhp => !permissionId.HasValue || mhp.PermissionId == permissionId.Value)
            .Where(mhp => !modelId.HasValue || mhp.ModelId == modelId.Value)
            .Where(mhp => string.IsNullOrWhiteSpace(modelType) || mhp.ModelType == modelType)
            .OrderBy(mhp => mhp.PermissionId)
            .ThenBy(mhp => mhp.ModelId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(),
        await _dbSet
            .Where(mhp => !permissionId.HasValue || mhp.PermissionId == permissionId.Value)
            .Where(mhp => !modelId.HasValue || mhp.ModelId == modelId.Value)
            .Where(mhp => string.IsNullOrWhiteSpace(modelType) || mhp.ModelType == modelType)
            .CountAsync());

    public async Task<ModelHasPermission> GetByPermissionIdAndModelIdAndModelTypeOrThrowAsync(long permissionId, long modelId, string modelType) =>
        await GetByPermissionIdAndModelIdAndModelTypeAsync(permissionId, modelId, modelType)
            ?? throw new NotFoundException("model_has_permissions.not_found_with_composite", permissionId, modelId, modelType);
}
