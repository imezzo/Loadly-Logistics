using System.Collections.Generic;
using System.Linq;
using LoadlyAggregator.Domain.Admin.DTOs.Dashboard;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Dashboard;

public class GetMonthlyTargetsAction(ITargetRepository targetRepository, ITripRepository tripRepository)
{
    private readonly ITargetRepository _targetRepository = targetRepository;
    private readonly ITripRepository _tripRepository = tripRepository;

    public async Task<IReadOnlyList<MonthlyTargetDto>> ExecuteAsync(GetMonthlyTargetsDto request)
    {
        var targets = await _targetRepository.GetMonthlyTargetsAsync(request.Year);
        var years = targets.Select(t => t.Year).Distinct().ToList();

        var monthlyCompletedStats = await _tripRepository.GetMonthlyCompletedTripAggregatesForYearsAsync(years);

        var achievedLookup = monthlyCompletedStats
            .ToDictionary(
                x => (x.Year, x.Month),
                x => (x.TripsCount, x.TotalCost));

        return targets.Select(t => new MonthlyTargetDto
        {
            Month = t.Month,
            Year = t.Year,
            TargetValue = t.TargetValue,
            TargetType = t.TargetType,
            AchievedValue = ResolveAchievedValue(t, achievedLookup)
        }).ToList();
    }

    private static decimal ResolveAchievedValue(
        Target target,
        IReadOnlyDictionary<(int Year, int Month), (int TripsCount, decimal TotalCost)> achievedLookup)
    {
        var month = (int)target.Month;
        if (!achievedLookup.TryGetValue((target.Year, month), out var achieved))
        {
            return 0m;
        }

        return target.TargetType switch
        {
            TargetType.Cost => achieved.TotalCost,
            TargetType.Trips => achieved.TripsCount,
            _ => 0m
        };
    }
}
