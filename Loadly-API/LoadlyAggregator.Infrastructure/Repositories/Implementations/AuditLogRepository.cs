using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly DbSet<AuditLog> _dbSet;

    public AuditLogRepository(LogsDbContext context)
    {
        _dbSet = context.AuditLogs;
    }

    public async Task<AuditLog?> GetByIdAsync(long id) =>
        await _dbSet.FirstOrDefaultAsync(x => x.Id == id);

    public async Task<ListingDto<AuditLog>> ListAsync(
        string? eventType = null,
        string? entityType = null,
        long? userId = null,
        string? userType = null,
        string? correlationId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int page = 1,
        int pageSize = 15)
    {
        var normalizedPage = page > 0 ? page : 1;
        var normalizedPageSize = pageSize > 0 ? pageSize : 15;
        var query = _dbSet.AsQueryable()
            .Where(x => string.IsNullOrWhiteSpace(eventType) || x.EventType == eventType)
            .Where(x => string.IsNullOrWhiteSpace(entityType) || x.EntityType.Contains(entityType!))
            .Where(x => !userId.HasValue || x.UserId == userId.Value)
            .Where(x => string.IsNullOrWhiteSpace(userType) || x.UserType == userType)
            .Where(x => string.IsNullOrWhiteSpace(correlationId) || x.CorrelationId == correlationId)
            .Where(x => !startDate.HasValue || x.CreatedAt >= startDate.Value)
            .Where(x => !endDate.HasValue || x.CreatedAt <= endDate.Value);
        var totalRecords = await query.CountAsync();
        var logs = await query
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .Skip((normalizedPage - 1) * normalizedPageSize)
            .Take(normalizedPageSize)
            .ToListAsync();
        return new ListingDto<AuditLog>
        {
            Data = logs,
            Pagination = new PaginationDto(normalizedPage, normalizedPageSize, totalRecords)
        };
    }
}

