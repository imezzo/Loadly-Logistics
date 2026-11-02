using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Time;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class VehicleFacilityVisitRepository(ApplicationDbContext context) : Repository<VehicleFacilityVisit>(context), IVehicleFacilityVisitRepository
{
    public Task<DateTime?> GetLatestEventDateAsync(long vehicleId) =>
        Query()
            .Where(v => v.VehicleId == vehicleId)
            .OrderByDescending(v => v.EventDate)
            .ThenByDescending(v => v.Id)
            .Select(v => (DateTime?)v.EventDate)
            .FirstOrDefaultAsync();

    public async Task<IReadOnlyList<VehicleFacilityVisit>> GetForVehicleInPeriodAsync(
        long vehicleId,
        DateTime periodStartUtc,
        DateTime periodEndUtc) =>
        await _dbSet
            .AsNoTracking()
            .Include(v => v.VehicleFacility)
            .Where(v => v.VehicleId == vehicleId)
            .Where(v => v.DeletedAt == null)
            .Where(v => v.EventDate >= periodStartUtc && v.EventDate < periodEndUtc)
            .OrderBy(v => v.EventDate)
            .ThenBy(v => v.Id)
            .ToListAsync();

    public async Task<IReadOnlyList<VehicleFacilityVisit>> GetForContractSlotBillingAsync(
        long contractSlotId,
        long vehicleId,
        DateTime assignedAtUtc,
        DateTime? archivedAtUtc,
        DateTime periodStartUtc,
        DateTime periodEndUtc,
        DateTime? chainAssignedAtUtc = null)
    {
        var visitsBySlotId = await GetForContractSlotBillingBySlotsAsync(
            [new ContractSlotBillingLoadCriteria(
                contractSlotId,
                vehicleId,
                assignedAtUtc,
                archivedAtUtc,
                chainAssignedAtUtc ?? assignedAtUtc)],
            periodEndUtc);

        return visitsBySlotId.GetValueOrDefault(contractSlotId, []);
    }

    public async Task<IReadOnlyDictionary<long, IReadOnlyList<VehicleFacilityVisit>>> GetForContractSlotBillingBySlotsAsync(
        IReadOnlyList<ContractSlotBillingLoadCriteria> slots,
        DateTime globalPeriodEndUtc)
    {
        if (slots.Count == 0)
            return new Dictionary<long, IReadOnlyList<VehicleFacilityVisit>>();

        var vehicleIds = slots
            .Where(slot => slot.VehicleId is > 0)
            .Select(slot => slot.VehicleId!.Value)
            .Distinct()
            .ToList();
        var earliestAssignedDate = slots.Min(slot => EgyptTime.ToCalendarDate(slot.ChainAssignedAtUtc));
        var rangeStart = EgyptTime.ToUtcHalfOpenRange(earliestAssignedDate, TimeOnly.MinValue, TimeOnly.MinValue).StartUtc;
        var allVisits = await _dbSet
            .AsNoTracking()
            .Include(v => v.VehicleFacility)
            .Where(v => vehicleIds.Contains(v.VehicleId))
            .Where(v => v.DeletedAt == null)
            .Where(v => v.EventDate >= rangeStart && v.EventDate < globalPeriodEndUtc)
            .OrderBy(v => v.EventDate)
            .ThenBy(v => v.Id)
            .ToListAsync();

        return ContractSlotBillingVisitAttribution.PartitionBySlot(slots, allVisits);
    }

    /// <summary>
    /// A visit records where the vehicle physically is, so it is matched on the vehicle and read from the
    /// start of the replacement chain — not from the slot's own assignment, and not restricted to the slot
    /// that happened to be active when the event was written. Otherwise a replacement slot cannot see the
    /// facility stay it was created in the middle of, and bills those days as working days.
    /// </summary>
    private static List<VehicleFacilityVisit> FilterVisitsForSlot(
        IReadOnlyList<VehicleFacilityVisit> visits,
        ContractSlotBillingLoadCriteria slot) =>
        visits
            .Where(v => slot.VehicleId is > 0 && v.VehicleId == slot.VehicleId)
            .Where(v => v.EventDate >= slot.ChainAssignedAtUtc)
            .Where(v => !slot.ArchivedAtUtc.HasValue || v.EventDate < slot.ArchivedAtUtc.Value)
            .ToList();
}
