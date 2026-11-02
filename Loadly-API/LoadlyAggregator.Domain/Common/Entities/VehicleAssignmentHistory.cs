using LoadlyAggregator.Domain.Common.BaseClasses;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Entities;

public class VehicleAssignmentHistory : BaseEntity
{
    public long VehicleId { get; set; }
    public VehicleAssignmentHistorySource Source { get; set; }
    public long? PrevTailVehicleId { get; set; }
    public long? NewTailVehicleId { get; set; }
    public long? PrevDriverId { get; set; }
    public long? NewDriverId { get; set; }
    public long? AddedBy { get; set; }

    public virtual Vehicle? Vehicle { get; set; }
    public virtual Vehicle? PrevTailVehicle { get; set; }
    public virtual Vehicle? NewTailVehicle { get; set; }
    public virtual Driver? PrevDriver { get; set; }
    public virtual Driver? NewDriver { get; set; }
    public virtual Admin? AddedByAdmin { get; set; }
}
