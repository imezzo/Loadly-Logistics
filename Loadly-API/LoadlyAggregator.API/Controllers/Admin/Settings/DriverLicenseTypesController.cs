using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Requests.Admin.Settings.DriverLicenseTypes;
using LoadlyAggregator.API.Resources.Admin.Settings;
using LoadlyAggregator.Domain.Admin.Actions.DriverLicenseTypes;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Infrastructure.Authorization;

namespace LoadlyAggregator.API.Controllers.Admin.Settings;

[ApiController]
[Route("api/admin/settings/driver-license-types")]
[Authorize]
public class DriverLicenseTypesController : ControllerBase
{
    private readonly CreateDriverLicenseTypeAction _createDriverLicenseTypeAction;
    private readonly GetDriverLicenseTypeAction _getDriverLicenseTypeAction;
    private readonly ListDriverLicenseTypesAction _listDriverLicenseTypesAction;
    private readonly UpdateDriverLicenseTypeAction _updateDriverLicenseTypeAction;
    private readonly DeleteDriverLicenseTypeAction _deleteDriverLicenseTypeAction;

    public DriverLicenseTypesController(
        CreateDriverLicenseTypeAction createDriverLicenseTypeAction,
        GetDriverLicenseTypeAction getDriverLicenseTypeAction,
        ListDriverLicenseTypesAction listDriverLicenseTypesAction,
        UpdateDriverLicenseTypeAction updateDriverLicenseTypeAction,
        DeleteDriverLicenseTypeAction deleteDriverLicenseTypeAction)
    {
        _createDriverLicenseTypeAction = createDriverLicenseTypeAction;
        _getDriverLicenseTypeAction = getDriverLicenseTypeAction;
        _listDriverLicenseTypesAction = listDriverLicenseTypesAction;
        _updateDriverLicenseTypeAction = updateDriverLicenseTypeAction;
        _deleteDriverLicenseTypeAction = deleteDriverLicenseTypeAction;
    }

    [HttpGet]
    [HasPermission(ListDriverLicenseTypesRequest.Permission)]
    public async Task<ActionResult> List([FromQuery] ListDriverLicenseTypesRequest request) =>
        Ok((await _listDriverLicenseTypesAction.ExecuteAsync(request.FromRequest()))
            .ToResourceCollection<DriverLicenseTypeDto, DriverLicenseTypeResource>(dto => DriverLicenseTypeResource.FromDto(dto)));

    [HttpPost]
    [HasPermission(CreateDriverLicenseTypeRequest.Permission)]
    public async Task<ActionResult> Create([FromBody] CreateDriverLicenseTypeRequest request)
    {
        await _createDriverLicenseTypeAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "driver_license_types.created_successfully" });
    }

    [HttpGet("{id}")]
    [HasPermission(GetDriverLicenseTypeRequest.Permission)]
    public async Task<ActionResult<DriverLicenseTypeResource>> Get(long id) =>
        Ok(DriverLicenseTypeResource.FromDto(await _getDriverLicenseTypeAction.ExecuteAsync(new GetDriverLicenseTypeRequest { Id = id }.FromRequest())));

    [HttpPut("{id}")]
    [HasPermission(UpdateDriverLicenseTypeRequest.Permission)]
    public async Task<ActionResult> Update(long id, [FromBody] UpdateDriverLicenseTypeRequest request)
    {
        request.Id = id;
        await _updateDriverLicenseTypeAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "driver_license_types.updated_successfully" });
    }

    [HttpDelete("{id}")]
    [HasPermission(DeleteDriverLicenseTypeRequest.Permission)]
    public async Task<ActionResult> Delete(long id)
    {
        await _deleteDriverLicenseTypeAction.ExecuteAsync(new DeleteDriverLicenseTypeRequest { Id = id }.FromRequest());
        return Ok(new { message = "driver_license_types.deleted_successfully" });
    }
}
