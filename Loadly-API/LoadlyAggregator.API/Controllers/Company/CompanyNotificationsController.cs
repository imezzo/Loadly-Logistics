using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Requests.Company.Notifications;
using LoadlyAggregator.API.Requests.Common.Notifications;
using LoadlyAggregator.API.Resources.Common;
using LoadlyAggregator.Domain.Common.DTOs.Notifications;
using LoadlyAggregator.Domain.Company.Actions.Notifications;

namespace LoadlyAggregator.API.Controllers.Company;

[ApiController]
[Route("api/company/notifications")]
[Authorize]
public class CompanyNotificationsController : ControllerBase
{
    private readonly CompanyListNotificationsAction _listNotificationsAction;
    private readonly CompanyGetUnreadCountAction _getUnreadCountAction;
    private readonly CompanyMarkNotificationReadAction _markNotificationReadAction;
    private readonly CompanyBatchMarkNotificationsReadAction _batchMarkNotificationsReadAction;
    private readonly CompanyMarkAllNotificationsReadAction _markAllNotificationsReadAction;
    private readonly CompanyListNotificationPreferencesAction _listNotificationPreferencesAction;
    private readonly CompanyUpdateNotificationPreferenceAction _updateNotificationPreferenceAction;

    public CompanyNotificationsController(
        CompanyListNotificationsAction listNotificationsAction,
        CompanyGetUnreadCountAction getUnreadCountAction,
        CompanyMarkNotificationReadAction markNotificationReadAction,
        CompanyBatchMarkNotificationsReadAction batchMarkNotificationsReadAction,
        CompanyMarkAllNotificationsReadAction markAllNotificationsReadAction,
        CompanyListNotificationPreferencesAction listNotificationPreferencesAction,
        CompanyUpdateNotificationPreferenceAction updateNotificationPreferenceAction)
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
