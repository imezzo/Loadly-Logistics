using LoadlyAggregator.Domain.Common.Entities;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class ContractBillingSnapshotCommercialTermsResolver
{
    public static ContractBillingSnapshotCommercialOverride? FindForSeatOnDate(
        IReadOnlyList<ContractBillingSnapshotCommercialOverride> overrides,
        long seatRootSlotId,
        DateOnly date) =>
        overrides
            .Where(row => row.DeletedAt == null && row.SeatRootSlotId == seatRootSlotId)
            .FirstOrDefault(row => date >= row.EffectiveStart && date <= row.EffectiveEnd);

    public static ContractSlotBillingRates? ResolveRatesForDate(
        IReadOnlyList<ContractBillingSnapshotCommercialOverride> overrides,
        long seatRootSlotId,
        DateOnly date)
    {
        var active = FindForSeatOnDate(overrides, seatRootSlotId, date);
        return active is null ? null : ContractSlotBillingRates.FromOverride(active);
    }

    public static void ValidateEffectiveRange(
        ContractBillingSnapshot snapshot,
        DateOnly effectiveStart,
        DateOnly effectiveEnd)
    {
        var periodStart = snapshot.PeriodStart;
        var periodEnd = snapshot.PeriodEnd;
        if (effectiveStart < periodStart || effectiveEnd > periodEnd)
            throw new Shared.Exceptions.ValidationException("contract_billing_snapshot.commercial_override_outside_period");

        if (effectiveEnd < effectiveStart)
            throw new Shared.Exceptions.ValidationException("contract_billing_snapshot.commercial_override_invalid_range");
    }

    public static long ResolveSeatRootSlotId(
        long seatRootSlotId,
        IReadOnlyList<ContractSlot> contractSlots)
    {
        var slotsById = contractSlots.ToDictionary(slot => slot.Id);
        if (!slotsById.TryGetValue(seatRootSlotId, out var slot))
            throw new Shared.Exceptions.NotFoundException("contracts.contract_slot_not_found", seatRootSlotId);

        return IncludedContractSlotSeatHelper.ResolveSeatRootId(slot, slotsById);
    }
}
