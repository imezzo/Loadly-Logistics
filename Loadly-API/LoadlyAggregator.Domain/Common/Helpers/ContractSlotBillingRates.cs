using LoadlyAggregator.Domain.Common.Entities;

namespace LoadlyAggregator.Domain.Common.Helpers;

/// <summary>
/// Optional per-day commercial rates for snapshot billing; unset fields fall back to the slot.
/// </summary>
public readonly record struct ContractSlotBillingRates(
    decimal? DailyValue,
    decimal? ExtraWorkingDayFees,
    decimal? OverConsumption)
{
    public static ContractSlotBillingRates FromOverride(ContractBillingSnapshotCommercialOverride overrideRow) =>
        new(overrideRow.DailyValue, overrideRow.ExtraWorkingDayFees, overrideRow.OverConsumption);

    public decimal ResolveDailyValue(ContractSlot slot) => DailyValue ?? slot.DailyValue ?? 0m;

    public decimal ResolveExtraWorkingDayAmount(ContractSlot slot) =>
        ExtraWorkingDayFees ?? slot.ExtraWorkingDayFees ?? 0m;
}
