using LoadlyAggregator.Domain.Admin.DTOs.Logs;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.Actions.Logs;

public class GetActivityLogsAction : IAction<GetActivityLogsDto, ListingDto<ActivityLogDto>>
{
    private readonly IAuditLogRepository _auditLogRepository;

    public GetActivityLogsAction(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task<ListingDto<ActivityLogDto>> ExecuteAsync(GetActivityLogsDto dto)
    {
        var result = await _auditLogRepository.ListAsync(
            dto.EventType,
            dto.EntityType,
            dto.UserId,
            dto.UserType,
            dto.CorrelationId,
            dto.StartDate,
            dto.EndDate,
            dto.Page,
            dto.PageSize);

        return new ListingDto<ActivityLogDto>
        {
            Data = result.Data.Select(ActivityLogDto.FromEntity).ToList(),
            Pagination = result.Pagination
        };
    }
}
