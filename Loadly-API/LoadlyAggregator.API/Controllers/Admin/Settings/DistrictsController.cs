using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Requests.Admin.Settings.Districts;
using LoadlyAggregator.API.Resources.Admin.Settings;
using LoadlyAggregator.Domain.Admin.Actions.Districts;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Infrastructure.Authorization;

namespace LoadlyAggregator.API.Controllers.Admin.Settings;

[ApiController]
[Route("api/admin/settings/addresses/districts")]
[Authorize]
public class DistrictsController : ControllerBase
{
    private readonly CreateDistrictAction _createDistrictAction;
    private readonly GetDistrictAction _getDistrictAction;
    private readonly ListDistrictsAction _listDistrictsAction;
    private readonly UpdateDistrictAction _updateDistrictAction;
    private readonly DeleteDistrictAction _deleteDistrictAction;

    public DistrictsController(
        CreateDistrictAction createDistrictAction,
        GetDistrictAction getDistrictAction,
        ListDistrictsAction listDistrictsAction,
        UpdateDistrictAction updateDistrictAction,
        DeleteDistrictAction deleteDistrictAction)
    {
        _createDistrictAction = createDistrictAction;
        _getDistrictAction = getDistrictAction;
        _listDistrictsAction = listDistrictsAction;
        _updateDistrictAction = updateDistrictAction;
        _deleteDistrictAction = deleteDistrictAction;
    }

    [HttpPost]
    [HasPermission(CreateDistrictRequest.Permission)]
    public async Task<ActionResult> Create([FromBody] CreateDistrictRequest request)
    {
        await _createDistrictAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "districts.created_successfully" });
    }

    [HttpGet("{id}")]
    [HasPermission(GetDistrictRequest.Permission)]
    public async Task<ActionResult<DistrictResource>> Get(long id) =>
        Ok(DistrictResource.FromDto(await _getDistrictAction.ExecuteAsync(new GetDistrictRequest { Id = id }.FromRequest())));

    [HttpGet]
    [HasPermission(ListDistrictsRequest.Permission)]
    public async Task<ActionResult> List([FromQuery] ListDistrictsRequest request) =>
        Ok((await _listDistrictsAction.ExecuteAsync(request.FromRequest()))
            .ToResourceCollection<DistrictDto, DistrictResource>(dto => DistrictResource.FromDto(dto)));

    [HttpPut("{id}")]
    [HasPermission(UpdateDistrictRequest.Permission)]
    public async Task<ActionResult> Update(long id, [FromBody] UpdateDistrictRequest request)
    {
        request.Id = id;
        await _updateDistrictAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "districts.updated_successfully" });
    }

    [HttpDelete("{id}")]
    [HasPermission(DeleteDistrictRequest.Permission)]
    public async Task<ActionResult> Delete(long id)
    {
        await _deleteDistrictAction.ExecuteAsync(new DeleteDistrictRequest { Id = id }.FromRequest());
        return Ok(new { message = "districts.deleted_successfully" });
    }
}
