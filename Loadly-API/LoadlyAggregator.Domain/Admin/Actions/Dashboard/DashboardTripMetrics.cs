using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Admin.Actions.Dashboard;

/// <summary>
/// Single source of truth for how trip statuses map onto dashboard performance metrics.
/// Rates are expressed as the share (%) of all trips that fall into the given status(es),
/// keeping the trip-performance card definitions consistent across dashboard actions.
/// </summary>
public static class DashboardTripMetrics
{
    public static int Total(IReadOnlyDictionary<TripStatus, int> statusCounts) =>
        statusCounts.Values.Sum();

    public static decimal StatusShareRate(IReadOnlyDictionary<TripStatus, int> statusCounts, params TripStatus[] statuses)
    {
        int total = Total(statusCounts);
        if (total == 0)
        {
            return 0m;
        }

        int matched = statuses.Sum(status => statusCounts.GetValueOrDefault(status));
        return Round(matched * 100m / total);
    }

    public static decimal Round(decimal value) =>
        Math.Round(value, 2, MidpointRounding.AwayFromZero);
}
