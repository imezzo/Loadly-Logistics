using LoadlyAggregator.Domain.Common.BaseClasses;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Entities;

public class ShippingLine : BaseEntity
{
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public ShippingLineStatus Status { get; set; } = ShippingLineStatus.ACTIVE;
    public long? AddedBy { get; set; }
}
