using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Requests.Admin.Settings;
using LoadlyAggregator.API.Resources.Admin.Settings;
using LoadlyAggregator.Domain.Admin.Actions.VehicleCapacityUnits;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Infrastructure.Authorization;

namespace LoadlyAggregator.API.Controllers.Admin.Settings;

[ApiController]
[Route("api/admin/settings/vehicle-capacity-units")]
[Authorize]
public class VehicleCapacityUnitsController : ControllerBase
{
    private readonly CreateVehicleCapacityUnitAction _createVehicleCapacityUnitAction;
    private readonly GetVehicleCapacityUnitAction _getVehicleCapacityUnitAction;
    private readonly ListVehicleCapacityUnitsAction _listVehicleCapacityUnitsAction;
    private readonly UpdateVehicleCapacityUnitAction _updateVehicleCapacityUnitAction;
    private readonly DeleteVehicleCapacityUnitAction _deleteVehicleCapacityUnitAction;

    public VehicleCapacityUnitsController(
        CreateVehicleCapacityUnitAction createVehicleCapacityUnitAction,
        GetVehicleCapacityUnitAction getVehicleCapacityUnitAction,
        ListVehicleCapacityUnitsAction listVehicleCapacityUnitsAction,
        UpdateVehicleCapacityUnitAction updateVehicleCapacityUnitAction,
        DeleteVehicleCapacityUnitAction deleteVehicleCapacityUnitAction)
    {
        _createVehicleCapacityUnitAction = createVehicleCapacityUnitAction;
        _getVehicleCapacityUnitAction = getVehicleCapacityUnitAction;
        _listVehicleCapacityUnitsAction = listVehicleCapacityUnitsAction;
        _updateVehicleCapacityUnitAction = updateVehicleCapacityUnitAction;
        _deleteVehicleCapacityUnitAction = deleteVehicleCapacityUnitAction;
    }

    [HttpPost]
    [HasPermission(CreateVehicleCapacityUnitRequest.Permission)]
    public async Task<ActionResult> Create([FromBody] CreateVehicleCapacityUnitRequest request)
    {
        await _createVehicleCapacityUnitAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "vehicle_capacity_units.created_successfully" });
    }

    [HttpGet("{id}")]
    [HasPermission(GetVehicleCapacityUnitRequest.Permission)]
    public async Task<ActionResult<VehicleCapacityUnitResource>> Get(long id) =>
        Ok(VehicleCapacityUnitResource.FromDto(await _getVehicleCapacityUnitAction.ExecuteAsync(new GetVehicleCapacityUnitRequest { Id = id }.FromRequest())));

    [HttpGet]
    [HasPermission(ListVehicleCapacityUnitsRequest.Permission)]
    public async Task<ActionResult> List([FromQuery] ListVehicleCapacityUnitsRequest request) =>
        Ok((await _listVehicleCapacityUnitsAction.ExecuteAsync(request.FromRequest()))
            .ToResourceCollection<VehicleCapacityUnitDto, VehicleCapacityUnitResource>(dto => VehicleCapacityUnitResource.FromDto(dto)));

    [HttpPut("{id}")]
    [HasPermission(UpdateVehicleCapacityUnitRequest.Permission)]
    public async Task<ActionResult> Update(long id, [FromBody] UpdateVehicleCapacityUnitRequest request)
    {
        request.Id = id;
        await _updateVehicleCapacityUnitAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "vehicle_capacity_units.updated_successfully" });
    }

    [HttpDelete("{id}")]
    [HasPermission(DeleteVehicleCapacityUnitRequest.Permission)]
    public async Task<ActionResult> Delete(long id)
    {
        var deleteRequest = new DeleteVehicleCapacityUnitRequest { Id = id };
        await _deleteVehicleCapacityUnitAction.ExecuteAsync(deleteRequest.FromRequest());
        return Ok(new { message = "vehicle_capacity_units.deleted_successfully" });
    }
}
