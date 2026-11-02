using LoadlyAggregator.Domain.Common.DTOs.Notifications;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Contractor.Actions.Notifications;

public class ContractorListNotificationsAction
{
    private readonly INotificationRepository _notificationRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMediaService _mediaService;

    public ContractorListNotificationsAction(
        INotificationRepository notificationRepository,
        ICurrentUserService currentUserService,
        IMediaService mediaService)
    {
        _notificationRepository = notificationRepository;
        _currentUserService = currentUserService;
        _mediaService = mediaService;
    }

    public async Task<ListingDto<NotificationDto>> ExecuteAsync(ListNotificationsDto dto)
    {
        long userId = _currentUserService.GetCurrentUserId() ?? throw new UnauthorizedException("auth.user_not_authenticated");
        string userType = _currentUserService.GetCurrentUserType() ?? throw new UnauthorizedException("User type not found");

        (List<Notification> items, int totalCount) = await _notificationRepository.ListAsync(
            userId,
            userType,
            dto.EventType,
            dto.IsRead,
            dto.Page,
            dto.PageSize,
            dto.CreatedFrom,
            dto.CreatedTo,
            dto.Exclude);

        List<NotificationDto> data = items.ConvertAll(NotificationDto.FromEntity);
        foreach (NotificationDto item in data)
        {
            foreach (NotificationAttachmentDto attachment in item.Attachments)
            {
                attachment.Url = _mediaService.Read(attachment.Path);
            }
        }

        return new ListingDto<NotificationDto>
        {
            Data = data,
            Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
        };
    }
}
