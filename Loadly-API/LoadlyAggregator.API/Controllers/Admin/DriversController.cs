using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Requests.Admin.Drivers;
using LoadlyAggregator.API.Resources.Admin.Drivers;
using LoadlyAggregator.Domain.Admin.Actions.Drivers;
using LoadlyAggregator.Domain.Admin.DTOs.Drivers;
using LoadlyAggregator.Infrastructure.Authorization;

namespace LoadlyAggregator.API.Controllers.Admin;

[ApiController]
[Route("api/admin/drivers")]
[Authorize]
public class DriversController : ControllerBase
{
    private readonly CreateDriverAction _createDriverAction;
    private readonly GetDriverAction _getDriverAction;
    private readonly ListDriversAction _listDriversAction;
    private readonly UpdateDriverAction _updateDriverAction;
    private readonly DeleteDriverAction _deleteDriverAction;
    private readonly BulkImportDriversAction _bulkImportDriversAction;
    private readonly GetDriverWorkingStatisticsAction _getDriverWorkingStatisticsAction;
    private readonly ExportDriversExcelAction _exportDriversExcelAction;

    public DriversController(
        CreateDriverAction createDriverAction,
        GetDriverAction getDriverAction,
        ListDriversAction listDriversAction,
        UpdateDriverAction updateDriverAction,
        DeleteDriverAction deleteDriverAction,
        BulkImportDriversAction bulkImportDriversAction,
        GetDriverWorkingStatisticsAction getDriverWorkingStatisticsAction,
        ExportDriversExcelAction exportDriversExcelAction)
    {
        _createDriverAction = createDriverAction;
        _getDriverAction = getDriverAction;
        _listDriversAction = listDriversAction;
        _updateDriverAction = updateDriverAction;
        _deleteDriverAction = deleteDriverAction;
        _bulkImportDriversAction = bulkImportDriversAction;
        _getDriverWorkingStatisticsAction = getDriverWorkingStatisticsAction;
        _exportDriversExcelAction = exportDriversExcelAction;
    }

    [HttpPost]
    [HasPermission(CreateDriverRequest.Permission)]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult> Create([FromForm] CreateDriverRequest request)
    {
        await _createDriverAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "drivers.created_successfully" });
    }

    [HttpGet("statistics")]
    [HasPermission(GetDriverWorkingStatisticsRequest.Permission)]
    public async Task<ActionResult<DriversWorkingStatisticsResource>> GetWorkingStatistics() =>
        Ok(DriversWorkingStatisticsResource.FromDto(await _getDriverWorkingStatisticsAction.ExecuteAsync()));

    [HttpGet("excel")]
    [HasPermission(ExportDriversExcelRequest.Permission)]
    public async Task<IActionResult> ExportExcel([FromQuery] ExportDriversExcelRequest request)
    {
        DriversExcelFileResource resource = DriversExcelFileResource.FromDto(
            await _exportDriversExcelAction.ExecuteAsync(request.FromRequest()));
        return File(resource.Content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", resource.FileName);
    }

    [HttpGet("{id}")]
    [HasPermission(GetDriverRequest.Permission)]
    public async Task<ActionResult<DriverResource>> Get(long id) =>
        Ok(DriverResource.FromDto(await _getDriverAction.ExecuteAsync(new GetDriverRequest { Id = id }.FromRequest())));

    [HttpGet]
    [HasPermission(ListDriversRequest.Permission)]
    public async Task<ActionResult> List([FromQuery] ListDriversRequest request) =>
        Ok((await _listDriversAction.ExecuteAsync(request.FromRequest()))
            .ToResourceCollection<DriverDto, DriverResource>(dto => DriverResource.FromDto(dto)));

    [HttpPut("{id}")]
    [HasPermission(UpdateDriverRequest.Permission)]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult> Update(long id, [FromForm] UpdateDriverRequest request)
    {
        request.Id = id;
        await _updateDriverAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "drivers.updated_successfully" });
    }

    [HttpDelete("{id}")]
    [HasPermission(DeleteDriverRequest.Permission)]
    public async Task<ActionResult> Delete(long id)
    {
        var deleteRequest = new DeleteDriverRequest { Id = id };
        await _deleteDriverAction.ExecuteAsync(deleteRequest.FromRequest());
        return Ok(new { message = "drivers.deleted_successfully" });
    }

    [HttpPost("import")]
    [HasPermission(ImportDriversRequest.Permission)]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<BulkImportDriversResultResource>> Import([FromForm] ImportDriversRequest request)
    {
        var result = await _bulkImportDriversAction.ExecuteAsync(new ImportDriversFromExcelDto { OpenStream = () => request.ExcelFile.OpenReadStream() });
        return Ok(BulkImportDriversResultResource.FromResult(result));
    }
}
