using System.Linq;
using LoadlyAggregator.Domain.Admin.DTOs.Dashboard;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Dashboard;

public class GetMonthlyTripsCostStatisticsAction
{
    private readonly ITripRepository _tripRepository;

    public GetMonthlyTripsCostStatisticsAction(ITripRepository tripRepository)
    {
        _tripRepository = tripRepository;
    }

    public async Task<IReadOnlyList<MonthlyTripCostDto>> ExecuteAsync(GetMonthlyTripsCostDto request)
    {
        var monthlyTripsAndCost = await _tripRepository.GetMonthlyCompletedTripAggregatesAsync(request.Year, request.Month);
        var byMonth = monthlyTripsAndCost.ToDictionary(x => x.Month, x => (x.TripsCount, x.TotalCost));

        IEnumerable<int> months = request.Month.HasValue ? new[] { request.Month.Value } : Enumerable.Range(1, 12);
        return months.Select(m => new MonthlyTripCostDto
        {
            Month = m,
            TripsCount = byMonth.GetValueOrDefault(m).TripsCount,
            TotalCost = byMonth.GetValueOrDefault(m).TotalCost
        }).ToList();
    }
}
