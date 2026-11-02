using LoadlyAggregator.Domain.Admin.DTOs.Trips;

namespace LoadlyAggregator.Domain.Admin.Actions.Trips.Responses;
public class BulkCreateTripsResponse
{
    public BulkTripsSummary Summary { get; set; } = new();
    public List<long> CreatedTripIds { get; set; } = new();
}

public class BulkTripsSummary
{
    public int TotalTrips { get; set; }
    public int SuccessCount { get; set; }
    public int FailedCount { get; set; }
}
