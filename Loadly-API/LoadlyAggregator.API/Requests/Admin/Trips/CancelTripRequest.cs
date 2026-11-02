using LoadlyAggregator.Domain.Admin.DTOs.Trips;

namespace LoadlyAggregator.API.Requests.Admin.Trips;

public class CancelTripRequest
{
    public const string Permission = "cancel_trip";

    public string? CancellationReason { get; set; }

    public CancelTripDto FromRequest(long id) => new()
    {
        Id = id,
        CancellationReason = CancellationReason
    };
}
