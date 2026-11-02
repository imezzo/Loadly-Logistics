using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Requests.Admin.Settings.ShippingLines;
using LoadlyAggregator.API.Resources.Admin.Settings;
using LoadlyAggregator.Domain.Admin.Actions.ShippingLines;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Infrastructure.Authorization;

namespace LoadlyAggregator.API.Controllers.Admin.Settings;

[ApiController]
[Route("api/admin/settings/shipping-lines")]
[Authorize]
public class ShippingLinesController : ControllerBase
{
    private readonly CreateShippingLineAction _createShippingLineAction;
    private readonly GetShippingLineAction _getShippingLineAction;
    private readonly ListShippingLinesAction _listShippingLinesAction;
    private readonly UpdateShippingLineAction _updateShippingLineAction;
    private readonly DeleteShippingLineAction _deleteShippingLineAction;

    public ShippingLinesController(
        CreateShippingLineAction createShippingLineAction,
        GetShippingLineAction getShippingLineAction,
        ListShippingLinesAction listShippingLinesAction,
        UpdateShippingLineAction updateShippingLineAction,
        DeleteShippingLineAction deleteShippingLineAction)
    {
        _createShippingLineAction = createShippingLineAction;
        _getShippingLineAction = getShippingLineAction;
        _listShippingLinesAction = listShippingLinesAction;
        _updateShippingLineAction = updateShippingLineAction;
        _deleteShippingLineAction = deleteShippingLineAction;
    }

    [HttpGet]
    [HasPermission(ListShippingLinesRequest.Permission)]
    public async Task<ActionResult> List([FromQuery] ListShippingLinesRequest request) =>
        Ok((await _listShippingLinesAction.ExecuteAsync(request.FromRequest()))
            .ToResourceCollection<ShippingLineDto, ShippingLineResource>(dto => ShippingLineResource.FromDto(dto)));

    [HttpPost]
    [HasPermission(CreateShippingLineRequest.Permission)]
    public async Task<ActionResult> Create([FromBody] CreateShippingLineRequest request)
    {
        await _createShippingLineAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "shipping_lines.created_successfully" });
    }

    [HttpGet("{id:long}")]
    [HasPermission(GetShippingLineRequest.Permission)]
    public async Task<ActionResult<ShippingLineResource>> Get(long id) =>
        Ok(ShippingLineResource.FromDto(await _getShippingLineAction.ExecuteAsync(new GetShippingLineRequest { Id = id }.FromRequest())));

    [HttpPut("{id:long}")]
    [HasPermission(UpdateShippingLineRequest.Permission)]
    public async Task<ActionResult> Update(long id, [FromBody] UpdateShippingLineRequest request)
    {
        request.Id = id;
        await _updateShippingLineAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "shipping_lines.updated_successfully" });
    }

    [HttpDelete("{id:long}")]
    [HasPermission(DeleteShippingLineRequest.Permission)]
    public async Task<ActionResult> Delete(long id)
    {
        await _deleteShippingLineAction.ExecuteAsync(new DeleteShippingLineRequest { Id = id }.FromRequest());
        return Ok(new { message = "shipping_lines.deleted_successfully" });
    }
}
