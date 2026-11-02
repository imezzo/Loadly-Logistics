using LoadlyAggregator.Domain.Common.DTOs.Notifications;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Contractor.Actions.Notifications;

public class ContractorBatchMarkNotificationsReadAction
{
    private readonly INotificationRepository _notificationRepository;
    private readonly ICurrentUserService _currentUserService;

    public ContractorBatchMarkNotificationsReadAction(
        INotificationRepository notificationRepository,
        ICurrentUserService currentUserService)
    {
        _notificationRepository = notificationRepository;
        _currentUserService = currentUserService;
    }

    public async Task ExecuteAsync(BatchMarkNotificationsReadDto dto)
    {
        var userId = _currentUserService.GetCurrentUserId() ?? throw new UnauthorizedException("auth.user_not_authenticated");
        var userType = _currentUserService.GetCurrentUserType() ?? throw new UnauthorizedException("User type not found");

        await _notificationRepository.MarkAsReadAsync(dto.Ids, userId, userType);
    }
}
