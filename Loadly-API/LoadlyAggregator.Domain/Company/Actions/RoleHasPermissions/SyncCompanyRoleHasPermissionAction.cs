using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Company.DTOs.RoleHasPermissions;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Company.Actions.RoleHasPermissions;

public class SyncCompanyRoleHasPermissionAction
{
    private readonly IRoleRepository _roleRepository;
    private readonly IModelHasRoleRepository _modelHasRoleRepository;
    private readonly IPermissionService _permissionService;

    public SyncCompanyRoleHasPermissionAction(
        IRoleRepository roleRepository,
        IModelHasRoleRepository modelHasRoleRepository,
        IPermissionService permissionService)
    {
        _roleRepository = roleRepository;
        _modelHasRoleRepository = modelHasRoleRepository;
        _permissionService = permissionService;
    }

    public async Task ExecuteAsync(SyncCompanyRoleHasPermissionDto dto)
    {
        var role = await _roleRepository.GetByIdOrThrowAsync(dto.RoleId);

        if (role.CompanyId != dto.CompanyId)
        {
            throw new NotFoundException("roles.not_found");
        }

        await _permissionService.SyncPermissionsToRoleAsync(role.Id, dto.PermissionIds);

        var modelsWithRole = await _modelHasRoleRepository.GetByRoleIdAsync(role.Id);
        foreach (var model in modelsWithRole)
        {
            await _permissionService.SyncRolePermissionsAsync(model.ModelId, model.ModelType);
        }

        await _permissionService.ClearCacheForRoleUsersAsync(role.Id);
    }
}
