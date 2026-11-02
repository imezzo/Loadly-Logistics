using System.Linq;
namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IPermissionRepository : IRepository<Common.Entities.Permission>
{
    Task<Common.Entities.Permission?> GetByNameAndGuardAsync(string name, string guardName);
    Task<List<Common.Entities.Permission>> GetByGuardAsync(string guardName);
    Task<List<Common.Entities.Permission>> GetByIdsAsync(List<long> ids);
    Task<Common.Entities.Permission> GetByIdOrThrowAsync(long id);
}
