using LoadlyAggregator.Domain.Common.BaseClasses;

namespace LoadlyAggregator.Domain.Common.Entities;

public class Waypoint : BaseEntity
{
    public string? GooglePlaceId { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string AddressName { get; set; } = string.Empty;
    public long? AddedBy { get; set; }
}
