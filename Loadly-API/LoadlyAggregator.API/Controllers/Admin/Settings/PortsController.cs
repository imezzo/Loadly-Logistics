using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Requests.Admin.Settings.Ports;
using LoadlyAggregator.API.Resources.Admin.Settings;
using LoadlyAggregator.Domain.Admin.Actions.Ports;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Infrastructure.Authorization;

namespace LoadlyAggregator.API.Controllers.Admin.Settings;

[ApiController]
[Route("api/admin/settings/ports")]
[Authorize]
public class PortsController : ControllerBase
{
    private readonly CreatePortAction _createPortAction;
    private readonly GetPortAction _getPortAction;
    private readonly ListPortsAction _listPortsAction;
    private readonly UpdatePortAction _updatePortAction;
    private readonly DeletePortAction _deletePortAction;

    public PortsController(
        CreatePortAction createPortAction,
        GetPortAction getPortAction,
        ListPortsAction listPortsAction,
        UpdatePortAction updatePortAction,
        DeletePortAction deletePortAction)
    {
        _createPortAction = createPortAction;
        _getPortAction = getPortAction;
        _listPortsAction = listPortsAction;
        _updatePortAction = updatePortAction;
        _deletePortAction = deletePortAction;
    }

    [HttpGet]
    [HasPermission(ListPortsRequest.Permission)]
    public async Task<ActionResult> List([FromQuery] ListPortsRequest request) =>
        Ok((await _listPortsAction.ExecuteAsync(request.FromRequest()))
            .ToResourceCollection<PortDto, PortResource>(dto => PortResource.FromDto(dto)));

    [HttpPost]
    [HasPermission(CreatePortRequest.Permission)]
    public async Task<ActionResult> Create([FromBody] CreatePortRequest request)
    {
        await _createPortAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "ports.created_successfully" });
    }

    [HttpGet("{id:long}")]
    [HasPermission(GetPortRequest.Permission)]
    public async Task<ActionResult<PortResource>> Get(long id) =>
        Ok(PortResource.FromDto(await _getPortAction.ExecuteAsync(new GetPortRequest { Id = id }.FromRequest())));

    [HttpPut("{id:long}")]
    [HasPermission(UpdatePortRequest.Permission)]
    public async Task<ActionResult> Update(long id, [FromBody] UpdatePortRequest request)
    {
        request.Id = id;
        await _updatePortAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "ports.updated_successfully" });
    }

    [HttpDelete("{id:long}")]
    [HasPermission(DeletePortRequest.Permission)]
    public async Task<ActionResult> Delete(long id)
    {
        await _deletePortAction.ExecuteAsync(new DeletePortRequest { Id = id }.FromRequest());
        return Ok(new { message = "ports.deleted_successfully" });
    }
}
