using LoadlyAggregator.Domain.Common.BaseClasses;
using LoadlyAggregator.Domain.Common.Enums;
namespace LoadlyAggregator.Domain.Common.Entities;

public class VehicleType : BaseEntity
{
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public VehicleTypeCategory Category { get; set; }
    public VehicleTypeStatus Status { get; set; } = VehicleTypeStatus.ACTIVE;
    public VehicleUnitType? VehicleUnitType { get; set; }
    public long? AddedBy { get; set; }

    public virtual ICollection<VehicleTypeTag> VehicleTypeTags { get; set; } = new List<VehicleTypeTag>();
}
