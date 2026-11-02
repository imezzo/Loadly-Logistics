using System.Linq;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IModelHasRoleRepository : IRepository<Common.Entities.ModelHasRole>
{
    Task<Common.Entities.ModelHasRole?> GetByRoleIdAndModelIdAndModelTypeAsync(long roleId, long modelId, string modelType);
    Task<List<Common.Entities.ModelHasRole>> GetByModelIdAndModelTypeAsync(long modelId, string modelType);
    Task<List<Common.Entities.ModelHasRole>> GetByRoleIdAsync(long roleId);

    Task<List<Common.Entities.ModelHasRole>> GetByModelIdAndModelTypeWithRolePermissionsAsync(
        long modelId,
        string modelType,
        long? excludeRoleId = null);

    Task<(List<Common.Entities.ModelHasRole> Items, int TotalCount)> ListAsync(long? roleId, long? modelId, string? modelType, int page, int pageSize);
    Task<Common.Entities.ModelHasRole> GetByRoleIdAndModelIdAndModelTypeOrThrowAsync(long roleId, long modelId, string modelType);
}

