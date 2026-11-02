using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IPermissionService
{

    Task<Permission> CreatePermissionAsync(string name, string guardName, long? permissionGroupId = null);
    Task<List<Permission>> GetPermissionsByGuardAsync(string guardName);
    Task<Permission?> GetPermissionByNameAsync(string name, string guardName);

    Task GivePermissionToAsync<T>(T user, string permissionName) where T : class, IHasRoles;
    Task GivePermissionToAsync<T>(T user, long permissionId) where T : class, IHasRoles;
    Task GivePermissionsToAsync<T>(T user, List<string> permissionNames) where T : class, IHasRoles;
    Task GivePermissionsToAsync<T>(T user, List<long> permissionIds) where T : class, IHasRoles;
    Task GiveAllGuardPermissionsAsync<T>(T user) where T : class, IHasRoles;
    Task RevokePermissionFromAsync<T>(T user, string permissionName) where T : class, IHasRoles;
    Task RevokePermissionFromAsync<T>(T user, long permissionId) where T : class, IHasRoles;
    Task SyncPermissionsAsync<T>(T user, List<string> permissionNames) where T : class, IHasRoles;
    Task<bool> HasPermissionToAsync<T>(T user, string permissionName) where T : class, IHasRoles;
    Task<bool> CanAsync<T>(T user, string permissionName) where T : class, IHasRoles;
    Task<bool> HasAnyPermissionAsync<T>(T user, params string[] permissions) where T : class, IHasRoles;
    Task<bool> HasAllPermissionsAsync<T>(T user, params string[] permissions) where T : class, IHasRoles;
    Task<List<string>> GetPermissionNamesAsync<T>(T user) where T : class, IHasRoles;
    Task<List<Permission>> GetPermissionsAsync<T>(T user) where T : class, IHasRoles;
    Task<Dictionary<string, List<string>>> GetPermissionsGroupedByGroupAsync<T>(T user) where T : class, IHasRoles;

    Task<Role> CreateRoleAsync(string name, string guardName);
    Task<Role?> GetRoleByNameAsync(string name, string guardName);
    Task AssignRoleAsync<T>(T user, string roleName) where T : class, IHasRoles;
    Task AssignRoleAsync<T>(T user, long roleId) where T : class, IHasRoles;
    Task AssignRolesAsync<T>(T user, IEnumerable<long> roleIds) where T : class, IHasRoles;
    Task AssignRolesAsync<T>(T user, List<string> roleNames) where T : class, IHasRoles;
    Task RemoveRoleAsync<T>(T user, string roleName) where T : class, IHasRoles;
    Task RemoveRoleAsync<T>(T user, long roleId) where T : class, IHasRoles;
    Task RemoveRolesAsync<T>(T user, IEnumerable<long> roleIds) where T : class, IHasRoles;
    Task SyncRolesAsync<T>(T user, List<string> roleNames) where T : class, IHasRoles;
    Task<bool> HasRoleAsync<T>(T user, string roleName) where T : class, IHasRoles;
    Task<bool> HasAnyRoleAsync<T>(T user, params string[] roles) where T : class, IHasRoles;
    Task<bool> HasAllRolesAsync<T>(T user, params string[] roles) where T : class, IHasRoles;
    Task<List<string>> GetRoleNamesAsync<T>(T user) where T : class, IHasRoles;
    Task<List<Role>> GetRolesAsync<T>(T user) where T : class, IHasRoles;

    Task GivePermissionToRoleAsync(string roleName, string permissionName, string guardName);
    Task GivePermissionToRoleAsync(long roleId, long permissionId);
    Task GivePermissionsToRoleAsync(string roleName, List<string> permissionNames, string guardName);
    Task RevokePermissionFromRoleAsync(string roleName, string permissionName, string guardName);
    Task SyncPermissionsToRoleAsync(string roleName, List<string> permissionNames, string guardName);
    Task SyncPermissionsToRoleAsync(long roleId, List<long> permissionIds);

    void ClearUserCache<T>(T user) where T : class, IHasRoles;
    void ClearAllCache();
    Task ClearCacheForRoleUsersAsync(long roleId);


    Task SyncRolePermissionsToUserAsync<T>(T user) where T : class, IHasRoles;
    Task SyncRolePermissionsAsync(long modelId, string modelType);

    Task AssignRolesIfProvidedAsync<T>(T user, List<long>? roleIds) where T : class, IHasRoles;
    Task UpdateRolesAsync<T>(T user, List<long>? roleIds) where T : class, IHasRoles;

    Task<IReadOnlyList<long>> GetModelIdsWithPermissionAsync(
        string permissionName,
        string guardName,
        string modelType,
        CancellationToken cancellationToken = default);
}
