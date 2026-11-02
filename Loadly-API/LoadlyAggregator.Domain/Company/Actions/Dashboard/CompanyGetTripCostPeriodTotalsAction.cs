using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Company.DTOs.Dashboard;
using LoadlyAggregator.Shared.Time;

namespace LoadlyAggregator.Domain.Company.Actions.Dashboard;

public class CompanyGetTripCostPeriodTotalsAction
{
    private readonly ITripRepository _tripRepository;
    private readonly TimeProvider _timeProvider;

    public CompanyGetTripCostPeriodTotalsAction(ITripRepository tripRepository, TimeProvider timeProvider)
    {
        _tripRepository = tripRepository;
        _timeProvider = timeProvider;
    }

    public async Task<CompanyTripCostPeriodTotalsDto> ExecuteAsync(CompanyGetTripCostPeriodTotalsDto dto)
    {
        DateTime utcNow = AppUtcTime.UtcNow(_timeProvider);
        (decimal lastSeven, decimal lastThirty, decimal lastThreeMonths) =
            await _tripRepository.GetCompanyCompletedTripCostTotalsForRollingPeriodsAsync(dto.CompanyId, utcNow);

        return new CompanyTripCostPeriodTotalsDto
        {
            TotalCostLastSevenDays = lastSeven,
            TotalCostLastThirtyDays = lastThirty,
            TotalCostLastThreeMonths = lastThreeMonths
        };
    }
}
