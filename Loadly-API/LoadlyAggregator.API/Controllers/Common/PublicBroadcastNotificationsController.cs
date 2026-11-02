using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Resources.Common;
using LoadlyAggregator.Domain.Common.Actions.BroadcastNotifications;
using LoadlyAggregator.Domain.Common.DTOs;

namespace LoadlyAggregator.API.Controllers.Common;

[ApiController]
[Route("api/public/broadcast-notifications")]
[AllowAnonymous]
public class PublicBroadcastNotificationsController : ControllerBase
{
    private readonly GetPublicBroadcastNotificationAction _getPublicBroadcastNotificationAction;

    public PublicBroadcastNotificationsController(
        GetPublicBroadcastNotificationAction getPublicBroadcastNotificationAction)
    {
        _getPublicBroadcastNotificationAction = getPublicBroadcastNotificationAction;
    }

    [HttpGet("{token}")]
    public async Task<ActionResult<PublicBroadcastNotificationResource>> Get(string token)
    {
        PublicBroadcastNotificationDto dto = await _getPublicBroadcastNotificationAction.ExecuteAsync(token);
        return Ok(PublicBroadcastNotificationResource.FromDto(dto));
    }
}
