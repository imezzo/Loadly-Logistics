using LoadlyAggregator.Shared.Time;

namespace LoadlyAggregator.Domain.Company.Actions.Dashboard;

public static class CompanyDashboardDateRangeResolver
{
    public static void Resolve(int? year, bool lastMonth, bool lastWeek, out DateTime? from, out DateTime? to)
    {
        from = null;
        to = null;

        if (lastWeek)
        {
            (from, to) = AppUtcTime.UtcPreviousSaturdayWeekRange(AppUtcTime.UtcTodayAtMidnight(TimeProvider.System));
            return;
        }

        if (lastMonth)
        {
            DateTime now = AppUtcTime.UtcNow(TimeProvider.System);
            DateTime startOfCurrentMonth = new(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            from = startOfCurrentMonth.AddMonths(-1);
            to = startOfCurrentMonth;
            return;
        }

        if (year.HasValue)
        {
            from = new DateTime(year.Value, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            to = from.Value.AddYears(1);
        }
    }

    public static bool HasFilter(int? year, bool lastMonth, bool lastWeek) =>
        lastWeek || lastMonth || year.HasValue;

    public static (DateTime From, DateTime To) GetPreviousPeriod(DateTime from, DateTime to)
    {
        TimeSpan duration = to - from;
        return (from - duration, from);
    }

    public static string BuildHomeCacheKey(long companyId, int? year, bool lastMonth, bool lastWeek)
    {
        string suffix = lastWeek ? "last-week"
            : lastMonth ? "last-month"
            : year.HasValue ? $"year:{year.Value}"
            : "default";

        return $"company:dashboard:home:{companyId}:{suffix}";
    }
}
