using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Company.DTOs.RoleHasPermissions;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Company.Actions.RoleHasPermissions;

public class GetCompanyRolePermissionsGroupedAction
{
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IRoleHasPermissionRepository _roleHasPermissionRepository;

    public GetCompanyRolePermissionsGroupedAction(
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository,
        IRoleHasPermissionRepository roleHasPermissionRepository)
    {
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _roleHasPermissionRepository = roleHasPermissionRepository;
    }

    public async Task<object> ExecuteAsync(GetCompanyRolePermissionsDto dto)
    {
        var role = await _roleRepository.GetByIdOrThrowAsync(dto.RoleId);

        if (role.CompanyId != dto.CompanyId)
        {
            throw new NotFoundException("roles.not_found");
        }

        var allPermissions = await _permissionRepository.GetByGuardAsync(role.GuardName);
        var assignedPermissionIds = (await _roleHasPermissionRepository.GetPermissionIdsByRoleIdAsync(dto.RoleId)).ToHashSet();

        var grouped = allPermissions
            .GroupBy(p => p.PermissionGroup?.Name ?? "uncategorized")
            .ToDictionary(
                g => g.Key,
                g => g.OrderBy(p => p.Name)
                    .Select(p => new
                    {
                        Id = p.Id,
                        Name = p.Name,
                        GuardName = p.GuardName,
                        PermissionGroupId = p.PermissionGroupId,
                        IsAssigned = assignedPermissionIds.Contains(p.Id)
                    })
                    .ToList<object>());

        return new
        {
            RoleId = role.Id,
            Permissions = grouped
        };
    }
}
