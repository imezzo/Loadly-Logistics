using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Requests.Admin.Vehicles;
using LoadlyAggregator.API.Requests.Admin.Vehicles.VehicleFacilityVisits;
using LoadlyAggregator.API.Resources.Admin.Vehicles;
using LoadlyAggregator.Domain.Admin.Actions.Vehicles;
using LoadlyAggregator.Domain.Admin.Actions.VehicleFacilityVisits;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Infrastructure.Authorization;

namespace LoadlyAggregator.API.Controllers.Admin;

[ApiController]
[Route("api/admin/vehicles")]
[Authorize]
public class VehiclesController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly CreateVehicleAction _createVehicleAction;
    private readonly GetVehicleAction _getVehicleAction;
    private readonly ListVehiclesAction _listVehiclesAction;
    private readonly UpdateVehicleAction _updateVehicleAction;
    private readonly DeleteVehicleAction _deleteVehicleAction;
    private readonly BulkImportVehiclesAction _bulkImportVehiclesAction;
    private readonly ExportVehiclesExcelAction _exportVehiclesExcelAction;
    private readonly EnterVehicleFacilityVisitAction _enterVehicleFacilityVisitAction;
    private readonly ExitVehicleFacilityVisitAction _exitVehicleFacilityVisitAction;
    private readonly EnterVehicleParkingFacilityAction _enterVehicleParkingFacilityAction;
    private readonly ExitVehicleParkingFacilityAction _exitVehicleParkingFacilityAction;
    private readonly SetVehicleAssignedParkingFacilityAction _setVehicleAssignedParkingFacilityAction;
    private readonly AssignVehicleDriverAction _assignVehicleDriverAction;
    private readonly ListVehicleAssignmentHistoriesAction _listVehicleAssignmentHistoriesAction;

    public VehiclesController(
        IMapper mapper,
        CreateVehicleAction createVehicleAction,
        GetVehicleAction getVehicleAction,
        ListVehiclesAction listVehiclesAction,
        UpdateVehicleAction updateVehicleAction,
        DeleteVehicleAction deleteVehicleAction,
        BulkImportVehiclesAction bulkImportVehiclesAction,
        ExportVehiclesExcelAction exportVehiclesExcelAction,
        EnterVehicleFacilityVisitAction enterVehicleFacilityVisitAction,
        ExitVehicleFacilityVisitAction exitVehicleFacilityVisitAction,
        EnterVehicleParkingFacilityAction enterVehicleParkingFacilityAction,
        ExitVehicleParkingFacilityAction exitVehicleParkingFacilityAction,
        SetVehicleAssignedParkingFacilityAction setVehicleAssignedParkingFacilityAction,
        AssignVehicleDriverAction assignVehicleDriverAction,
        ListVehicleAssignmentHistoriesAction listVehicleAssignmentHistoriesAction)
    {
        _mapper = mapper;
        _createVehicleAction = createVehicleAction;
        _getVehicleAction = getVehicleAction;
        _listVehiclesAction = listVehiclesAction;
        _updateVehicleAction = updateVehicleAction;
        _deleteVehicleAction = deleteVehicleAction;
        _bulkImportVehiclesAction = bulkImportVehiclesAction;
        _exportVehiclesExcelAction = exportVehiclesExcelAction;
        _enterVehicleFacilityVisitAction = enterVehicleFacilityVisitAction;
        _exitVehicleFacilityVisitAction = exitVehicleFacilityVisitAction;
        _enterVehicleParkingFacilityAction = enterVehicleParkingFacilityAction;
        _exitVehicleParkingFacilityAction = exitVehicleParkingFacilityAction;
        _setVehicleAssignedParkingFacilityAction = setVehicleAssignedParkingFacilityAction;
        _assignVehicleDriverAction = assignVehicleDriverAction;
        _listVehicleAssignmentHistoriesAction = listVehicleAssignmentHistoriesAction;
    }

    [HttpPost]
    [HasPermission(CreateVehicleRequest.Permission)]
    public async Task<ActionResult> Create([FromForm] CreateVehicleRequest request)
    {
        await _createVehicleAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "vehicles.created_successfully" });
    }

    [HttpGet("excel")]
    [HasPermission(ExportVehiclesExcelRequest.Permission)]
    public async Task<IActionResult> ExportExcel([FromQuery] ExportVehiclesExcelRequest request)
    {
        VehiclesExcelFileResource resource = VehiclesExcelFileResource.FromDto(
            await _exportVehiclesExcelAction.ExecuteAsync(request.FromRequest()));
        return File(resource.Content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", resource.FileName);
    }

    [HttpGet("{id}")]
    [HasPermission(GetVehicleRequest.Permission)]
    public async Task<ActionResult<VehicleResource>> Get(long id) =>
        Ok(_mapper.Map<VehicleResource>(await _getVehicleAction.ExecuteAsync(new GetVehicleRequest { Id = id }.FromRequest())));

    [HttpGet]
    [HasPermission(ListVehiclesRequest.Permission)]
    public async Task<ActionResult> List([FromQuery] ListVehiclesRequest request) =>
        Ok((await _listVehiclesAction.ExecuteAsync(request.FromRequest()))
            .ToResourceCollection<VehicleDto, VehicleResource>(dto => _mapper.Map<VehicleResource>(dto)));

    [HttpPut("{id}")]
    [HasPermission(UpdateVehicleRequest.Permission)]
    public async Task<ActionResult> Update(long id, [FromForm] UpdateVehicleRequest request)
    {
        request.Id = id;
        await _updateVehicleAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "vehicles.updated_successfully" });
    }

    [HttpDelete("{id}")]
    [HasPermission(DeleteVehicleRequest.Permission)]
    public async Task<ActionResult> Delete(long id)
    {
        var request = new DeleteVehicleRequest { Id = id };
        await _deleteVehicleAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "vehicles.deleted_successfully" });
    }

    [HttpPost("import")]
    [HasPermission(ImportVehiclesRequest.Permission)]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<BulkImportVehiclesResultResource>> Import([FromForm] ImportVehiclesRequest request)
    {
        var result = await _bulkImportVehiclesAction.ExecuteAsync(new ImportVehiclesFromExcelDto { OpenStream = () => request.ExcelFile!.OpenReadStream() });
        return Ok(BulkImportVehiclesResultResource.FromResult(result));
    }

    [HttpGet("{vehicleId}/assignment-histories")]
    [HasPermission(ListVehicleAssignmentHistoriesRequest.Permission)]
    public async Task<ActionResult> ListAssignmentHistories(long vehicleId, [FromQuery] ListVehicleAssignmentHistoriesRequest request) =>
        Ok((await _listVehicleAssignmentHistoriesAction.ExecuteAsync(request.FromRequest(vehicleId)))
            .ToResourceCollection<VehicleAssignmentHistoryDto, VehicleAssignmentHistoryResource>(VehicleAssignmentHistoryResource.FromDto));

    [HttpPost("{vehicleId}/facility-visits/parking/enter")]
    [HasPermission(EnterVehicleParkingFacilityRequest.Permission)]
    public async Task<ActionResult> EnterParkingFacilityVisit(long vehicleId, [FromBody] EnterVehicleParkingFacilityRequest request)
    {
        await _enterVehicleParkingFacilityAction.ExecuteAsync(request.FromRequest(vehicleId));
        return Ok(new { message = "vehicle_facility_visits.parking_entered_successfully" });
    }

    [HttpPost("{vehicleId}/facility-visits/parking/exit")]
    [HasPermission(ExitVehicleParkingFacilityRequest.Permission)]
    public async Task<ActionResult> ExitParkingFacilityVisit(long vehicleId, [FromBody] ExitVehicleParkingFacilityRequest request)
    {
        await _exitVehicleParkingFacilityAction.ExecuteAsync(request.FromRequest(vehicleId));
        return Ok(new { message = "vehicle_facility_visits.parking_exited_successfully" });
    }

    [HttpPost("{vehicleId}/facility-visits/enter")]
    [HasPermission(EnterVehicleFacilityVisitRequest.Permission)]
    public async Task<ActionResult> EnterLocationVisit(long vehicleId, [FromBody] EnterVehicleFacilityVisitRequest request)
    {
        await _enterVehicleFacilityVisitAction.ExecuteAsync(request.FromRequest(vehicleId));
        return Ok(new { message = "vehicle_facility_visits.entered_successfully" });
    }

    [HttpPost("{vehicleId}/facility-visits/exit")]
    [HasPermission(ExitVehicleFacilityVisitRequest.Permission)]
    public async Task<ActionResult> ExitLocationVisit(long vehicleId, [FromBody] ExitVehicleFacilityVisitRequest request)
    {
        await _exitVehicleFacilityVisitAction.ExecuteAsync(request.FromRequest(vehicleId));
        return Ok(new { message = "vehicle_facility_visits.exited_successfully" });
    }

    [HttpPatch("{vehicleId}/assigned-parking-facility")]
    [HasPermission(SetVehicleAssignedParkingFacilityRequest.Permission)]
    public async Task<ActionResult> SetAssignedParkingFacility(long vehicleId, [FromBody] SetVehicleAssignedParkingFacilityRequest request)
    {
        await _setVehicleAssignedParkingFacilityAction.ExecuteAsync(request.FromRequest(vehicleId));
        return Ok(new { message = "vehicles.assigned_parking_facility_updated_successfully" });
    }

    [HttpPatch("{vehicleId}/driver")]
    [HasPermission(AssignVehicleDriverRequest.Permission)]
    public async Task<ActionResult> AssignDriver(long vehicleId, [FromBody] AssignVehicleDriverRequest? request)
    {
        request ??= new AssignVehicleDriverRequest();
        await _assignVehicleDriverAction.ExecuteAsync(request.FromRequest(vehicleId));
        return Ok(new { message = "vehicles.driver_assigned_successfully" });
    }
}
