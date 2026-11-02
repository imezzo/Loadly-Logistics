using System.Linq;
using LoadlyAggregator.Domain.Admin.DTOs.Dashboard;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Dashboard;

public class GetMonthlyCostTargetsAction(ITargetRepository targetRepository, ITripRepository tripRepository)
{
    private readonly ITargetRepository _targetRepository = targetRepository;
    private readonly ITripRepository _tripRepository = tripRepository;

    public async Task<IReadOnlyList<MonthlyTargetDto>> ExecuteAsync(GetMonthlyYearMonthTargetsDto request)
    {
        var targets = (await _targetRepository.GetMonthlyTargetsAsync(request.Year, request.Month))
            .Where(t => t.TargetType == TargetType.Cost)
            .ToList();

        var years = targets.Select(t => t.Year).Distinct().ToList();
        var monthlyCompletedStats = await _tripRepository.GetMonthlyCompletedTripAggregatesForYearsAsync(years);

        var costLookup = monthlyCompletedStats
            .ToDictionary(x => (x.Year, x.Month), x => x.TotalCost);

        return targets.Select(t => new MonthlyTargetDto
        {
            Month = t.Month,
            Year = t.Year,
            TargetValue = t.TargetValue,
            TargetType = t.TargetType,
            AchievedValue = costLookup.TryGetValue((t.Year, (int)t.Month), out decimal totalCost) ? totalCost : 0m
        }).ToList();
    }
}
