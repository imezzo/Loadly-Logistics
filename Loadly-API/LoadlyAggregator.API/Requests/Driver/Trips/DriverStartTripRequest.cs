using LoadlyAggregator.Domain.Driver.DTOs.Trips;

namespace LoadlyAggregator.API.Requests.Driver.Trips;

public class DriverStartTripRequest
{
    public string? Note { get; set; }

    public DriverStartTripDto FromRequest(long tripId, long driverId) => new()
    {
        TripId = tripId,
        DriverId = driverId,
        Note = Note
    };
}
