using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Helpers;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IVehicleFacilityVisitRepository : IRepository<VehicleFacilityVisit>
{
    Task<DateTime?> GetLatestEventDateAsync(long vehicleId);

    Task<IReadOnlyList<VehicleFacilityVisit>> GetForVehicleInPeriodAsync(
        long vehicleId,
        DateTime periodStartUtc,
        DateTime periodEndUtc);

    Task<IReadOnlyList<VehicleFacilityVisit>> GetForContractSlotBillingAsync(
        long contractSlotId,
        long vehicleId,
        DateTime assignedAtUtc,
        DateTime? archivedAtUtc,
        DateTime periodStartUtc,
        DateTime periodEndUtc,
        DateTime? chainAssignedAtUtc = null);

    Task<IReadOnlyDictionary<long, IReadOnlyList<VehicleFacilityVisit>>> GetForContractSlotBillingBySlotsAsync(
        IReadOnlyList<ContractSlotBillingLoadCriteria> slots,
        DateTime globalPeriodEndUtc);
}
