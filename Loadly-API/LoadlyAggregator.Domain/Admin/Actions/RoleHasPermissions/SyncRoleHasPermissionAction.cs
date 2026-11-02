using LoadlyAggregator.Domain.Admin.DTOs.Roles;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.RoleHasPermissions;

public class SyncRoleHasPermissionAction
{
    private readonly IRoleRepository _roleRepository;
    private readonly IRoleHasPermissionRepository _roleHasPermissionRepository;
    private readonly IModelHasRoleRepository _modelHasRoleRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IPermissionService _permissionService;

    public SyncRoleHasPermissionAction(
        IRoleRepository roleRepository,
        IRoleHasPermissionRepository roleHasPermissionRepository,
        IModelHasRoleRepository modelHasRoleRepository,
        IPermissionRepository permissionRepository,
        IPermissionService permissionService)
    {
        _roleRepository = roleRepository;
        _roleHasPermissionRepository = roleHasPermissionRepository;
        _modelHasRoleRepository = modelHasRoleRepository;
        _permissionRepository = permissionRepository;
        _permissionService = permissionService;
    }

    public async Task<List<object>> ExecuteAsync(SyncRoleHasPermissionDto dto)
    {
        var role = await _roleRepository.GetByIdOrThrowAsync(dto.RoleId);
        if ((role.GuardName != "admin" && role.GuardName != "collector") || role.CompanyId != null)
        {
            throw new NotFoundException("roles.not_found");
        }

        await ValidateConflictingPermissionsAsync(dto.PermissionIds);

        await _permissionService.SyncPermissionsToRoleAsync(dto.RoleId, dto.PermissionIds);

        var modelsWithRole = await _modelHasRoleRepository.GetByRoleIdAsync(dto.RoleId);
        if (modelsWithRole.Any())
        {
            var groupedByModelType = modelsWithRole.GroupBy(mhr => mhr.ModelType);
            foreach (var group in groupedByModelType)
            {
                var modelType = group.Key;
                var modelIds = group.Select(mhr => mhr.ModelId).Distinct().ToList();
                foreach (var modelId in modelIds)
                {
                    await _permissionService.SyncRolePermissionsAsync(modelId, modelType);
                }
            }
        }

        await _permissionService.ClearCacheForRoleUsersAsync(dto.RoleId);

        var roleHasPermissionsWithNav = await _roleHasPermissionRepository.GetByRoleIdAndPermissionIdsAsync(
            dto.RoleId,
            dto.PermissionIds);
        if (roleHasPermissionsWithNav.Count != dto.PermissionIds.Count)
        {
            throw new NotFoundException("Some role-permission relationships not found after creation");
        }

        var responses = roleHasPermissionsWithNav
            .Select(rhp => new
            {
                RoleId = rhp.RoleId,
                PermissionId = rhp.PermissionId
            })
            .ToList<object>();
        return responses;
    }

    private async Task ValidateConflictingPermissionsAsync(List<long> permissionIds)
    {
        var permissions = await _permissionRepository.GetByIdsAsync(permissionIds);
        var permissionNames = permissions.Select(p => p.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (permissionNames.Contains("collect_trip_documents")
            && permissionNames.Contains("confirm_trip_documents"))
        {
            throw new ValidationException("roles.conflict_permissions", "collect_trip_documents, confirm_trip_documents");
        }
    }
}
