using LoadlyAggregator.Domain.Contractor.DTOs.Trips;

namespace LoadlyAggregator.API.Requests.Contractor.Trips;

public class ContractorCreateBookingRequest
{
    public ContractorCreateBookingRequestDto FromRequest(long tripId, long contractorId) => new()
    {
        TripId = tripId,
        ContractorId = contractorId
    };
}
