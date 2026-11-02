using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Requests.Admin.Settings;
using LoadlyAggregator.API.Resources.Admin.Settings;
using LoadlyAggregator.Domain.Admin.Actions.VehicleInsurances;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Infrastructure.Authorization;

namespace LoadlyAggregator.API.Controllers.Admin.Settings;

[ApiController]
[Route("api/admin/settings/vehicle-insurances")]
[Authorize]
public class VehicleInsurancesController : ControllerBase
{
    private readonly CreateVehicleInsuranceAction _createVehicleInsuranceAction;
    private readonly GetVehicleInsuranceAction _getVehicleInsuranceAction;
    private readonly ListVehicleInsurancesAction _listVehicleInsurancesAction;
    private readonly UpdateVehicleInsuranceAction _updateVehicleInsuranceAction;
    private readonly DeleteVehicleInsuranceAction _deleteVehicleInsuranceAction;

    public VehicleInsurancesController(
        CreateVehicleInsuranceAction createVehicleInsuranceAction,
        GetVehicleInsuranceAction getVehicleInsuranceAction,
        ListVehicleInsurancesAction listVehicleInsurancesAction,
        UpdateVehicleInsuranceAction updateVehicleInsuranceAction,
        DeleteVehicleInsuranceAction deleteVehicleInsuranceAction)
    {
        _createVehicleInsuranceAction = createVehicleInsuranceAction;
        _getVehicleInsuranceAction = getVehicleInsuranceAction;
        _listVehicleInsurancesAction = listVehicleInsurancesAction;
        _updateVehicleInsuranceAction = updateVehicleInsuranceAction;
        _deleteVehicleInsuranceAction = deleteVehicleInsuranceAction;
    }

    [HttpPost]
    [HasPermission(CreateVehicleInsuranceRequest.Permission)]
    public async Task<ActionResult> Create([FromBody] CreateVehicleInsuranceRequest request)
    {
        await _createVehicleInsuranceAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "vehicle_insurances.created_successfully" });
    }

    [HttpGet("{id}")]
    [HasPermission(GetVehicleInsuranceRequest.Permission)]
    public async Task<ActionResult<VehicleInsuranceResource>> Get(long id) =>
        Ok(VehicleInsuranceResource.FromDto(await _getVehicleInsuranceAction.ExecuteAsync(new GetVehicleInsuranceRequest { Id = id }.FromRequest())));

    [HttpGet]
    [HasPermission(ListVehicleInsurancesRequest.Permission)]
    public async Task<ActionResult> List([FromQuery] ListVehicleInsurancesRequest request) =>
        Ok((await _listVehicleInsurancesAction.ExecuteAsync(request.FromRequest()))
            .ToResourceCollection<VehicleInsuranceDto, VehicleInsuranceResource>(dto => VehicleInsuranceResource.FromDto(dto)));

    [HttpPut("{id}")]
    [HasPermission(UpdateVehicleInsuranceRequest.Permission)]
    public async Task<ActionResult> Update(long id, [FromBody] UpdateVehicleInsuranceRequest request)
    {
        request.Id = id;
        await _updateVehicleInsuranceAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "vehicle_insurances.updated_successfully" });
    }

    [HttpDelete("{id}")]
    [HasPermission(DeleteVehicleInsuranceRequest.Permission)]
    public async Task<ActionResult> Delete(long id)
    {
        var deleteRequest = new DeleteVehicleInsuranceRequest { Id = id };
        await _deleteVehicleInsuranceAction.ExecuteAsync(deleteRequest.FromRequest());
        return Ok(new { message = "vehicle_insurances.deleted_successfully" });
    }
}
