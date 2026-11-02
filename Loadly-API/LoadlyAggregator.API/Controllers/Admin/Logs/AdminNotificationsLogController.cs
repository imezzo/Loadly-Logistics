using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Requests.Admin.Logs;
using LoadlyAggregator.API.Resources.Admin.Logs;
using LoadlyAggregator.Domain.Admin.Actions.Logs;
using LoadlyAggregator.Domain.Admin.DTOs.Logs;
using LoadlyAggregator.Infrastructure.Authorization;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.API.Controllers.Admin.Logs;

[ApiController]
[Route("api/admin/logs/notifications")]
[Authorize]
public class AdminNotificationsLogController : ControllerBase
{
    private readonly GetNotificationsLogsAction _getNotificationsLogsAction;

    public AdminNotificationsLogController(GetNotificationsLogsAction getNotificationsLogsAction)
    {
        _getNotificationsLogsAction = getNotificationsLogsAction;
    }

    [HttpGet]
    [HasPermission(GetNotificationsLogsRequest.Permission)]
    public async Task<ActionResult> GetNotificationsLogs([FromQuery] GetNotificationsLogsRequest request) =>
     Ok((await _getNotificationsLogsAction.ExecuteAsync(request.FromRequest()))
         .ToResourceCollection<NotificationsLogDto, NotificationsLogResource>(dto => NotificationsLogResource.FromDto(dto)));
}

