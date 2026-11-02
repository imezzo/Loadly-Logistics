using System.Linq;
namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IModelHasPermissionRepository : IRepository<Common.Entities.ModelHasPermission>
{
    Task<Common.Entities.ModelHasPermission?> GetByPermissionIdAndModelIdAndModelTypeAsync(long permissionId, long modelId, string modelType);
    Task<List<Common.Entities.ModelHasPermission>> GetByModelIdAndModelTypeAsync(long modelId, string modelType);
    Task<List<Common.Entities.ModelHasPermission>> GetByPermissionIdAsync(long permissionId);
    Task<(List<Common.Entities.ModelHasPermission> Items, int TotalCount)> ListAsync(long? permissionId, long? modelId, string? modelType, int page, int pageSize);
    Task<Common.Entities.ModelHasPermission> GetByPermissionIdAndModelIdAndModelTypeOrThrowAsync(long permissionId, long modelId, string modelType);
}
