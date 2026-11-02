using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Requests.Admin.Settings.VehicleBrands;
using LoadlyAggregator.API.Resources.Admin.Settings;
using LoadlyAggregator.Domain.Admin.Actions.VehicleBrands;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Infrastructure.Authorization;

namespace LoadlyAggregator.API.Controllers.Admin.Settings;

[ApiController]
[Route("api/admin/settings/vehicle-brands")]
[Authorize]
public class VehicleBrandsController : ControllerBase
{
    private readonly CreateVehicleBrandAction _createVehicleBrandAction;
    private readonly GetVehicleBrandAction _getVehicleBrandAction;
    private readonly ListVehicleBrandsAction _listVehicleBrandsAction;
    private readonly UpdateVehicleBrandAction _updateVehicleBrandAction;
    private readonly DeleteVehicleBrandAction _deleteVehicleBrandAction;

    public VehicleBrandsController(
        CreateVehicleBrandAction createVehicleBrandAction,
        GetVehicleBrandAction getVehicleBrandAction,
        ListVehicleBrandsAction listVehicleBrandsAction,
        UpdateVehicleBrandAction updateVehicleBrandAction,
        DeleteVehicleBrandAction deleteVehicleBrandAction)
    {
        _createVehicleBrandAction = createVehicleBrandAction;
        _getVehicleBrandAction = getVehicleBrandAction;
        _listVehicleBrandsAction = listVehicleBrandsAction;
        _updateVehicleBrandAction = updateVehicleBrandAction;
        _deleteVehicleBrandAction = deleteVehicleBrandAction;
    }

    [HttpGet]
    [HasPermission(ListVehicleBrandsRequest.Permission)]
    public async Task<ActionResult> List([FromQuery] ListVehicleBrandsRequest request) =>
        Ok((await _listVehicleBrandsAction.ExecuteAsync(request.FromRequest()))
            .ToResourceCollection<VehicleBrandDto, VehicleBrandResource>(dto => VehicleBrandResource.FromDto(dto)));

    [HttpGet("{id}")]
    [HasPermission(GetVehicleBrandRequest.Permission)]
    public async Task<ActionResult<VehicleBrandResource>> Get(long id) =>
        Ok(VehicleBrandResource.FromDto(await _getVehicleBrandAction.ExecuteAsync(new GetVehicleBrandRequest { Id = id }.FromRequest())));

    [HttpPost]
    [HasPermission(CreateVehicleBrandRequest.Permission)]
    public async Task<ActionResult> Create([FromBody] CreateVehicleBrandRequest request)
    {
        await _createVehicleBrandAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "vehicle_brands.created_successfully" });
    }

    [HttpPut("{id}")]
    [HasPermission(UpdateVehicleBrandRequest.Permission)]
    public async Task<ActionResult> Update(long id, [FromBody] UpdateVehicleBrandRequest request)
    {
        request.Id = id;
        await _updateVehicleBrandAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "vehicle_brands.updated_successfully" });
    }

    [HttpDelete("{id}")]
    [HasPermission(DeleteVehicleBrandRequest.Permission)]
    public async Task<ActionResult> Delete(long id)
    {
        await _deleteVehicleBrandAction.ExecuteAsync(new DeleteVehicleBrandRequest { Id = id }.FromRequest());
        return Ok(new { message = "vehicle_brands.deleted_successfully" });
    }
}
