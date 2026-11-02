using Microsoft.Extensions.Caching.Memory;
using LoadlyAggregator.Domain.Admin.DTOs.Dashboard;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Time;

namespace LoadlyAggregator.Domain.Admin.Actions.Dashboard;

public class GetHomeDashboardAction(
    GetTripsStatisticsAction getTripsStatisticsAction,
    GetTripPerformanceAction getTripPerformanceAction,
    GetDriverFleetOverviewAction getDriverFleetOverviewAction,
    GetContractorActivityAction getContractorActivityAction,
    ITripRepository tripRepository,
    IMemoryCache cache)
{
    private const string CacheKey = "admin:dashboard:home";
    private const int ContractorTrendDays = 7;
    private static readonly TimeSpan CacheTtl = TimeSpan.FromSeconds(45);

    public async Task<HomeDashboardDto> ExecuteAsync()
    {
        if (cache.TryGetValue(CacheKey, out HomeDashboardDto? cached) && cached is not null)
        {
            return cached;
        }

        HomeDashboardDto result = await BuildAsync();
        cache.Set(CacheKey, result, CacheTtl);
        return result;
    }

    private async Task<HomeDashboardDto> BuildAsync()
    {
        DateTime now = AppUtcTime.UtcNow(TimeProvider.System);
        DateTime today = AppUtcTime.UtcTodayAtMidnight(TimeProvider.System);

        // Sequential by design: the actions and repository share a single scoped DbContext,
        // which is not safe for concurrent access.
        var tripsStatistics = await getTripsStatisticsAction.ExecuteAsync(new GetDashboardTripsStatisticsDto());
        var tripPerformance = await getTripPerformanceAction.ExecuteAsync();
        var driverFleetOverview = await getDriverFleetOverviewAction.ExecuteAsync();
        var contractorActivity = await getContractorActivityAction.ExecuteAsync();

        return new HomeDashboardDto
        {
            TotalTrips = await BuildTotalTripsAsync(tripsStatistics, today),
            TripPerformance = await BuildTripPerformanceAsync(tripPerformance, now),
            DriverFleetOverview = driverFleetOverview,
            ContractorActivity = await BuildContractorActivityAsync(contractorActivity, today)
        };
    }

    private async Task<HomeTotalTripsDto> BuildTotalTripsAsync(TripsStatisticsDto stats, DateTime today)
    {
        int todayCount = await tripRepository.GetTripCountInDateRangeAsync(today, today.AddDays(1));
        int yesterdayCount = await tripRepository.GetTripCountInDateRangeAsync(today.AddDays(-1), today);
        int lastSevenDaysCount = await tripRepository.GetTripCountInDateRangeAsync(today.AddDays(-7), today);

        return new HomeTotalTripsDto
        {
            Total = stats.TotalTrips,
            Completed = stats.CompletedTrips,
            InProgress = stats.InProgressTrips,
            Pending = stats.OtherTrips,
            Cancelled = stats.CanceledTrips,
            VsYesterday = ComparisonCalculator.Compare(todayCount, yesterdayCount),
            SevenDayAverage = DashboardTripMetrics.Round(lastSevenDaysCount / 7m)
        };
    }

    private async Task<HomeTripPerformanceDto> BuildTripPerformanceAsync(TripPerformanceDto performance, DateTime now)
    {
        var (thisWeekStart, thisWeekEnd) = AppUtcTime.UtcSaturdayWeekRange(now);
        var (lastWeekStart, lastWeekEnd) = AppUtcTime.UtcPreviousSaturdayWeekRange(now);

        var thisWeekCounts = await tripRepository.GetTripStatusCountsAsync(thisWeekStart, thisWeekEnd);
        var lastWeekCounts = await tripRepository.GetTripStatusCountsAsync(lastWeekStart, lastWeekEnd);

        return new HomeTripPerformanceDto
        {
            ScheduledRate = performance.ScheduledRate,
            ScheduledVsLastWeek = ComparisonCalculator.Compare(
                DashboardTripMetrics.StatusShareRate(thisWeekCounts, TripStatus.Scheduled),
                DashboardTripMetrics.StatusShareRate(lastWeekCounts, TripStatus.Scheduled)),
            InProgressRate = performance.InProgressRate,
            CompletedRate = performance.CompletedRate,
            CompletedVsLastWeek = ComparisonCalculator.Compare(
                DashboardTripMetrics.StatusShareRate(thisWeekCounts, TripStatus.Completed),
                DashboardTripMetrics.StatusShareRate(lastWeekCounts, TripStatus.Completed)),
            AverageTripDurationMinutes = performance.AverageTripDurationMinutes,
            PendingTripsCount = performance.PendingTripsCount,
            CancellationRate = performance.CancellationRate
        };
    }

    private async Task<HomeContractorActivityDto> BuildContractorActivityAsync(ContractorActivityDto contractor, DateTime today)
    {
        DateTime from = today.AddDays(-(ContractorTrendDays - 1));
        DateTime to = today.AddDays(1);

        var dailyCounts = await tripRepository.GetDailyActiveContractorTripCountsAsync(from, to);
        var countsByDate = dailyCounts.ToDictionary(x => x.Date, x => x.Count);

        var trend = Enumerable.Range(0, ContractorTrendDays)
            .Select(offset => countsByDate.GetValueOrDefault(from.AddDays(offset).Date))
            .ToList();

        int todayCount = trend[^1];
        decimal weeklyAverage = DashboardTripMetrics.Round(trend.Sum() / (decimal)ContractorTrendDays);

        return new HomeContractorActivityDto
        {
            ActiveExternalContractorTrips = contractor.ActiveContractorTrips,
            VsWeeklyAverage = ComparisonCalculator.Compare(todayCount, weeklyAverage),
            Trend = trend
        };
    }
}
