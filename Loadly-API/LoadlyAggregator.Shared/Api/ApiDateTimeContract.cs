using LoadlyAggregator.Shared.Time;

namespace LoadlyAggregator.Shared.Api;

/// <summary>
/// Documents how this API represents date and time in JSON and related user-facing text.
/// </summary>
public static class ApiDateTimeContract
{
    /// <summary>
    /// Shown in Swagger. Keep in sync with the <c>AddJsonOptions</c> note in the API <c>Program.cs</c>.
    /// </summary>
    public const string JsonDescription =
        "All date and time instants in JSON (ISO 8601 / DateTime) are in UTC. "
        + "Clients should convert to the end user's time zone (or a clearly stated business time zone) for display.";

    /// <summary>
    /// Notification body clock in Egypt local time, 12-hour with AM/PM.
    /// </summary>
    private const string DisplayFormat = "dd MMM yyyy hh:mm tt";

    /// <summary>
    /// Isolate the left-to-right timestamp so it survives embedding in right-to-left copy.
    /// Without this the bidi algorithm reorders the leading day in Arabic notifications, and
    /// "06 Sep 2026 02:57 PM" is displayed as "Sep 2026 02:57 PM 06".
    /// </summary>
    private const string LeftToRightIsolate = "\u2066";
    private const string PopDirectionalIsolate = "\u2069";

    public static string FormatUtcInstantForDisplay(DateTime? value) =>
        value is null
            ? string.Empty
            : LeftToRightIsolate + EgyptTime.Format(value, DisplayFormat) + PopDirectionalIsolate;
}
