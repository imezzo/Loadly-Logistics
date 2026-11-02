using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Infrastructure.Services.Notification;

/// <summary>
/// Centralised placeholder factories for notification templates.
/// Eliminates per-handler duplication of the same dictionary construction.
/// </summary>
internal static class NotificationPlaceholderBuilder
{
    internal static Dictionary<string, string> ForTrip(long tripId, DateTime? startDate, DateTime? endDate) =>
        new()
        {
            ["tripId"] = tripId.ToString(),
            ["startDate"] = ApiDateTimeContract.FormatUtcInstantForDisplay(startDate),
            ["endDate"] = ApiDateTimeContract.FormatUtcInstantForDisplay(endDate)
        };

    internal static Dictionary<string, string> ForTripProgress(
        long tripId, DateTime? startDate, DateTime? endDate, string? waypointName) =>
        new()
        {
            ["tripId"] = tripId.ToString(),
            ["startDate"] = ApiDateTimeContract.FormatUtcInstantForDisplay(startDate),
            ["endDate"] = ApiDateTimeContract.FormatUtcInstantForDisplay(endDate),
            ["waypointName"] = waypointName ?? ""
        };
}
