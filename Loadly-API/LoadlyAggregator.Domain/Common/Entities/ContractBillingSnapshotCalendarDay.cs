using LoadlyAggregator.Domain.Common.BaseClasses;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Entities;

public class ContractBillingSnapshotCalendarDay : BaseEntity
{
    public long SnapshotId { get; set; }
    public long ContractSlotId { get; set; }
    public DateOnly Date { get; set; }
    public ContractSlotDayDisposition Disposition { get; set; }
    public ContractBillingSnapshotDispositionSource DispositionSource { get; set; }
    public long[] TripIds { get; set; } = [];
    public ContractBillingSnapshotDispositionSource TripCountSource { get; set; }
    public int? WorkingDayIndex { get; set; }
    public bool IsExtra { get; set; }
    public VehicleFacilityType? FacilityType { get; set; }
    public long? FacilityId { get; set; }
    public string? OverrideReason { get; set; }
    public long? ChangedBy { get; set; }
    public DateTime? ChangedAt { get; set; }

    public virtual ContractBillingSnapshot? Snapshot { get; set; }
    public virtual ContractSlot? ContractSlot { get; set; }
    public virtual VehicleFacility? VehicleFacility { get; set; }
}
