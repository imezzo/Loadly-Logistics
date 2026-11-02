using LoadlyAggregator.Domain.Common.BaseClasses;

namespace LoadlyAggregator.Domain.Common.Entities;

public class VehicleTypeTag : BaseEntity
{
    public long VehicleTypeId { get; set; }
    public long TagId { get; set; }
    public long? AddedBy { get; set; }

    public virtual VehicleType VehicleType { get; set; } = null!;
    public virtual Tag Tag { get; set; } = null!;
}
