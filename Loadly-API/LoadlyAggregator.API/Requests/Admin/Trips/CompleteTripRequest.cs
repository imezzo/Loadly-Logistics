using LoadlyAggregator.Domain.Admin.DTOs.Trips;

namespace LoadlyAggregator.API.Requests.Admin.Trips;

public class CompleteTripRequest
{
    public const string Permission = "complete_trip";

    public long Id { get; set; }

    public CompleteTripDto FromRequest() => new()
    {
        Id = Id
    };
}
