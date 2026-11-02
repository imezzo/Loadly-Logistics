using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IContractBillingSnapshotRepository : IRepository<ContractBillingSnapshot>
{
    Task<bool> HasOverlappingPeriodForContractAsync(
        long contractId,
        DateOnly periodStart,
        DateOnly periodEnd,
        long? excludeSnapshotId = null);

    Task<(List<ContractBillingSnapshot> Snapshots, int TotalCount)> ListAsync(
        string? search,
        long? contractId,
        ContractBillingSnapshotStatus? status,
        DateOnly? periodStart,
        DateOnly? periodEnd,
        int page,
        int pageSize);

    Task<ContractBillingSnapshot> GetByIdWithDetailsOrThrowAsync(long id);

    Task<ContractBillingSnapshotCalendarDay?> GetCalendarDayAsync(long snapshotId, long calendarDayId);
}
