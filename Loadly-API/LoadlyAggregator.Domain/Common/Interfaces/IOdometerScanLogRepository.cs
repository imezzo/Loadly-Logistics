using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IOdometerScanLogRepository
{
    Task AddAsync(OdometerScanLog log);
    Task<int> SaveChangesAsync();
    Task<OdometerScanLog?> GetByIdAsync(long id);
    Task<(List<OdometerScanLog> Items, int TotalCount)> ListAsync(
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
        int pageSize = 15);
}
