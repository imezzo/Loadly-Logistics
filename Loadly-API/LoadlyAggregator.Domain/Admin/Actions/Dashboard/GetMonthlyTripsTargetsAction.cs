using System.Linq;
using LoadlyAggregator.Domain.Admin.DTOs.Dashboard;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Dashboard;

public class GetMonthlyTripsTargetsAction(ITargetRepository targetRepository, ITripRepository tripRepository)
{
    private readonly ITargetRepository _targetRepository = targetRepository;
    private readonly ITripRepository _tripRepository = tripRepository;

    public async Task<IReadOnlyList<MonthlyTargetDto>> ExecuteAsync(GetMonthlyYearMonthTargetsDto request)
    {
        var targets = (await _targetRepository.GetMonthlyTargetsAsync(request.Year, request.Month))
            .Where(t => t.TargetType == TargetType.Trips)
            .ToList();

        var years = targets.Select(t => t.Year).Distinct().ToList();
        var monthlyCompletedStats = await _tripRepository.GetMonthlyCompletedTripAggregatesForYearsAsync(years);

        var tripsLookup = monthlyCompletedStats
            .ToDictionary(x => (x.Year, x.Month), x => x.TripsCount);

        return targets.Select(t => new MonthlyTargetDto
        {
            Month = t.Month,
            Year = t.Year,
            TargetValue = t.TargetValue,
            TargetType = t.TargetType,
            AchievedValue = tripsLookup.TryGetValue((t.Year, (int)t.Month), out int tripsCount) ? tripsCount : 0m
        }).ToList();
    }
}
