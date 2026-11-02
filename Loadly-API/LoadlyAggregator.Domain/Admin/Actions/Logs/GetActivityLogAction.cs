using LoadlyAggregator.Domain.Admin.DTOs.Logs;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Logs;

public class GetActivityLogAction : IAction<GetActivityLogDto, ActivityLogDto>
{
    private readonly IAuditLogRepository _auditLogRepository;

    public GetActivityLogAction(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task<ActivityLogDto> ExecuteAsync(GetActivityLogDto dto)
    {
        var auditLog = await _auditLogRepository.GetByIdAsync(dto.Id)
            ?? throw new NotFoundException("activity_logs.not_found");

        return ActivityLogDto.FromEntity(auditLog);
    }
}
