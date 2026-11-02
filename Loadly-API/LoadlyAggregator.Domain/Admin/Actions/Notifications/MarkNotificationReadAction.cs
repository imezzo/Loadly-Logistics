using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Notifications;

public class MarkNotificationReadAction
{
    private readonly INotificationRepository _notificationRepository;
    private readonly ICurrentUserService _currentUserService;

    public MarkNotificationReadAction(
        INotificationRepository notificationRepository,
        ICurrentUserService currentUserService)
    {
        _notificationRepository = notificationRepository;
        _currentUserService = currentUserService;
    }

    public async Task ExecuteAsync(long id)
    {
        var userId = _currentUserService.GetCurrentUserId() ?? throw new UnauthorizedException("auth.user_not_authenticated");
        var userType = _currentUserService.GetCurrentUserType() ?? throw new UnauthorizedException("User type not found");

        await _notificationRepository.MarkAsReadAsync(id, userId, userType);
    }
}
