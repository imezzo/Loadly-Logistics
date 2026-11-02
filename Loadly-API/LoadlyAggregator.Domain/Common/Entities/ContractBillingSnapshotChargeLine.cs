using LoadlyAggregator.Domain.Common.BaseClasses;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Entities;

public class ContractBillingSnapshotChargeLine : BaseEntity
{
    public long SnapshotId { get; set; }
    public long? ContractSlotId { get; set; }
    public DateOnly? Date { get; set; }
    public ContractSlotChargeKind ChargeKind { get; set; }
    public decimal Amount { get; set; }

    public virtual ContractBillingSnapshot? Snapshot { get; set; }
    public virtual ContractSlot? ContractSlot { get; set; }
}
