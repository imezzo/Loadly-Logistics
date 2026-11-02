namespace LoadlyAggregator.Domain.Company.DTOs.Maps;

public class GoogleMapsPlaceDto
{
    public string? Url { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? Address { get; set; }
    public string? PlaceId { get; set; }
}
