using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Requests.Admin.Settings.VehicleTypes;
using LoadlyAggregator.API.Resources.Admin.Settings;
using LoadlyAggregator.Domain.Admin.Actions.VehicleTypes;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Infrastructure.Authorization;

namespace LoadlyAggregator.API.Controllers.Admin.Settings;

[ApiController]
[Route("api/admin/settings/vehicle-types")]
[Authorize]
public class VehicleTypesController : ControllerBase
{
    private readonly CreateVehicleTypeAction _createVehicleTypeAction;
    private readonly GetVehicleTypeAction _getVehicleTypeAction;
    private readonly ListVehicleTypesAction _listVehicleTypesAction;
    private readonly UpdateVehicleTypeAction _updateVehicleTypeAction;
    private readonly DeleteVehicleTypeAction _deleteVehicleTypeAction;

    public VehicleTypesController(
        CreateVehicleTypeAction createVehicleTypeAction,
        GetVehicleTypeAction getVehicleTypeAction,
        ListVehicleTypesAction listVehicleTypesAction,
        UpdateVehicleTypeAction updateVehicleTypeAction,
        DeleteVehicleTypeAction deleteVehicleTypeAction)
    {
        _createVehicleTypeAction = createVehicleTypeAction;
        _getVehicleTypeAction = getVehicleTypeAction;
        _listVehicleTypesAction = listVehicleTypesAction;
        _updateVehicleTypeAction = updateVehicleTypeAction;
        _deleteVehicleTypeAction = deleteVehicleTypeAction;
    }

    [HttpGet]
    //[HasPermission(ListVehicleTypesRequest.Permission)]
    public async Task<ActionResult> List([FromQuery] ListVehicleTypesRequest request) =>
        Ok((await _listVehicleTypesAction.ExecuteAsync(request.FromRequest()))
            .ToResourceCollection<VehicleTypeDto, VehicleTypeResource>(dto => VehicleTypeResource.FromDto(dto)));

    [HttpGet("{id}")]
    //[HasPermission(GetVehicleTypeRequest.Permission)]
    public async Task<ActionResult<VehicleTypeResource>> Get(long id) =>
        Ok(VehicleTypeResource.FromDto(await _getVehicleTypeAction.ExecuteAsync(new GetVehicleTypeRequest { Id = id }.FromRequest())));

    [HttpPost]
    [HasPermission(CreateVehicleTypeRequest.Permission)]
    public async Task<ActionResult> Create([FromBody] CreateVehicleTypeRequest request)
    {
        await _createVehicleTypeAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "vehicle_types.created_successfully" });
    }

    [HttpPut("{id}")]
    [HasPermission(UpdateVehicleTypeRequest.Permission)]
    public async Task<ActionResult> Update(long id, [FromBody] UpdateVehicleTypeRequest request)
    {
        request.Id = id;
        await _updateVehicleTypeAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "vehicle_types.updated_successfully" });
    }

    [HttpDelete("{id}")]
    [HasPermission(DeleteVehicleTypeRequest.Permission)]
    public async Task<ActionResult> Delete(long id)
    {
        await _deleteVehicleTypeAction.ExecuteAsync(new DeleteVehicleTypeRequest { Id = id }.FromRequest());
        return Ok(new { message = "vehicle_types.deleted_successfully" });
    }
}
