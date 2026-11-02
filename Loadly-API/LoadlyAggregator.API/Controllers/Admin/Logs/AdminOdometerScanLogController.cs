using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Requests.Admin.Logs;
using LoadlyAggregator.API.Resources.Admin.Logs;
using LoadlyAggregator.Domain.Admin.Actions.Logs;
using LoadlyAggregator.Domain.Admin.DTOs.Logs;
using LoadlyAggregator.Infrastructure.Authorization;

namespace LoadlyAggregator.API.Controllers.Admin.Logs;

[ApiController]
[Route("api/admin/logs/odometer-scans")]
[Authorize]
public class AdminOdometerScanLogController : ControllerBase
{
    private readonly GetOdometerScanLogsAction _getOdometerScanLogsAction;
    private readonly GetOdometerScanLogAction _getOdometerScanLogAction;

    public AdminOdometerScanLogController(
        GetOdometerScanLogsAction getOdometerScanLogsAction,
        GetOdometerScanLogAction getOdometerScanLogAction)
    {
        _getOdometerScanLogsAction = getOdometerScanLogsAction;
        _getOdometerScanLogAction = getOdometerScanLogAction;
    }

    [HttpGet]
    [HasPermission(GetOdometerScanLogsRequest.Permission)]
    public async Task<ActionResult> GetOdometerScanLogs(
        [FromQuery] GetOdometerScanLogsRequest request) =>
        Ok((await _getOdometerScanLogsAction.ExecuteAsync(request.FromRequest()))
            .ToResourceCollection<OdometerScanLogDto, OdometerScanLogResource>(dto => OdometerScanLogResource.FromDto(dto)));

    [HttpGet("{id:long}")]
    [HasPermission(GetOdometerScanLogsRequest.Permission)]
    public async Task<ActionResult<OdometerScanLogResource>> GetOdometerScanLog(long id) =>
        Ok(OdometerScanLogResource.FromDto(
            await _getOdometerScanLogAction.ExecuteAsync(new GetOdometerScanLogDto { Id = id })));
}
