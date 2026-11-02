using LoadlyAggregator.Domain.Common.Models;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Driver.DTOs.Trips;

public class DriverStartTripDto
{
    public long TripId { get; set; }
    public long DriverId { get; set; }
    public string? Note { get; set; }
    public DriverLocationData? CurrentLocation { get; set; }
}
