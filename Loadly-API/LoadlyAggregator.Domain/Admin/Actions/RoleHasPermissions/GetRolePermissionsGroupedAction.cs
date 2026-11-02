using System.Linq;
using LoadlyAggregator.Domain.Admin.DTOs.Roles;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.RoleHasPermissions;

public class GetRolePermissionsGroupedAction : IAction<GetRolePermissionsDto, object>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IRoleHasPermissionRepository _roleHasPermissionRepository;

    public GetRolePermissionsGroupedAction(
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository,
        IRoleHasPermissionRepository roleHasPermissionRepository)
    {
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _roleHasPermissionRepository = roleHasPermissionRepository;
    }

    public async Task<object> ExecuteAsync(GetRolePermissionsDto dto)
    {
        var role = await _roleRepository.GetByIdAsync(dto.RoleId)
            ?? throw new NotFoundException("roles.not_found_with_id", dto.RoleId);

        if (role.GuardName != "admin" || role.CompanyId != null)
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
                      .ToList<object>()
            );

        return new { Permissions = grouped };
    }
}

