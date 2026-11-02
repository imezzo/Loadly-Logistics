using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Requests.Admin.Settings.Governerates;
using LoadlyAggregator.API.Resources.Admin.Settings;
using LoadlyAggregator.Domain.Admin.Actions.Governerates;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Infrastructure.Authorization;

namespace LoadlyAggregator.API.Controllers.Admin.Settings;

[ApiController]
[Route("api/admin/settings/addresses/governerates")]
[Authorize]
public class GoverneratesController : ControllerBase
{
    private readonly CreateGovernerateAction _createGovernerateAction;
    private readonly GetGovernerateAction _getGovernerateAction;
    private readonly ListGoverneratesAction _listGoverneratesAction;
    private readonly UpdateGovernerateAction _updateGovernerateAction;
    private readonly DeleteGovernerateAction _deleteGovernerateAction;

    public GoverneratesController(
        CreateGovernerateAction createGovernerateAction,
        GetGovernerateAction getGovernerateAction,
        ListGoverneratesAction listGoverneratesAction,
        UpdateGovernerateAction updateGovernerateAction,
        DeleteGovernerateAction deleteGovernerateAction)
    {
        _createGovernerateAction = createGovernerateAction;
        _getGovernerateAction = getGovernerateAction;
        _listGoverneratesAction = listGoverneratesAction;
        _updateGovernerateAction = updateGovernerateAction;
        _deleteGovernerateAction = deleteGovernerateAction;
    }

    [HttpPost]
    [HasPermission(CreateGovernerateRequest.Permission)]
    public async Task<ActionResult> Create([FromBody] CreateGovernerateRequest request)
    {
        await _createGovernerateAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "governerates.created_successfully" });
    }

    [HttpGet("{id}")]
    [HasPermission(GetGovernerateRequest.Permission)]
    public async Task<ActionResult<GovernerateResource>> Get(long id) =>
        Ok(GovernerateResource.FromDto(await _getGovernerateAction.ExecuteAsync(new GetGovernerateRequest { Id = id }.FromRequest())));

    [HttpGet]
    [HasPermission(ListGoverneratesRequest.Permission)]
    public async Task<ActionResult> List([FromQuery] ListGoverneratesRequest request) =>
        Ok((await _listGoverneratesAction.ExecuteAsync(request.FromRequest()))
            .ToResourceCollection<GovernerateDto, GovernerateResource>(dto => GovernerateResource.FromDto(dto)));

    [HttpPut("{id}")]
    [HasPermission(UpdateGovernerateRequest.Permission)]
    public async Task<ActionResult> Update(long id, [FromBody] UpdateGovernerateRequest request)
    {
        request.Id = id;
        await _updateGovernerateAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "governerates.updated_successfully" });
    }

    [HttpDelete("{id}")]
    [HasPermission(DeleteGovernerateRequest.Permission)]
    public async Task<ActionResult> Delete(long id)
    {
        await _deleteGovernerateAction.ExecuteAsync(new DeleteGovernerateRequest { Id = id }.FromRequest());
        return Ok(new { message = "governerates.deleted_successfully" });
    }
}
