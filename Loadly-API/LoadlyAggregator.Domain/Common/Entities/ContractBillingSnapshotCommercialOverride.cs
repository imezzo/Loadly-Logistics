using LoadlyAggregator.Domain.Common.BaseClasses;

namespace LoadlyAggregator.Domain.Common.Entities;

public class ContractBillingSnapshotCommercialOverride : BaseEntity
{
    public long SnapshotId { get; set; }
    public long SeatRootSlotId { get; set; }
    public DateOnly EffectiveStart { get; set; }
    public DateOnly EffectiveEnd { get; set; }
    public decimal? DailyValue { get; set; }
    public decimal? ExtraWorkingDayFees { get; set; }
    public decimal? OverConsumption { get; set; }
    public string OverrideReason { get; set; } = string.Empty;
    public long? AddedBy { get; set; }

    public virtual ContractBillingSnapshot? Snapshot { get; set; }
    public virtual ContractSlot? SeatRootSlot { get; set; }
}
