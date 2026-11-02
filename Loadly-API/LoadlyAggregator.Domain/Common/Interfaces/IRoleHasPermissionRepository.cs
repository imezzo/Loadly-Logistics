using System.Linq;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IRoleHasPermissionRepository : IRepository<Common.Entities.RoleHasPermission>
{
    Task<Common.Entities.RoleHasPermission?> GetByRoleIdAndPermissionIdAsync(long roleId, long permissionId);
    Task<List<Common.Entities.RoleHasPermission>> GetByRoleIdAndPermissionIdsAsync(long roleId, List<long> permissionIds);
    Task<List<Common.Entities.RoleHasPermission>> GetByRoleIdAsync(long roleId);
    Task<List<Common.Entities.RoleHasPermission>> GetByPermissionIdAsync(long permissionId);

    Task<List<Common.Entities.Permission>> GetNonDeletedPermissionsByRoleIdAsync(long roleId);

    Task<List<long>> GetPermissionIdsByRoleIdAsync(long roleId);
}

