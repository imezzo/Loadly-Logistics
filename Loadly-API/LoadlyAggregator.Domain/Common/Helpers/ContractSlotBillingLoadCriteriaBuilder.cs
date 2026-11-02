using LoadlyAggregator.Domain.Common.Entities;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class ContractSlotBillingLoadCriteriaBuilder
{
    /// <summary>
    /// Archived slots may have <see cref="ContractSlot.VehicleId"/> cleared while trips remain pinned via
    /// <see cref="Trip.ContractSlotId"/>; those rows must still participate in billing load.
    /// </summary>
    public static bool ShouldIncludeSlot(ContractSlot slot) =>
        slot.VehicleId is > 0 || slot.ArchivedAt.HasValue;

    public static ContractSlotBillingLoadCriteria Create(
        ContractSlot slot,
        ContractSlot billingSlot,
        DateTime chainAssignedAt)
    {
        var chainStart = chainAssignedAt < billingSlot.AssignedAt ? chainAssignedAt : billingSlot.AssignedAt;
        return new ContractSlotBillingLoadCriteria(
            slot.Id,
            slot.VehicleId,
            billingSlot.AssignedAt,
            slot.ArchivedAt,
            chainStart);
    }
}
