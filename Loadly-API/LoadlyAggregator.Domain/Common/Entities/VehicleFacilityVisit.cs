using LoadlyAggregator.Domain.Common.Attributes;
using LoadlyAggregator.Domain.Common.BaseClasses;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Entities;

[NoAudit]
public class VehicleFacilityVisit : BaseEntity
{
    public long VehicleId { get; set; }
    public long VehicleFacilityId { get; set; }
    public long? ContractSlotId { get; set; }
    public VehicleFacilityVisitEvent Event { get; set; }
    public VehicleFacilityVisitContext Context { get; set; } = VehicleFacilityVisitContext.Service;
    public DateTime EventDate { get; set; }
    public long? DriverId { get; set; }
    public string? Reason { get; set; }
    public long? AddedBy { get; set; }

    public virtual Vehicle Vehicle { get; set; } = null!;
    public virtual VehicleFacility VehicleFacility { get; set; } = null!;
    public virtual ContractSlot? ContractSlot { get; set; }
    public virtual Driver? Driver { get; set; }
    public virtual Admin? AddedByAdmin { get; set; }
}
