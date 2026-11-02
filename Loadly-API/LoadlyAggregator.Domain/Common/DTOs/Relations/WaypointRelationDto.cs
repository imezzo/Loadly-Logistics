namespace LoadlyAggregator.Domain.Common.DTOs.Relations;

public class WaypointRelationDto
{
    public long Id { get; set; }
    public string? GooglePlaceId { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string AddressName { get; set; } = string.Empty;
}
