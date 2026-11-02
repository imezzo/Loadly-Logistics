using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Requests.Admin.Notifications;
using LoadlyAggregator.API.Requests.Common.Notifications;
using LoadlyAggregator.API.Resources.Common;
using LoadlyAggregator.Domain.Admin.Actions.Notifications;
using LoadlyAggregator.Domain.Common.DTOs.Notifications;

namespace LoadlyAggregator.API.Controllers.Admin;

[ApiController]
[Route("api/admin/notifications")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly ListNotificationsAction _listNotificationsAction;
    private readonly GetUnreadCountAction _getUnreadCountAction;
    private readonly MarkNotificationReadAction _markNotificationReadAction;
    private readonly BatchMarkNotificationsReadAction _batchMarkNotificationsReadAction;
    private readonly MarkAllNotificationsReadAction _markAllNotificationsReadAction;
    private readonly ListNotificationPreferencesAction _listNotificationPreferencesAction;
    private readonly UpdateNotificationPreferenceAction _updateNotificationPreferenceAction;

    public NotificationsController(
        ListNotificationsAction listNotificationsAction,
        GetUnreadCountAction getUnreadCountAction,
        MarkNotificationReadAction markNotificationReadAction,
        BatchMarkNotificationsReadAction batchMarkNotificationsReadAction,
        MarkAllNotificationsReadAction markAllNotificationsReadAction,
        ListNotificationPreferencesAction listNotificationPreferencesAction,
        UpdateNotificationPreferenceAction updateNotificationPreferenceAction)
    {
        _listNotificationsAction = listNotificationsAction;
        _getUnreadCountAction = getUnreadCountAction;
        _markNotificationReadAction = markNotificationReadAction;
        _batchMarkNotificationsReadAction = batchMarkNotificationsReadAction;
        _markAllNotificationsReadAction = markAllNotificationsReadAction;
        _listNotificationPreferencesAction = listNotificationPreferencesAction;
        _updateNotificationPreferenceAction = updateNotificationPreferenceAction;
    }

    [HttpGet]
    public async Task<ActionResult> List([FromQuery] ListNotificationsRequest request) =>
        Ok((await _listNotificationsAction.ExecuteAsync(request.FromRequest()))
            .ToResourceCollection<NotificationDto, NotificationResource>(dto => NotificationResource.FromDto(dto)));

    [HttpGet("unread-count")]
    public async Task<ActionResult> GetUnreadCount([FromQuery] GetUnreadCountRequest request) =>
        Ok(new { count = await _getUnreadCountAction.ExecuteAsync(request.EventType, request.Exclude) });

    [HttpPatch("{id}/read")]
    public async Task<ActionResult> MarkAsRead(long id)
    {
        await _markNotificationReadAction.ExecuteAsync(id);
        return Ok(new { message = "notifications.marked_as_read" });
    }

    [HttpPatch("read")]
    public async Task<ActionResult> BatchMarkAsRead([FromBody] BatchMarkNotificationsReadRequest request)
    {
        await _batchMarkNotificationsReadAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "notifications.marked_as_read" });
    }

    [HttpPatch("read-all")]
    public async Task<ActionResult> MarkAllAsRead()
    {
        await _markAllNotificationsReadAction.ExecuteAsync();
        return Ok(new { message = "notifications.all_marked_as_read" });
    }

    [HttpGet("preferences")]
    public async Task<ActionResult<IReadOnlyList<NotificationPreferenceResource>>> ListPreferences()
    {
        var items = await _listNotificationPreferencesAction.ExecuteAsync();
        return Ok(items.Select(NotificationPreferenceResource.FromDto).ToList());
    }

    [HttpPut("preferences")]
    public async Task<ActionResult> UpdatePreference([FromBody] UpdateNotificationPreferenceRequest request)
    {
        await _updateNotificationPreferenceAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "notifications.preference_updated" });
    }

}
