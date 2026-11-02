using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IAuditLogRepository
{
    Task<AuditLog?> GetByIdAsync(long id);
    Task<ListingDto<AuditLog>> ListAsync(
        string? eventType = null,
        string? entityType = null,
        long? userId = null,
        string? userType = null,
        string? correlationId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int page = 1,
        int pageSize = 15);
}
