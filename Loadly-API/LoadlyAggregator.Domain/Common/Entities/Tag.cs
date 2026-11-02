using LoadlyAggregator.Domain.Common.BaseClasses;

namespace LoadlyAggregator.Domain.Common.Entities;

public class Tag : BaseEntity
{
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public bool IsPricing { get; set; } = false;
    public long? AddedBy { get; set; }

    public virtual ICollection<VehicleTypeTag> VehicleTypeTags { get; set; } = new List<VehicleTypeTag>();
}
