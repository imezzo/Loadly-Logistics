using LoadlyAggregator.Domain.Common.BaseClasses;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Entities;

public class Port : BaseEntity
{
    public string NameAr { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public string Country { get; set; } = string.Empty;
    public string? PortCode { get; set; }
    public PortType Type { get; set; }
    public PortStatus Status { get; set; } = PortStatus.ACTIVE;
    public long? AddedBy { get; set; }
}
