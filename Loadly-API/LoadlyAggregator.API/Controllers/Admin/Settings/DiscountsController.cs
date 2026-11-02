using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Requests.Admin.Settings.Discounts;
using LoadlyAggregator.API.Resources.Admin.Settings;
using LoadlyAggregator.Domain.Admin.Actions.Discounts;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Infrastructure.Authorization;

namespace LoadlyAggregator.API.Controllers.Admin.Settings;

[ApiController]
[Route("api/admin/settings/discounts")]
[Authorize]
public class DiscountsController : ControllerBase
{
    private readonly CreateDiscountAction _createDiscountAction;
    private readonly GetDiscountAction _getDiscountAction;
    private readonly ListDiscountsAction _listDiscountsAction;
    private readonly UpdateDiscountAction _updateDiscountAction;
    private readonly DeleteDiscountAction _deleteDiscountAction;

    public DiscountsController(
        CreateDiscountAction createDiscountAction,
        GetDiscountAction getDiscountAction,
        ListDiscountsAction listDiscountsAction,
        UpdateDiscountAction updateDiscountAction,
        DeleteDiscountAction deleteDiscountAction)
    {
        _createDiscountAction = createDiscountAction;
        _getDiscountAction = getDiscountAction;
        _listDiscountsAction = listDiscountsAction;
        _updateDiscountAction = updateDiscountAction;
        _deleteDiscountAction = deleteDiscountAction;
    }

    [HttpGet]
    [HasPermission(ListDiscountsRequest.Permission)]
    public async Task<ActionResult> List([FromQuery] ListDiscountsRequest request) =>
        Ok((await _listDiscountsAction.ExecuteAsync(request.FromRequest()))
            .ToResourceCollection<DiscountDto, DiscountResource>(dto => DiscountResource.FromDto(dto)));

    [HttpPost]
    [HasPermission(CreateDiscountRequest.Permission)]
    public async Task<ActionResult> Create([FromBody] CreateDiscountRequest request)
    {
        await _createDiscountAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "discounts.created_successfully" });
    }

    [HttpGet("{id}")]
    [HasPermission(GetDiscountRequest.Permission)]
    public async Task<ActionResult<DiscountResource>> Get(long id) =>
        Ok(DiscountResource.FromDto(await _getDiscountAction.ExecuteAsync(new GetDiscountRequest { Id = id }.FromRequest())));

    [HttpPut("{id}")]
    [HasPermission(UpdateDiscountRequest.Permission)]
    public async Task<ActionResult> Update(long id, [FromBody] UpdateDiscountRequest request)
    {
        request.Id = id;
        await _updateDiscountAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "discounts.updated_successfully" });
    }

    [HttpDelete("{id}")]
    [HasPermission(DeleteDiscountRequest.Permission)]
    public async Task<ActionResult> Delete(long id)
    {
        await _deleteDiscountAction.ExecuteAsync(new DeleteDiscountRequest { Id = id }.FromRequest());
        return Ok(new { message = "discounts.deleted_successfully" });
    }
}
