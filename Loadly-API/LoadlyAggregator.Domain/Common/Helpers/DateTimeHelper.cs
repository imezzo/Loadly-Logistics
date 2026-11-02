namespace LoadlyAggregator.Domain.Common.Helpers;

public static class DateTimeHelper
{
    public static DateTime? ToUtcNullable(DateTime? value)
    {
        if (!value.HasValue) return null;
        return value.Value.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc)
            : value.Value.ToUniversalTime();
    }
}
