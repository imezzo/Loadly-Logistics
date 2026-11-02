using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Contractor.DTOs.Trips;

namespace LoadlyAggregator.Domain.Contractor.Actions.Trips;

public class ContractorGetTripStatusStatisticsAction
{
    private readonly ITripRepository _tripRepository;

    public ContractorGetTripStatusStatisticsAction(ITripRepository tripRepository)
    {
        _tripRepository = tripRepository;
    }

    public async Task<ContractorTripStatusStatisticsDto> ExecuteAsync(long contractorId)
    {
        var statusCountsByStatus = await _tripRepository.GetTripStatusCountsForContractorAsync(contractorId);

        return new ContractorTripStatusStatisticsDto
        {
            CompletedTrips = statusCountsByStatus.GetValueOrDefault(TripStatus.Completed),
            ScheduledTrips = statusCountsByStatus.GetValueOrDefault(TripStatus.Scheduled),
            InProgressTrips = statusCountsByStatus.GetValueOrDefault(TripStatus.InProgress),
            PendingTrips = statusCountsByStatus.GetValueOrDefault(TripStatus.Pending)
        };
    }
}
