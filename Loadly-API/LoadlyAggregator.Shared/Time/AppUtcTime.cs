namespace LoadlyAggregator.Shared.Time;

/// <summary>
/// UTC clock and calendar-day helpers. Use with <see cref="TimeProvider"/> for tests.
/// Stored instants and API contract use UTC; use these instead of server-local <see cref="DateTime.Now"/>.
/// </summary>
public static class AppUtcTime
{
    public static DateTime UtcNow(TimeProvider timeProvider) =>
        timeProvider.GetUtcNow().UtcDateTime;

    public static int UtcCalendarYear(TimeProvider timeProvider) =>
        UtcNow(timeProvider).Year;

    /// <summary>UTC midnight for the calendar day of <paramref name="value"/> (Unspecified treated as UTC wall time).</summary>
    public static DateTime UtcDateAtMidnight(DateTime value)
    {
        DateTime utc = value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };
        return new DateTime(utc.Year, utc.Month, utc.Day, 0, 0, 0, DateTimeKind.Utc);
    }

    public static DateTime UtcTodayAtMidnight(TimeProvider timeProvider) =>
        UtcDateAtMidnight(UtcNow(timeProvider));

    /// <summary>UTC midnight for a calendar date from parsed query or form values (components only).</summary>
    public static DateTime UtcStartOfDay(int year, int month, int day) =>
        new(year, month, day, 0, 0, 0, DateTimeKind.Utc);

    /// <summary>UTC midnight at the start of the Saturday-start week containing <paramref name="dateUtc"/>.</summary>
    public static DateTime UtcStartOfSaturdayWeek(DateTime dateUtc)
    {
        DateTime date = UtcDateAtMidnight(dateUtc);
        int daysSinceSaturday = ((int)date.DayOfWeek - (int)DayOfWeek.Saturday + 7) % 7;
        return date.AddDays(-daysSinceSaturday);
    }

    /// <summary>UTC start (inclusive) and end (exclusive) of the Saturday-start week containing <paramref name="dateUtc"/>.</summary>
    public static (DateTime Start, DateTime End) UtcSaturdayWeekRange(DateTime dateUtc)
    {
        DateTime start = UtcStartOfSaturdayWeek(dateUtc);
        return (start, start.AddDays(7));
    }

    /// <summary>UTC date range [from, to) for the Saturday-start week immediately before the week containing <paramref name="dateUtc"/>.</summary>
    public static (DateTime From, DateTime To) UtcPreviousSaturdayWeekRange(DateTime dateUtc)
    {
        DateTime startOfCurrentWeek = UtcStartOfSaturdayWeek(dateUtc);
        return (startOfCurrentWeek.AddDays(-7), startOfCurrentWeek);
    }
}
