using LoadlyAggregator.Domain.Company.DTOs.Trips;

namespace LoadlyAggregator.API.Requests.Company.Trips;

public class CompanyDeleteTripRequest
{
    public const string Permission = "delete_trip";

    public CompanyDeleteTripDto FromRequest(long tripId, long companyId) => new()
    {
        TripId = tripId,
        CompanyId = companyId
    };
}
