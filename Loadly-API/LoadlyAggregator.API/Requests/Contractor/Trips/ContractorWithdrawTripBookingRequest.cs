using LoadlyAggregator.Domain.Contractor.DTOs.Trips;

namespace LoadlyAggregator.API.Requests.Contractor.Trips;

public class ContractorWithdrawTripBookingRequest
{
    public ContractorWithdrawTripBookingDto FromRequest(long tripId, long contractorId) => new()
    {
        TripId = tripId,
        ContractorId = contractorId
    };
}
