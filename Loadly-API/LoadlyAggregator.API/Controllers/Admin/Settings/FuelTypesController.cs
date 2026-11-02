using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Requests.Admin.Settings.FuelTypes;
using LoadlyAggregator.API.Resources.Admin.Settings;
using LoadlyAggregator.Domain.Admin.Actions.FuelTypes;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Infrastructure.Authorization;

namespace LoadlyAggregator.API.Controllers.Admin.Settings;

[ApiController]
[Route("api/admin/settings/fuel-types")]
[Authorize]
public class FuelTypesController : ControllerBase
{
    private readonly CreateFuelTypeAction _createFuelTypeAction;
    private readonly GetFuelTypeAction _getFuelTypeAction;
    private readonly ListFuelTypesAction _listFuelTypesAction;
    private readonly UpdateFuelTypeAction _updateFuelTypeAction;
    private readonly DeleteFuelTypeAction _deleteFuelTypeAction;

    public FuelTypesController(
        CreateFuelTypeAction createFuelTypeAction,
        GetFuelTypeAction getFuelTypeAction,
        ListFuelTypesAction listFuelTypesAction,
        UpdateFuelTypeAction updateFuelTypeAction,
        DeleteFuelTypeAction deleteFuelTypeAction)
    {
        _createFuelTypeAction = createFuelTypeAction;
        _getFuelTypeAction = getFuelTypeAction;
        _listFuelTypesAction = listFuelTypesAction;
        _updateFuelTypeAction = updateFuelTypeAction;
        _deleteFuelTypeAction = deleteFuelTypeAction;
    }

    [HttpPost]
    [HasPermission(CreateFuelTypeRequest.Permission)]
    public async Task<ActionResult> Create([FromBody] CreateFuelTypeRequest request)
    {
        await _createFuelTypeAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "fuel_types.created_successfully" });
    }

    [HttpGet("{id}")]
    [HasPermission(GetFuelTypeRequest.Permission)]
    public async Task<ActionResult<FuelTypeResource>> Get(long id) =>
        Ok(FuelTypeResource.FromDto(await _getFuelTypeAction.ExecuteAsync(new GetFuelTypeRequest { Id = id }.FromRequest())));

    [HttpGet]
    [HasPermission(ListFuelTypesRequest.Permission)]
    public async Task<ActionResult> List([FromQuery] ListFuelTypesRequest request) =>
        Ok((await _listFuelTypesAction.ExecuteAsync(request.FromRequest()))
            .ToResourceCollection<FuelTypeDto, FuelTypeResource>(dto => FuelTypeResource.FromDto(dto)));

    [HttpPut("{id}")]
    [HasPermission(UpdateFuelTypeRequest.Permission)]
    public async Task<ActionResult> Update(long id, [FromBody] UpdateFuelTypeRequest request)
    {
        request.Id = id;
        await _updateFuelTypeAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "fuel_types.updated_successfully" });
    }

    [HttpDelete("{id}")]
    [HasPermission(DeleteFuelTypeRequest.Permission)]
    public async Task<ActionResult> Delete(long id)
    {
        await _deleteFuelTypeAction.ExecuteAsync(new DeleteFuelTypeRequest { Id = id }.FromRequest());
        return Ok(new { message = "fuel_types.deleted_successfully" });
    }
}
