using LoadlyAggregator.Domain.Admin.DTOs.Logs;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.Actions.Logs;

public class GetNotificationsLogsAction
{
    private readonly INotificationsLogRepository _notificationsLogRepository;

    public GetNotificationsLogsAction(INotificationsLogRepository notificationsLogRepository)
    {
        _notificationsLogRepository = notificationsLogRepository;
    }

    public async Task<ListingDto<NotificationsLogDto>> ExecuteAsync(GetNotificationsLogsDto dto)
    {
        (List<NotificationsLog> items, int totalCount) = await _notificationsLogRepository.ListAsync(
            dto.Channel,
            dto.Status,
            dto.Recipient,
            modelType: null,
            modelId: null,
            dto.StartDate,
            dto.EndDate,
            dto.Page,
            dto.PageSize);

        return new ListingDto<NotificationsLogDto>
        {
            Data = items.Select(NotificationsLogDto.FromEntity).ToList(),
            Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
        };
    }
}

