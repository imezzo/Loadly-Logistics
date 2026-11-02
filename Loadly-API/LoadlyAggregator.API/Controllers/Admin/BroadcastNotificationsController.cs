using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Requests.Admin.Settings.PushNotifications;
using LoadlyAggregator.API.Resources.Admin.Settings;
using LoadlyAggregator.Domain.Admin.Actions.Settings.PushNotifications;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Infrastructure.Authorization;

namespace LoadlyAggregator.API.Controllers.Admin;

[ApiController]
[Route("api/admin/broadcast-notifications")]
[Authorize]
public class BroadcastNotificationsController : ControllerBase
{
    private readonly ListBroadcastNotificationsAction _listBroadcastNotificationsAction;
    private readonly GetBroadcastNotificationAction _getBroadcastNotificationAction;
    private readonly SendBroadcastNotificationAction _sendBroadcastNotificationAction;

    public BroadcastNotificationsController(
        ListBroadcastNotificationsAction listBroadcastNotificationsAction,
        GetBroadcastNotificationAction getBroadcastNotificationAction,
        SendBroadcastNotificationAction sendBroadcastNotificationAction)
    {
        _listBroadcastNotificationsAction = listBroadcastNotificationsAction;
        _getBroadcastNotificationAction = getBroadcastNotificationAction;
        _sendBroadcastNotificationAction = sendBroadcastNotificationAction;
    }

    [HttpGet]
    [HasPermission(ListBroadcastNotificationsRequest.Permission)]
    public async Task<ActionResult> List([FromQuery] ListBroadcastNotificationsRequest request) =>
        Ok((await _listBroadcastNotificationsAction.ExecuteAsync(request.FromRequest()))
            .ToResourceCollection<BroadcastNotificationDto, BroadcastNotificationItemResource>(dto =>
                BroadcastNotificationItemResource.FromDto(dto)));

    [HttpGet("{id:long}")]
    [HasPermission(ListBroadcastNotificationsRequest.Permission)]
    public async Task<ActionResult<BroadcastNotificationItemResource>> Get(long id) =>
        Ok(BroadcastNotificationItemResource.FromDto(await _getBroadcastNotificationAction.ExecuteAsync(id)));

    [HttpPost("send")]
    [HasPermission(SendBroadcastNotificationRequest.Permission)]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<BroadcastNotificationResource>> Send([FromForm] SendBroadcastNotificationRequest request)
    {
        BroadcastNotificationResultDto result = await _sendBroadcastNotificationAction.ExecuteAsync(request.FromRequest());
        return Ok(BroadcastNotificationResource.FromDto(result));
    }
}
