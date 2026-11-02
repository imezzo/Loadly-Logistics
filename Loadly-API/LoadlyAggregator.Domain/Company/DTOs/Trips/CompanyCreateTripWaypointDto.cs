using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Company.DTOs.Trips;

public class CompanyCreateTripWaypointDto
{
    public int Sequence { get; set; }
    public TripWaypointStopType StopType { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? AddressName { get; set; }
    public string? GooglePlaceId { get; set; }
    public string? TaskDetails { get; set; }
}
