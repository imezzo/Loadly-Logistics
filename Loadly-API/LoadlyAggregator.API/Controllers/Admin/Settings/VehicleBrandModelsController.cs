using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Requests.Admin.Settings;
using LoadlyAggregator.API.Resources.Admin.Settings;
using LoadlyAggregator.Domain.Admin.Actions.VehicleBrandModels;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Infrastructure.Authorization;

namespace LoadlyAggregator.API.Controllers.Admin.Settings;

[ApiController]
[Route("api/admin/settings/vehicle-brand-models")]
[Authorize]
public class VehicleBrandModelsController : ControllerBase
{
    private readonly CreateVehicleBrandModelAction _createVehicleBrandModelAction;
    private readonly GetVehicleBrandModelAction _getVehicleBrandModelAction;
    private readonly ListVehicleBrandModelsAction _listVehicleBrandModelsAction;
    private readonly UpdateVehicleBrandModelAction _updateVehicleBrandModelAction;
    private readonly DeleteVehicleBrandModelAction _deleteVehicleBrandModelAction;

    public VehicleBrandModelsController(
        CreateVehicleBrandModelAction createVehicleBrandModelAction,
        GetVehicleBrandModelAction getVehicleBrandModelAction,
        ListVehicleBrandModelsAction listVehicleBrandModelsAction,
        UpdateVehicleBrandModelAction updateVehicleBrandModelAction,
        DeleteVehicleBrandModelAction deleteVehicleBrandModelAction)
    {
        _createVehicleBrandModelAction = createVehicleBrandModelAction;
        _getVehicleBrandModelAction = getVehicleBrandModelAction;
        _listVehicleBrandModelsAction = listVehicleBrandModelsAction;
        _updateVehicleBrandModelAction = updateVehicleBrandModelAction;
        _deleteVehicleBrandModelAction = deleteVehicleBrandModelAction;
    }

    [HttpGet]
    [HasPermission(ListVehicleBrandModelsRequest.Permission)]
    public async Task<ActionResult> List([FromQuery] ListVehicleBrandModelsRequest request) =>
        Ok((await _listVehicleBrandModelsAction.ExecuteAsync(request.FromRequest()))
            .ToResourceCollection<VehicleBrandModelDto, VehicleBrandModelResource>(dto => VehicleBrandModelResource.FromDto(dto)));

    [HttpGet("{id}")]
    [HasPermission(GetVehicleBrandModelRequest.Permission)]
    public async Task<ActionResult<VehicleBrandModelResource>> Get(long id) =>
        Ok(VehicleBrandModelResource.FromDto(await _getVehicleBrandModelAction.ExecuteAsync(new GetVehicleBrandModelRequest { Id = id }.FromRequest())));

    [HttpPost]
    [HasPermission(CreateVehicleBrandModelRequest.Permission)]
    public async Task<ActionResult> Create([FromBody] CreateVehicleBrandModelRequest request)
    {
        await _createVehicleBrandModelAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "vehicle_brand_models.created_successfully" });
    }

    [HttpPut("{id}")]
    [HasPermission(UpdateVehicleBrandModelRequest.Permission)]
    public async Task<ActionResult> Update(long id, [FromBody] UpdateVehicleBrandModelRequest request)
    {
        request.Id = id;
        await _updateVehicleBrandModelAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "vehicle_brand_models.updated_successfully" });
    }

    [HttpDelete("{id}")]
    [HasPermission(DeleteVehicleBrandModelRequest.Permission)]
    public async Task<ActionResult> Delete(long id)
    {
        var deleteRequest = new DeleteVehicleBrandModelRequest { Id = id };
        await _deleteVehicleBrandModelAction.ExecuteAsync(deleteRequest.FromRequest());
        return Ok(new { message = "vehicle_brand_models.deleted_successfully" });
    }
}
