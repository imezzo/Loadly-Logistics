using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class TripStatusCountsHelper
{
    public static int GetCompletedCount(IReadOnlyDictionary<TripStatus, int> statusCounts) =>
        statusCounts.GetValueOrDefault(TripStatus.Completed);
}
