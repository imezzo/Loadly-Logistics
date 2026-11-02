using LoadlyAggregator.Domain.Admin.DTOs.TripsManagement;

namespace LoadlyAggregator.API.Requests.Admin.TripsManagement;

public class StartTripRequest
{
    public const string Permission = "self_complete_trip";

    public string? Note { get; set; }

    public StartTripDto FromRequest(long tripId) => new()
    {
        TripId = tripId,
        Note = Note
    };
}
