using LoadlyAggregator.Domain.Admin.DTOs.Dashboard;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Admin.Actions.Dashboard;

/// <summary>
/// Computes a current-vs-previous period comparison (delta, percent change, direction)
/// used by the home dashboard for "vs yesterday / last week / weekly average" indicators.
/// </summary>
public static class ComparisonCalculator
{
    public static PeriodComparisonDto Compare(decimal current, decimal previous)
    {
        decimal delta = current - previous;

        decimal percentChange = previous == 0m
            ? (current == 0m ? 0m : 100m)
            : Round(delta / previous * 100m);

        return new PeriodComparisonDto
        {
            Current = current,
            Previous = previous,
            Delta = Round(delta),
            PercentChange = percentChange,
            Direction = delta > 0m
                ? TrendDirection.Up
                : delta < 0m
                    ? TrendDirection.Down
                    : TrendDirection.Flat
        };
    }

    private static decimal Round(decimal value) =>
        Math.Round(value, 2, MidpointRounding.AwayFromZero);
}
