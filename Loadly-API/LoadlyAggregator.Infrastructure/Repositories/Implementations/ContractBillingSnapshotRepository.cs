using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class ContractBillingSnapshotRepository : Repository<ContractBillingSnapshot>, IContractBillingSnapshotRepository
{
    public ContractBillingSnapshotRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> HasOverlappingPeriodForContractAsync(
        long contractId,
        DateOnly periodStart,
        DateOnly periodEnd,
        long? excludeSnapshotId = null) =>
        await _dbSet
            .AsNoTracking()
            .Where(snapshot => snapshot.DeletedAt == null)
            .Where(snapshot => snapshot.ContractId == contractId)
            .Where(snapshot => !excludeSnapshotId.HasValue || snapshot.Id != excludeSnapshotId.Value)
            .Where(snapshot => snapshot.PeriodStart <= periodEnd && snapshot.PeriodEnd >= periodStart)
            .AnyAsync();

    public async Task<(List<ContractBillingSnapshot> Snapshots, int TotalCount)> ListAsync(
        string? search,
        long? contractId,
        ContractBillingSnapshotStatus? status,
        DateOnly? periodStart,
        DateOnly? periodEnd,
        int page,
        int pageSize)
    {
        var filtered = _dbSet
            .AsNoTracking()
            .Where(snapshot => snapshot.DeletedAt == null)
            .Where(snapshot => string.IsNullOrWhiteSpace(search) || snapshot.Name.Contains(search!))
            .Where(snapshot => !contractId.HasValue || snapshot.ContractId == contractId.Value)
            .Where(snapshot => !status.HasValue || snapshot.Status == status.Value)
            .Where(snapshot => !periodStart.HasValue || snapshot.PeriodStart >= periodStart.Value)
            .Where(snapshot => !periodEnd.HasValue || snapshot.PeriodEnd <= periodEnd.Value);

        var totalCount = await filtered.CountAsync();
        var snapshots = await filtered
            .OrderByDescending(snapshot => snapshot.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(snapshot => snapshot.CalendarDays.Where(day => day.DeletedAt == null))
                .ThenInclude(day => day.VehicleFacility)
            .Include(snapshot => snapshot.ChargeLines.Where(line => line.DeletedAt == null))
            .Include(snapshot => snapshot.KmEvidence.Where(km => km.DeletedAt == null))
            .Include(snapshot => snapshot.CommercialOverrides.Where(row => row.DeletedAt == null))
            .AsSplitQuery()
            .ToListAsync();

        return (snapshots, totalCount);
    }

    public async Task<ContractBillingSnapshot> GetByIdWithDetailsOrThrowAsync(long id) =>
        await QueryByIdWithDetails(id).FirstOrDefaultAsync()
        ?? throw new NotFoundException("contract_billing_snapshot.not_found", id);

    private IQueryable<ContractBillingSnapshot> QueryByIdWithDetails(long id) =>
        _dbSet
            .Include(snapshot => snapshot.CalendarDays.Where(day => day.DeletedAt == null))
                .ThenInclude(day => day.VehicleFacility)
            .Include(snapshot => snapshot.ChargeLines.Where(line => line.DeletedAt == null))
            .Include(snapshot => snapshot.KmEvidence.Where(km => km.DeletedAt == null))
            .Include(snapshot => snapshot.CommercialOverrides.Where(row => row.DeletedAt == null))
            .AsSplitQuery()
            .Where(snapshot => snapshot.Id == id && snapshot.DeletedAt == null);

    public async Task<ContractBillingSnapshotCalendarDay?> GetCalendarDayAsync(long snapshotId, long calendarDayId) =>
        await _context.Set<ContractBillingSnapshotCalendarDay>()
            .FirstOrDefaultAsync(day =>
                day.Id == calendarDayId
                && day.SnapshotId == snapshotId
                && day.DeletedAt == null);
}
