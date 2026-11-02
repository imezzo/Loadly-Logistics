using System.Globalization;
using System.Text.Json;

namespace LoadlyAggregator.Shared.Time;

/// <summary>
/// Display conversion from stored UTC instants to Egypt local time (Africa/Cairo).
/// </summary>
public static class EgyptTime
{
    private static readonly TimeZoneInfo Zone = TimeZoneInfo.FindSystemTimeZoneById(ResolveTimeZoneId());

    public static DateTime FromUtc(DateTime value)
    {
        DateTime utc = ToUtc(value);
        return TimeZoneInfo.ConvertTimeFromUtc(utc, Zone);
    }

    /// <summary>Egypt local wall time, e.g. <c>2026-08-02 13:00:00</c>.</summary>
    public static string Format(DateTime? value, string format = "yyyy-MM-dd HH:mm:ss") =>
        value is null
            ? string.Empty
            : FromUtc(value.Value).ToString(format, CultureInfo.InvariantCulture);

    /// <summary>
    /// Calendar date in the configured business time zone for a stored UTC instant
    /// (e.g. <c>2026-08-31T21:00:00Z</c> → <c>2026-09-01</c> when UTC+3).
    /// </summary>
    public static DateOnly ToCalendarDate(DateTime utcInstant)
    {
        DateTime utc = ToUtc(utcInstant);
        DateTime local = TimeZoneInfo.ConvertTimeFromUtc(utc, Zone);
        return DateOnly.FromDateTime(local);
    }

    /// <summary>
    /// Half-open UTC range <c>[start, end)</c> for a business-calendar day in the configured time zone.
    /// </summary>
    public static (DateTime StartUtc, DateTime EndUtc) ToUtcHalfOpenRange(
        DateOnly calendarDate,
        TimeOnly start,
        TimeOnly end)
    {
        DateTime localStart = DateTime.SpecifyKind(calendarDate.ToDateTime(start), DateTimeKind.Unspecified);
        DateOnly endDate = end <= start ? calendarDate.AddDays(1) : calendarDate;
        DateTime localEnd = DateTime.SpecifyKind(endDate.ToDateTime(end), DateTimeKind.Unspecified);

        return (
            TimeZoneInfo.ConvertTimeToUtc(localStart, Zone),
            TimeZoneInfo.ConvertTimeToUtc(localEnd, Zone));
    }

    private static DateTime ToUtc(DateTime value) => value.Kind switch
    {
        DateTimeKind.Utc => value,
        DateTimeKind.Local => value.ToUniversalTime(),
        _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
    };

    private static string ResolveTimeZoneId()
    {
        bool win = OperatingSystem.IsWindows();
        string fallback = win ? "Egypt Standard Time" : "Africa/Cairo";
        try
        {
            using JsonDocument doc = JsonDocument.Parse(
                File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "appsettings.json")));
            return doc.RootElement.GetProperty("EgyptTime")
                .GetProperty(win ? "WindowsTimeZoneId" : "IanaTimeZoneId").GetString() ?? fallback;
        }
        catch
        {
            return fallback;
        }
    }
}
