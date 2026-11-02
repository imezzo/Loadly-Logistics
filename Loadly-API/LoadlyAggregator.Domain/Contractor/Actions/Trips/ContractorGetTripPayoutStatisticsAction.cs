using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Contractor.DTOs.Trips;

namespace LoadlyAggregator.Domain.Contractor.Actions.Trips;

public class ContractorGetTripPayoutStatisticsAction
{
    private readonly ITripRepository _tripRepository;

    public ContractorGetTripPayoutStatisticsAction(ITripRepository tripRepository)
    {
        _tripRepository = tripRepository;
    }

    public async Task<ContractorTripPayoutStatisticsDto> ExecuteAsync(long contractorId)
    {
        var (completedPaidTrips, completedPaidTripsTotalAmount, completedUnpaidTrips, completedUnpaidTripsTotalAmount) =
            await _tripRepository.GetContractorCompletedTripPaymentTotalsAsync(contractorId);

        return new ContractorTripPayoutStatisticsDto
        {
            CompletedPaidTrips = completedPaidTrips,
            CompletedUnpaidTrips = completedUnpaidTrips,
            CompletedPaidTripsTotalAmount = completedPaidTripsTotalAmount,
            CompletedUnpaidTripsTotalAmount = completedUnpaidTripsTotalAmount
        };
    }
}
