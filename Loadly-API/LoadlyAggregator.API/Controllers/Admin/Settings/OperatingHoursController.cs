using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Requests.Admin.Settings.OperatingHours;
using LoadlyAggregator.API.Resources.Admin.Settings;
using LoadlyAggregator.Domain.Admin.Actions.OperatingHours;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Infrastructure.Authorization;

namespace LoadlyAggregator.API.Controllers.Admin.Settings;

[ApiController]
[Route("api/admin/settings/operating-hours")]
[Authorize]
public class OperatingHoursController : ControllerBase
{
    private readonly CreateOperatingHoursAction _createOperatingHoursAction;
    private readonly GetOperatingHoursAction _getOperatingHoursAction;
    private readonly ListOperatingHoursAction _listOperatingHoursAction;
    private readonly UpdateOperatingHoursAction _updateOperatingHoursAction;
    private readonly DeleteOperatingHoursAction _deleteOperatingHoursAction;

    public OperatingHoursController(
        CreateOperatingHoursAction createOperatingHoursAction,
        GetOperatingHoursAction getOperatingHoursAction,
        ListOperatingHoursAction listOperatingHoursAction,
        UpdateOperatingHoursAction updateOperatingHoursAction,
        DeleteOperatingHoursAction deleteOperatingHoursAction)
    {
        _createOperatingHoursAction = createOperatingHoursAction;
        _getOperatingHoursAction = getOperatingHoursAction;
        _listOperatingHoursAction = listOperatingHoursAction;
        _updateOperatingHoursAction = updateOperatingHoursAction;
        _deleteOperatingHoursAction = deleteOperatingHoursAction;
    }

    [HttpGet]
    [HasPermission(ListOperatingHoursRequest.Permission)]
    public async Task<ActionResult> List([FromQuery] ListOperatingHoursRequest request) =>
        Ok((await _listOperatingHoursAction.ExecuteAsync(request.FromRequest()))
            .ToResourceCollection<OperatingHoursDto, OperatingHoursResource>(dto => OperatingHoursResource.FromDto(dto)));

    [HttpPost]
    [HasPermission(CreateOperatingHoursRequest.Permission)]
    public async Task<ActionResult> Create([FromBody] CreateOperatingHoursRequest request)
    {
        await _createOperatingHoursAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "operating_hours.created_successfully" });
    }

    [HttpGet("{id}")]
    [HasPermission(GetOperatingHoursRequest.Permission)]
    public async Task<ActionResult<OperatingHoursResource>> Get(long id) =>
        Ok(OperatingHoursResource.FromDto(await _getOperatingHoursAction.ExecuteAsync(new GetOperatingHoursRequest { Id = id }.FromRequest())));

    [HttpPut("{id}")]
    [HasPermission(UpdateOperatingHoursRequest.Permission)]
    public async Task<ActionResult> Update(long id, [FromBody] UpdateOperatingHoursRequest request)
    {
        request.Id = id;
        await _updateOperatingHoursAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "operating_hours.updated_successfully" });
    }

    [HttpDelete("{id}")]
    [HasPermission(DeleteOperatingHoursRequest.Permission)]
    public async Task<ActionResult> Delete(long id)
    {
        await _deleteOperatingHoursAction.ExecuteAsync(new DeleteOperatingHoursRequest { Id = id }.FromRequest());
        return Ok(new { message = "operating_hours.deleted_successfully" });
    }
}
