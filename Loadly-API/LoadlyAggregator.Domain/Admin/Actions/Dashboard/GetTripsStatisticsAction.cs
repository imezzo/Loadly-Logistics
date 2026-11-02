using LoadlyAggregator.Domain.Admin.DTOs.Dashboard;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Time;

namespace LoadlyAggregator.Domain.Admin.Actions.Dashboard;

public class GetTripsStatisticsAction(ITripRepository tripRepository)
{
    private readonly ITripRepository _tripRepository = tripRepository;

    public async Task<TripsStatisticsDto> ExecuteAsync(GetDashboardTripsStatisticsDto request)
    {
        ResolveDateRange(request, out DateTime? from, out DateTime? to);
        var statusCounts = await _tripRepository.GetTripStatusCountsAsync(from, to);

        return new TripsStatisticsDto
        {
            TotalTrips = statusCounts.Values.Sum(),
            ScheduledTrips = statusCounts.GetValueOrDefault(TripStatus.Scheduled),
            InProgressTrips = statusCounts.GetValueOrDefault(TripStatus.InProgress),
            CanceledTrips = statusCounts.GetValueOrDefault(TripStatus.Cancelled),
            CompletedTrips = statusCounts.GetValueOrDefault(TripStatus.Completed),
            OtherTrips = statusCounts.GetValueOrDefault(TripStatus.Pending)
        };
    }

    private static void ResolveDateRange(GetDashboardTripsStatisticsDto request, out DateTime? from, out DateTime? to)
    {
        from = null;
        to = null;

        if (request.LastWeek)
        {
            (from, to) = AppUtcTime.UtcPreviousSaturdayWeekRange(AppUtcTime.UtcTodayAtMidnight(TimeProvider.System));
            return;
        }

        if (request.LastMonth)
        {
            DateTime now = AppUtcTime.UtcNow(TimeProvider.System);
            DateTime startOfCurrentMonth = new(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            from = startOfCurrentMonth.AddMonths(-1);
            to = startOfCurrentMonth;
            return;
        }

        if (request.Year.HasValue)
        {
            int year = request.Year.Value;
            from = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            to = new DateTime(year + 1, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        }
    }
}
