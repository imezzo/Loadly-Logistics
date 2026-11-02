using LoadlyAggregator.Domain.Admin.Actions.Dashboard;
using LoadlyAggregator.Domain.Admin.DTOs.Contractors;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Time;

namespace LoadlyAggregator.Domain.Admin.Actions.Contractors;

public class GetContractorDashboardStatisticsAction(
    ITripRepository tripRepository,
    IContractorRepository contractorRepository)
{
    public async Task<ContractorDashboardStatisticsDto> ExecuteAsync(GetContractorDashboardStatisticsDto dto)
    {
        var now = AppUtcTime.UtcNow(TimeProvider.System);
        var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var metrics = await tripRepository.GetContractorDashboardMetricsAsync(dto.ContractorId, monthStart);

        string? contractorFullName = null;
        if (dto.ContractorId is long contractorId)
        {
            contractorFullName = (await contractorRepository.GetByIdOrThrowAsync(contractorId)).FullName;
        }

        return new ContractorDashboardStatisticsDto
        {
            ContractorId = dto.ContractorId,
            ContractorFullName = contractorFullName,
            Trips = new ContractorDashboardTripsMetricDto
            {
                AllTime = metrics.AllTimeTrips,
                CurrentMonth = metrics.CurrentMonthTrips,
                VsLastMonth = ComparisonCalculator.Compare(metrics.CurrentMonthTrips, metrics.PreviousMonthTrips)
            },
            LoadlyProfileMargin = new ContractorDashboardLoadlyProfileMarginDto
            {
                AllTime = metrics.AllTimeMargin,
                CurrentMonth = metrics.CurrentMonthMargin,
                VsLastMonth = ComparisonCalculator.Compare(metrics.CurrentMonthMargin, metrics.PreviousMonthMargin)
            }
        };
    }
}
