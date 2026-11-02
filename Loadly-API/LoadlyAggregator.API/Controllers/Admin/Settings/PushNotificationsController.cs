using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Requests.Admin.Settings.PushNotifications;
using LoadlyAggregator.API.Resources.Admin.Settings;
using LoadlyAggregator.Domain.Admin.Actions.Settings.PushNotifications;
using LoadlyAggregator.Infrastructure.Authorization;

namespace LoadlyAggregator.API.Controllers.Admin.Settings;

[ApiController]
[Route("api/admin/settings/push-notifications")]
[Authorize]
public class PushNotificationsController : ControllerBase
{
    private readonly SendPushNotificationAction _sendPushNotificationAction;

    public PushNotificationsController(SendPushNotificationAction sendPushNotificationAction)
    {
        _sendPushNotificationAction = sendPushNotificationAction;
    }

    [HttpPost("send")]
    [HasPermission(SendPushNotificationRequest.Permission)]
    public async Task<ActionResult<PushNotificationResource>> Send([FromBody] SendPushNotificationRequest request)
    {
        var result = await _sendPushNotificationAction.ExecuteAsync(request.FromRequest());
        return Ok(PushNotificationResource.FromResult(result, request.DeviceToken, request.Title, request.Domain));
    }
}
