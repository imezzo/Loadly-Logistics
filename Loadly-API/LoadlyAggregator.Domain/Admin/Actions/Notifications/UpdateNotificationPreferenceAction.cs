using LoadlyAggregator.Domain.Common.DTOs.Notifications;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Notifications;

public class UpdateNotificationPreferenceAction
{
    private readonly INotificationPreferenceService _notificationPreferenceService;
    private readonly ICurrentUserService _currentUserService;

    public UpdateNotificationPreferenceAction(
        INotificationPreferenceService notificationPreferenceService,
        ICurrentUserService currentUserService)
    {
        _notificationPreferenceService = notificationPreferenceService;
        _currentUserService = currentUserService;
    }

    public async Task ExecuteAsync(UpdateNotificationPreferenceDto dto)
    {
        var userId = _currentUserService.GetCurrentUserId() ?? throw new UnauthorizedException("auth.user_not_authenticated");
        await _notificationPreferenceService.SetNotificationPreferenceAsync(
            userId,
            _currentUserService.GetCurrentUserType(),
            dto.Channel,
            dto.EventType,
            dto.IsEnabled);
    }
}
