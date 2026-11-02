using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Requests.Admin.RoleHasPermissions;
using LoadlyAggregator.Domain.Admin.Actions.RoleHasPermissions;
using LoadlyAggregator.Infrastructure.Authorization;

namespace LoadlyAggregator.API.Controllers.Admin;

[ApiController]
[Route("api/admin/role-has-permissions")]
[Authorize]
public class RoleHasPermissionsController : ControllerBase
{
    private readonly GetRolePermissionsGroupedAction _getRolePermissionsGroupedAction;
    private readonly SyncRoleHasPermissionAction _syncRoleHasPermissionAction;

    public RoleHasPermissionsController(
        GetRolePermissionsGroupedAction getRolePermissionsGroupedAction,
        SyncRoleHasPermissionAction syncRoleHasPermissionAction)
    {
        _getRolePermissionsGroupedAction = getRolePermissionsGroupedAction;
        _syncRoleHasPermissionAction = syncRoleHasPermissionAction;
    }

    [HttpGet("{roleId}/grouped")]
    [HasPermission(GetRolePermissionsRequest.Permission)]
    public async Task<ActionResult<object>> GetRolePermissions(long roleId)
    {
        var getRequest = new GetRolePermissionsRequest { RoleId = roleId };
        var response = await _getRolePermissionsGroupedAction.ExecuteAsync(getRequest.FromRequest());
        return Ok(response);
    }

    [HttpPost("{roleId}/sync")]
    [HasPermission(SyncRoleHasPermissionRequest.Permission)]
    public async Task<ActionResult> Sync(long roleId, [FromBody] SyncRoleHasPermissionRequest request)
    {
        await _syncRoleHasPermissionAction.ExecuteAsync(request.FromRequest(roleId));
        return Ok(new { message = "role_has_permissions.synced_successfully" });
    }
}
