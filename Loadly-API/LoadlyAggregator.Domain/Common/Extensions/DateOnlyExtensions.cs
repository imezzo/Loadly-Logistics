namespace LoadlyAggregator.Domain.Common.Extensions;

public static class DateOnlyExtensions
{
    /// <summary>
    /// Returns the half-open UTC instant range <c>[start, end)</c> that covers the whole calendar day.
    /// Designed for <c>WHERE column &gt;= start AND column &lt; end</c> filters against a
    /// <see cref="DateTime"/> column stored as UTC (<c>timestamptz</c>).
    /// </summary>
    public static (DateTime Start, DateTime End) ToUtcDayRange(this DateOnly date)
    {
        DateTime start = date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        DateTime end = date.AddDays(1).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        return (start, end);
    }
}
