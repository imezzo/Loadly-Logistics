using System.Linq;
namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IRoleRepository : IRepository<Common.Entities.Role>
{
    Task<Common.Entities.Role?> GetByNameAndGuardAsync(string name, string guardName);
    Task<Common.Entities.Role?> GetByNameAndGuardAndCompanyAsync(string name, string guardName, long? companyId);
    Task<List<Common.Entities.Role>> GetByGuardAsync(string guardName);
    Task<List<Common.Entities.Role>> GetByIdsAsync(List<long> ids);

    Task<(List<Common.Entities.Role> Roles, int TotalCount)> ListAsync(string? search, string? guardName, int page, int pageSize, long? companyId = null);
    Task<Common.Entities.Role> GetByIdOrThrowAsync(long id);
    Task<bool> HasAssignedUsersAsync(long roleId);
}
