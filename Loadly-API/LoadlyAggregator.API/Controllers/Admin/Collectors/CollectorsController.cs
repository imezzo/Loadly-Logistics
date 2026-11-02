using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Requests.Admin.Collectors;
using LoadlyAggregator.API.Resources.Admin.Collectors;
using LoadlyAggregator.Domain.Admin.Actions.Collectors;
using LoadlyAggregator.Domain.Admin.DTOs.Collectors;
using LoadlyAggregator.Infrastructure.Authorization;

namespace LoadlyAggregator.API.Controllers.Admin.Collectors;

[ApiController]
[Route("api/admin/collectors")]
[Authorize]
public class AdminsController : ControllerBase
{
    private readonly CreateCollectorAction _createAdminAction;
    private readonly GetCollectorAction _getAdminAction;
    private readonly ListCollectorsAction _listAdminsAction;
    private readonly UpdateCollectorAction _updateAdminAction;
    private readonly DeleteCollectorAction _deleteAdminAction;
    private readonly BulkUpdateCollectorStatusAction _bulkUpdateCollectorStatusAction;

    public AdminsController(
        CreateCollectorAction createAdminAction,
        GetCollectorAction getAdminAction,
        ListCollectorsAction listAdminsAction,
        UpdateCollectorAction updateAdminAction,
        DeleteCollectorAction deleteAdminAction,
        BulkUpdateCollectorStatusAction bulkUpdateCollectorStatusAction)
    {
        _createAdminAction = createAdminAction;
        _getAdminAction = getAdminAction;
        _listAdminsAction = listAdminsAction;
        _updateAdminAction = updateAdminAction;
        _deleteAdminAction = deleteAdminAction;
        _bulkUpdateCollectorStatusAction = bulkUpdateCollectorStatusAction;
    }

    [HttpGet]
    [HasPermission("view_collector")]
    public async Task<ActionResult> List([FromQuery] ListCollectorsRequest request) =>
        Ok((await _listAdminsAction.ExecuteAsync(request.FromRequest()))
            .ToResourceCollection<CollectorDto, CollectorResource>(dto => CollectorResource.FromDto(dto)));

    [HttpPost]
    [HasPermission("create_collector")]
    public async Task<ActionResult> Create([FromBody] CreateCollectorRequest request)
    {
        await _createAdminAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "collectors.created_successfully" });
    }

    [HttpGet("{id}")]
    [HasPermission("view_collector")]
    public async Task<ActionResult<CollectorResource>> Get(long id) =>
        Ok(CollectorResource.FromDto(await _getAdminAction.ExecuteAsync(new GetCollectorRequest { Id = id }.FromRequest())));

    [HttpPut("{id}")]
    [HasPermission("edit_collector")]
    public async Task<ActionResult> Update(long id, [FromBody] UpdateCollectorRequest request)
    {
        await _updateAdminAction.ExecuteAsync(request.FromRequest(id));
        return Ok(new { message = "collectors.updated_successfully" });
    }

    [HttpDelete("{id}")]
    [HasPermission("delete_collector")]
    public async Task<ActionResult> Delete(long id)
    {
        await _deleteAdminAction.ExecuteAsync(new DeleteCollectorRequest { Id = id }.FromRequest());
        return Ok(new { message = "collectors.deleted_successfully" });
    }

    [HttpPost("activate")]
    [HasPermission("edit_collector")]
    public async Task<ActionResult> Activate([FromBody] BulkUpdateCollectorStatusIdsRequest request)
    {
        await _bulkUpdateCollectorStatusAction.ExecuteAsync(request.FromRequest(1));
        return Ok(new { message = "collectors.activated_successfully" });
    }

    [HttpPost("deactivate")]
    [HasPermission("edit_collector")]
    public async Task<ActionResult> Deactivate([FromBody] BulkUpdateCollectorStatusIdsRequest request)
    {
        await _bulkUpdateCollectorStatusAction.ExecuteAsync(request.FromRequest(0));
        return Ok(new { message = "collectors.deactivated_successfully" });
    }
}
