using LoadlyAggregator.Domain.Common.DTOs.Notifications;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Company.Actions.Notifications;

public class CompanyListNotificationPreferencesAction
{
    private readonly INotificationPreferenceService _notificationPreferenceService;
    private readonly ICurrentUserService _currentUserService;

    public CompanyListNotificationPreferencesAction(
        INotificationPreferenceService notificationPreferenceService,
        ICurrentUserService currentUserService)
    {
        _notificationPreferenceService = notificationPreferenceService;
        _currentUserService = currentUserService;
    }

    public async Task<IReadOnlyList<NotificationPreferenceItemDto>> ExecuteAsync()
    {
        var userId = _currentUserService.GetCurrentUserId() ?? throw new UnauthorizedException("auth.user_not_authenticated");
        return await _notificationPreferenceService.GetEffectivePreferencesAsync(userId, _currentUserService.GetCurrentUserType());
    }
}
