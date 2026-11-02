using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Common.Helpers;

internal static class ContractBillingSnapshotChargeRebuildHelper
{
    public static async Task RebuildDraftChargesAsync(
        ContractBillingSnapshot snapshot,
        Contract contract,
        ITripRepository tripRepository)
    {
        var workingDayWindow = ContractWorkingDayWindow.Default;
        var billedSlots = IncludedContractSlotChargesComputer.ResolveTargetSlots(
            contract,
            contractSlotId: null,
            snapshot.PeriodStart,
            snapshot.PeriodEnd);
        var tripsBySlot = await ContractSlotBillingTripLoader.LoadTripsBySlotAsync(
            snapshot.ContractId,
            billedSlots,
            contract.ContractSlots.ToList(),
            snapshot.PeriodStart,
            snapshot.PeriodEnd,
            tripRepository,
            slot => slot);

        ContractBillingSnapshotCalendarChargeRegenerator.RebuildCharges(
            snapshot,
            contract,
            tripsBySlot,
            workingDayWindow);
    }
}
