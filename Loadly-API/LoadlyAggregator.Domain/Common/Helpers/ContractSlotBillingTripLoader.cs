using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class ContractSlotBillingTripLoader
{
    public static async Task<IReadOnlyDictionary<long, IReadOnlyList<Trip>>> LoadTripsBySlotAsync(
        long contractId,
        IReadOnlyList<ContractSlot> billedSlots,
        IReadOnlyList<ContractSlot> allContractSlots,
        DateOnly periodStart,
        DateOnly periodEnd,
        ITripRepository tripRepository,
        Func<ContractSlot, ContractSlot> toBillingSlot)
    {
        var allSlotsById = allContractSlots.ToDictionary(slot => slot.Id);
        var seats = IncludedContractSlotSeatHelper.GroupIntoSeats(billedSlots);
        var loadCriteria = new List<ContractSlotBillingLoadCriteria>();

        foreach (var seat in seats)
        {
            foreach (var slot in seat.SlotsInOrder)
            {
                if (!ContractSlotBillingLoadCriteriaBuilder.ShouldIncludeSlot(slot))
                    continue;

                var billingSlot = toBillingSlot(slot);
                var chainAssignedAt = IncludedContractSlotSeatHelper.ResolveChainAssignedAt(slot, allSlotsById);
                loadCriteria.Add(ContractSlotBillingLoadCriteriaBuilder.Create(
                    slot,
                    billingSlot,
                    chainAssignedAt));
            }
        }

        return await tripRepository.GetForContractSlotBillingBySlotsAsync(
            contractId,
            loadCriteria,
            periodStart,
            periodEnd);
    }
}
