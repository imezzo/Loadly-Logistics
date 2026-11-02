namespace LoadlyAggregator.Domain.Admin.DTOs.Dashboard;

public class TripPerformanceDto
{
    public decimal ScheduledRate { get; set; }
    public decimal InProgressRate { get; set; }
    public decimal CompletedRate { get; set; }
    public decimal AverageTripDurationMinutes { get; set; }
    public int PendingTripsCount { get; set; }
    public decimal CancellationRate { get; set; }
}
