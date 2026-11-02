using LoadlyAggregator.Domain.Common.BaseClasses;

namespace LoadlyAggregator.Domain.Common.Entities;

public class ContractBillingSnapshotKmEvidence : BaseEntity
{
    public long SnapshotId { get; set; }
    public long? ContractSlotId { get; set; }
    public bool IsPooled { get; set; }
    public decimal Consumption { get; set; }
    public decimal? MaxKms { get; set; }
    public decimal OverageKm { get; set; }
    public decimal OverConsumptionRate { get; set; }
    public decimal Amount { get; set; }

    public virtual ContractBillingSnapshot? Snapshot { get; set; }
    public virtual ContractSlot? ContractSlot { get; set; }
}
