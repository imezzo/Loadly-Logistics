using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class OdometerScanLogRepository(LogsDbContext context) : IOdometerScanLogRepository
{
    private readonly DbSet<OdometerScanLog> _dbSet = context.OdometerScanLogs;

    public async Task AddAsync(OdometerScanLog log) =>
        await _dbSet.AddAsync(log);

    public async Task<int> SaveChangesAsync() =>
        await context.SaveChangesAsync();

    public async Task<OdometerScanLog?> GetByIdAsync(long id) =>
        await _dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

    public async Task<(List<OdometerScanLog> Items, int TotalCount)> ListAsync(
        long? tripId = null,
        long? modelId = null,
        string? modelType = null,
        OdometerScanStatus? status = null,
        OdometerScanContext? scanContext = null,
        bool? isScreen = null,
        string? correlationId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int page = 1,
        int pageSize = 15)
    {
        int normalizedPage = page > 0 ? page : 1;
        int normalizedPageSize = pageSize > 0 ? pageSize : 15;

        IQueryable<OdometerScanLog> query = _dbSet.AsQueryable()
            .Where(x => !tripId.HasValue || x.TripId == tripId.Value)
            .Where(x => !modelId.HasValue || x.ModelId == modelId.Value)
            .Where(x => string.IsNullOrWhiteSpace(modelType) || x.ModelType == modelType)
            .Where(x => !status.HasValue || x.Status == status.Value)
            .Where(x => !scanContext.HasValue || x.ScanContext == scanContext.Value)
            .Where(x => !isScreen.HasValue || x.IsScreen == isScreen.Value)
            .Where(x => string.IsNullOrWhiteSpace(correlationId) || x.CorrelationId == correlationId)
            .Where(x => !startDate.HasValue || x.CreatedAt >= startDate.Value)
            .Where(x => !endDate.HasValue || x.CreatedAt <= endDate.Value);

        int totalCount = await query.CountAsync();
        List<OdometerScanLog> items = await query
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .Skip((normalizedPage - 1) * normalizedPageSize)
            .Take(normalizedPageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}
