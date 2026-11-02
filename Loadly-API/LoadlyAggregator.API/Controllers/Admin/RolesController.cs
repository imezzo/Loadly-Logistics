using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Requests.Admin.Roles;
using LoadlyAggregator.API.Resources.Admin.Roles;
using LoadlyAggregator.Domain.Admin.Actions.Roles;
using LoadlyAggregator.Domain.Admin.DTOs.Roles;
using LoadlyAggregator.Infrastructure.Authorization;

namespace LoadlyAggregator.API.Controllers.Admin;

[ApiController]
[Route("api/admin/roles")]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly CreateRoleAction _createRoleAction;
    private readonly GetRoleAction _getRoleAction;
    private readonly ListRolesAction _listRolesAction;
    private readonly UpdateRoleAction _updateRoleAction;
    private readonly DeleteRoleAction _deleteRoleAction;

    public RolesController(
        CreateRoleAction createRoleAction,
        GetRoleAction getRoleAction,
        ListRolesAction listRolesAction,
        UpdateRoleAction updateRoleAction,
        DeleteRoleAction deleteRoleAction)
    {
        _createRoleAction = createRoleAction;
        _getRoleAction = getRoleAction;
        _listRolesAction = listRolesAction;
        _updateRoleAction = updateRoleAction;
        _deleteRoleAction = deleteRoleAction;
    }

    [HttpPost]
    [HasPermission(CreateRoleRequest.Permission)]
    public async Task<ActionResult> Create([FromBody] CreateRoleRequest request)
    {
        await _createRoleAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "roles.created_successfully" });
    }

    [HttpGet("{id}")]
    [HasPermission(GetRoleRequest.Permission)]
    public async Task<ActionResult<RoleResource>> Get(long id) =>
        Ok(RoleResource.FromDto(await _getRoleAction.ExecuteAsync(new GetRoleRequest { Id = id }.FromRequest())));

    [HttpGet]
    [HasPermission(ListRolesRequest.Permission)]
    public async Task<ActionResult> List([FromQuery] ListRolesRequest request) =>
        Ok((await _listRolesAction.ExecuteAsync(request.FromRequest()))
            .ToResourceCollection<RoleDto, RoleResource>(dto => RoleResource.FromDto(dto)));

    [HttpPut("{id}")]
    [HasPermission(UpdateRoleRequest.Permission)]
    public async Task<ActionResult> Update(long id, [FromBody] UpdateRoleRequest request)
    {
        request.Id = id;
        await _updateRoleAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "roles.updated_successfully" });
    }

    [HttpDelete("{id}")]
    [HasPermission(DeleteRoleRequest.Permission)]
    public async Task<ActionResult> Delete(long id)
    {
        var deleteRequest = new DeleteRoleRequest { Id = id };
        await _deleteRoleAction.ExecuteAsync(deleteRequest.FromRequest());
        return Ok(new { message = "roles.deleted_successfully" });
    }
}
