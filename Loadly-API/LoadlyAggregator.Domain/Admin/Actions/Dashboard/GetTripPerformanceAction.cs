using LoadlyAggregator.Domain.Admin.DTOs.Dashboard;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Dashboard;

public class GetTripPerformanceAction(ITripRepository tripRepository)
{
    public async Task<TripPerformanceDto> ExecuteAsync()
    {
        var statusCounts = await tripRepository.GetTripStatusCountsAsync(null, null);
        decimal averageDurationHours = await tripRepository.GetAverageTripDurationHoursAsync();

        return new TripPerformanceDto
        {
            ScheduledRate = DashboardTripMetrics.StatusShareRate(statusCounts, TripStatus.Scheduled),
            InProgressRate = DashboardTripMetrics.StatusShareRate(statusCounts, TripStatus.InProgress),
            CompletedRate = DashboardTripMetrics.StatusShareRate(statusCounts, TripStatus.Completed),
            AverageTripDurationMinutes = DashboardTripMetrics.Round(averageDurationHours * 60m),
            PendingTripsCount = statusCounts.GetValueOrDefault(TripStatus.Pending),
            CancellationRate = DashboardTripMetrics.StatusShareRate(statusCounts, TripStatus.Cancelled)
        };
    }
}
