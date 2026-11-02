using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Notifications;

public class GetUnreadCountAction
{
    private readonly INotificationRepository _notificationRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetUnreadCountAction(
        INotificationRepository notificationRepository,
        ICurrentUserService currentUserService)
    {
        _notificationRepository = notificationRepository;
        _currentUserService = currentUserService;
    }

    public async Task<int> ExecuteAsync(NotificationEventType? eventType = null, NotificationEventType? exclude = null)
    {
        var userId = _currentUserService.GetCurrentUserId() ?? throw new UnauthorizedException("auth.user_not_authenticated");
        var userType = _currentUserService.GetCurrentUserType() ?? throw new UnauthorizedException("User type not found");

        return await _notificationRepository.GetUnreadCountAsync(userId, userType, eventType, exclude);
    }
}
