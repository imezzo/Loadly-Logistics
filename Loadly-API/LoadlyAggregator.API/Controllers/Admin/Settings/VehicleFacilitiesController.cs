using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Requests.Admin.Settings.VehicleFacilities;
using LoadlyAggregator.API.Resources.Admin.Settings;
using LoadlyAggregator.Domain.Admin.Actions.VehicleFacilities;
using LoadlyAggregator.Domain.Admin.DTOs.VehicleFacilities;
using LoadlyAggregator.Infrastructure.Authorization;

namespace LoadlyAggregator.API.Controllers.Admin.Settings;

[ApiController]
[Route("api/admin/settings/vehicle-facilities")]
[Authorize]
public class VehicleFacilitiesController : ControllerBase
{
    private readonly CreateVehicleFacilityAction _createVehicleFacilityAction;
    private readonly GetVehicleFacilityAction _getVehicleFacilityAction;
    private readonly ListVehicleFacilitiesAction _listVehicleFacilitiesAction;
    private readonly UpdateVehicleFacilityAction _updateVehicleFacilityAction;
    private readonly DeleteVehicleFacilityAction _deleteVehicleFacilityAction;

    public VehicleFacilitiesController(
        CreateVehicleFacilityAction createVehicleFacilityAction,
        GetVehicleFacilityAction getVehicleFacilityAction,
        ListVehicleFacilitiesAction listVehicleFacilitiesAction,
        UpdateVehicleFacilityAction updateVehicleFacilityAction,
        DeleteVehicleFacilityAction deleteVehicleFacilityAction)
    {
        _createVehicleFacilityAction = createVehicleFacilityAction;
        _getVehicleFacilityAction = getVehicleFacilityAction;
        _listVehicleFacilitiesAction = listVehicleFacilitiesAction;
        _updateVehicleFacilityAction = updateVehicleFacilityAction;
        _deleteVehicleFacilityAction = deleteVehicleFacilityAction;
    }

    [HttpPost]
    [HasPermission(CreateVehicleFacilityRequest.Permission)]
    public async Task<ActionResult> Create([FromBody] CreateVehicleFacilityRequest request)
    {
        await _createVehicleFacilityAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "vehicle_facilities.created_successfully" });
    }

    [HttpGet("{id}")]
    [HasPermission(GetVehicleFacilityRequest.Permission)]
    public async Task<ActionResult<VehicleFacilityResource>> Get(long id) =>
        Ok(VehicleFacilityResource.FromDto(await _getVehicleFacilityAction.ExecuteAsync(new GetVehicleFacilityRequest { Id = id }.FromRequest())));

    [HttpGet]
    [HasPermission(ListVehicleFacilitiesRequest.Permission)]
    public async Task<ActionResult> List([FromQuery] ListVehicleFacilitiesRequest request) =>
        Ok((await _listVehicleFacilitiesAction.ExecuteAsync(request.FromRequest()))
            .ToResourceCollection<VehicleFacilityDto, VehicleFacilityResource>(dto => VehicleFacilityResource.FromDto(dto)));

    [HttpPut("{id}")]
    [HasPermission(UpdateVehicleFacilityRequest.Permission)]
    public async Task<ActionResult> Update(long id, [FromBody] UpdateVehicleFacilityRequest request)
    {
        request.Id = id;
        await _updateVehicleFacilityAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "vehicle_facilities.updated_successfully" });
    }

    [HttpDelete("{id}")]
    [HasPermission(DeleteVehicleFacilityRequest.Permission)]
    public async Task<ActionResult> Delete(long id)
    {
        await _deleteVehicleFacilityAction.ExecuteAsync(new DeleteVehicleFacilityRequest { Id = id }.FromRequest());
        return Ok(new { message = "vehicle_facilities.deleted_successfully" });
    }
}
