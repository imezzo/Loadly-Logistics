using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Requests.Common.Notifications;
using LoadlyAggregator.API.Requests.Contractor.Notifications;
using LoadlyAggregator.API.Resources.Common;
using LoadlyAggregator.Domain.Common.DTOs.Notifications;
using LoadlyAggregator.Domain.Contractor.Actions.Notifications;

namespace LoadlyAggregator.API.Controllers.Contractor;

[ApiController]
[Route("api/contractor/notifications")]
[Authorize]
public class ContractorNotificationsController : ControllerBase
{
    private readonly ContractorListNotificationsAction _listNotificationsAction;
    private readonly ContractorGetUnreadCountAction _getUnreadCountAction;
    private readonly ContractorMarkNotificationReadAction _markNotificationReadAction;
    private readonly ContractorBatchMarkNotificationsReadAction _batchMarkNotificationsReadAction;
    private readonly ContractorMarkAllNotificationsReadAction _markAllNotificationsReadAction;
    private readonly ContractorListNotificationPreferencesAction _listNotificationPreferencesAction;
    private readonly ContractorUpdateNotificationPreferenceAction _updateNotificationPreferenceAction;

    public ContractorNotificationsController(
        ContractorListNotificationsAction listNotificationsAction,
        ContractorGetUnreadCountAction getUnreadCountAction,
        ContractorMarkNotificationReadAction markNotificationReadAction,
        ContractorBatchMarkNotificationsReadAction batchMarkNotificationsReadAction,
        ContractorMarkAllNotificationsReadAction markAllNotificationsReadAction,
        ContractorListNotificationPreferencesAction listNotificationPreferencesAction,
        ContractorUpdateNotificationPreferenceAction updateNotificationPreferenceAction)
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
