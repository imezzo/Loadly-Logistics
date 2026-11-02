using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Helpers;
using LoadlyAggregator.API.Requests.Company.RoleHasPermissions;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Company.Actions.RoleHasPermissions;
using LoadlyAggregator.Infrastructure.Authorization;

namespace LoadlyAggregator.API.Controllers.Company.RoleHasPermissions;

[ApiController]
[Route("api/company/role-has-permissions")]
[Authorize]
public class CompanyRoleHasPermissionsController : ControllerBase
{
    private readonly GetCompanyRolePermissionsGroupedAction _getCompanyRolePermissionsGroupedAction;
    private readonly SyncCompanyRoleHasPermissionAction _syncCompanyRoleHasPermissionAction;
    private readonly ICompanyRepository _companyRepository;

    public CompanyRoleHasPermissionsController(
        GetCompanyRolePermissionsGroupedAction getCompanyRolePermissionsGroupedAction,
        SyncCompanyRoleHasPermissionAction syncCompanyRoleHasPermissionAction,
        ICompanyRepository companyRepository)
    {
        _getCompanyRolePermissionsGroupedAction = getCompanyRolePermissionsGroupedAction;
        _syncCompanyRoleHasPermissionAction = syncCompanyRoleHasPermissionAction;
        _companyRepository = companyRepository;
    }

    [HttpGet("{roleId}/grouped")]
    [HasPermission(GetCompanyRolePermissionsRequest.Permission)]
    public async Task<ActionResult<object>> GetRolePermissions(long roleId)
    {
        var response = await _getCompanyRolePermissionsGroupedAction.ExecuteAsync(
            new GetCompanyRolePermissionsRequest().FromRequest(
                roleId,
                ClaimsHelper.GetCompanyId(User)));
        return Ok(response);
    }

    [HttpPost("{roleId}/sync")]
    [HasPermission(SyncCompanyRoleHasPermissionRequest.Permission)]
    public async Task<ActionResult> Sync(long roleId, [FromBody] SyncCompanyRoleHasPermissionRequest request)
    {
        await _syncCompanyRoleHasPermissionAction.ExecuteAsync(request.FromRequest(
            roleId,
            ClaimsHelper.GetCompanyId(User)));
        return Ok(new { message = "roles.permissions_synced_successfully" });
    }
}
