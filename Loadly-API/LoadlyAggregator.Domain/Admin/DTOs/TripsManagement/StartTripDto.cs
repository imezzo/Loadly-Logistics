using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.DTOs.TripsManagement;

public class StartTripDto
{
    public long TripId { get; set; }
    public string? Note { get; set; }
    public DriverLocationData? CurrentLocation { get; set; }
}
