using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Requests.Admin.Admins;
using LoadlyAggregator.API.Resources.Admin.Admins;
using LoadlyAggregator.Domain.Admin.Actions.Admins;
using LoadlyAggregator.Domain.Admin.DTOs.Admins;
using LoadlyAggregator.Infrastructure.Authorization;

namespace LoadlyAggregator.API.Controllers.Admin.Admins;

[ApiController]
[Route("api/admin/admins")]
[Authorize]
public class AdminsController : ControllerBase
{
    private readonly CreateAdminAction _createAdminAction;
    private readonly GetAdminAction _getAdminAction;
    private readonly ListAdminsAction _listAdminsAction;
    private readonly UpdateAdminAction _updateAdminAction;
    private readonly DeleteAdminAction _deleteAdminAction;
    private readonly BulkUpdateAdminStatusAction _bulkUpdateAdminStatusAction;

    public AdminsController(
        CreateAdminAction createAdminAction,
        GetAdminAction getAdminAction,
        ListAdminsAction listAdminsAction,
        UpdateAdminAction updateAdminAction,
        DeleteAdminAction deleteAdminAction,
        BulkUpdateAdminStatusAction bulkUpdateAdminStatusAction)
    {
        _createAdminAction = createAdminAction;
        _getAdminAction = getAdminAction;
        _listAdminsAction = listAdminsAction;
        _updateAdminAction = updateAdminAction;
        _deleteAdminAction = deleteAdminAction;
        _bulkUpdateAdminStatusAction = bulkUpdateAdminStatusAction;
    }

    [HttpGet]
    [HasPermission("view_admin")]
    public async Task<ActionResult> List([FromQuery] ListAdminsRequest request) =>
        Ok((await _listAdminsAction.ExecuteAsync(request.FromRequest()))
            .ToResourceCollection<AdminDto, AdminResource>(dto => AdminResource.FromDto(dto)));

    [HttpPost]
    [HasPermission("create_admin")]
    public async Task<ActionResult> Create([FromBody] CreateAdminRequest request)
    {
        await _createAdminAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "admins.created_successfully" });
    }

    [HttpGet("{id}")]
    [HasPermission("view_admin")]
    public async Task<ActionResult<AdminResource>> Get(long id) =>
        Ok(AdminResource.FromDto(await _getAdminAction.ExecuteAsync(new GetAdminRequest { Id = id }.FromRequest())));

    [HttpPut("{id}")]
    [HasPermission("edit_admin")]
    public async Task<ActionResult> Update(long id, [FromBody] UpdateAdminRequest request)
    {
        await _updateAdminAction.ExecuteAsync(request.FromRequest(id));
        return Ok(new { message = "admins.updated_successfully" });
    }

    [HttpDelete("{id}")]
    [HasPermission("delete_admin")]
    public async Task<ActionResult> Delete(long id)
    {
        await _deleteAdminAction.ExecuteAsync(new DeleteAdminRequest { Id = id }.FromRequest());
        return Ok(new { message = "admins.deleted_successfully" });
    }

    [HttpPost("activate")]
    [HasPermission("edit_admin")]
    public async Task<ActionResult> Activate([FromBody] BulkUpdateAdminStatusIdsRequest request)
    {
        await _bulkUpdateAdminStatusAction.ExecuteAsync(request.FromRequest(1));
        return Ok(new { message = "admins.activated_successfully" });
    }

    [HttpPost("deactivate")]
    [HasPermission("edit_admin")]
    public async Task<ActionResult> Deactivate([FromBody] BulkUpdateAdminStatusIdsRequest request)
    {
        await _bulkUpdateAdminStatusAction.ExecuteAsync(request.FromRequest(0));
        return Ok(new { message = "admins.deactivated_successfully" });
    }
}
