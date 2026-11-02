using LoadlyAggregator.Domain.Admin.DTOs.Dashboard;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Dashboard;

public class GetContractorActivityAction(ITripRepository tripRepository)
{
    public async Task<ContractorActivityDto> ExecuteAsync() =>
        new() { ActiveContractorTrips = await tripRepository.GetActiveContractorTripsCountAsync() };
}
