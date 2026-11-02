using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Contractor.DTOs.Trips;

namespace LoadlyAggregator.Domain.Contractor.Actions.Trips;

public class ContractorGetTripInvitationStatisticsAction
{
    private readonly ITripRepository _tripRepository;

    public ContractorGetTripInvitationStatisticsAction(ITripRepository tripRepository)
    {
        _tripRepository = tripRepository;
    }

    public async Task<ContractorTripInvitationStatisticsDto> ExecuteAsync(long contractorId)
    {
        var (openInvitationTrips, pendingBookingRequestTrips) =
            await _tripRepository.GetContractorInviteTripCountsAsync(contractorId);

        return new ContractorTripInvitationStatisticsDto
        {
            OpenInvitationTrips = openInvitationTrips,
            PendingBookingRequestTrips = pendingBookingRequestTrips
        };
    }
}
