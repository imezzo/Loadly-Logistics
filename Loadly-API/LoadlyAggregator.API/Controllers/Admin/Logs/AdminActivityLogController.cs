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
[Route("api/admin/logs/activity")]
[Authorize]
public class AdminActivityLogController : ControllerBase
{
    private readonly GetActivityLogsAction _getActivityLogsAction;
    private readonly GetActivityLogAction _getActivityLogAction;

    public AdminActivityLogController(
        GetActivityLogsAction getActivityLogsAction,
        GetActivityLogAction getActivityLogAction)
    {
        _getActivityLogsAction = getActivityLogsAction;
        _getActivityLogAction = getActivityLogAction;
    }

    [HttpGet]
    [HasPermission(GetActivityLogsRequest.Permission)]
    public async Task<ActionResult> GetActivityLogs(
        [FromQuery] GetActivityLogsRequest request) =>
        Ok((await _getActivityLogsAction.ExecuteAsync(request.FromRequest()))
            .ToResourceCollection<ActivityLogDto, ActivityLogResource>(dto => ActivityLogResource.FromDto(dto)));

    [HttpGet("{id}")]
    [HasPermission(GetActivityLogsRequest.Permission)]
    public async Task<ActionResult<ActivityLogResource>> GetActivityLog(long id) =>
        Ok(ActivityLogResource.FromDto(
            await _getActivityLogAction.ExecuteAsync(new GetActivityLogDto { Id = id })));
}
