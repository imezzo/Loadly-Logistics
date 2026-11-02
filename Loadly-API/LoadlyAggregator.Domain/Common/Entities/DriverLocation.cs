using LoadlyAggregator.Domain.Common.Attributes;
using LoadlyAggregator.Domain.Common.BaseClasses;

namespace LoadlyAggregator.Domain.Common.Entities;

[NoAudit]
public class DriverLocation : BaseEntity
{
    public long DriverId { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public decimal? Accuracy { get; set; }
    public decimal? Heading { get; set; }
    public decimal? Speed { get; set; }
    public DateTime RecordedAt { get; set; }

    public virtual Driver Driver { get; set; } = null!;
}
