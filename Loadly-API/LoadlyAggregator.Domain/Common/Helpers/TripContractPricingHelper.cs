using LoadlyAggregator.Domain.Common.Entities;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class TripContractPricingHelper
{
    public static ContractTripPricingTerms Resolve(Contract contract, Trip trip, ContractVehicleType contractVehicleType)
    {
        if (TripContractSlotHelper.ShouldDeriveContractSlot(contract)
            && trip.ContractSlotId is long slotId)
        {
            var slot = contract.ContractSlots.FirstOrDefault(s => s.Id == slotId);
            if (slot is not null)
                return ContractTripPricingTerms.FromSlot(slot);
        }

        return ContractTripPricingTerms.FromVehicleType(contractVehicleType);
    }
}
