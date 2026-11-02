namespace LoadlyAggregator.Domain.Admin.DTOs.Dashboard;

public class HomeDashboardDto
{
    public HomeTotalTripsDto TotalTrips { get; set; } = new();
    public HomeTripPerformanceDto TripPerformance { get; set; } = new();
    public DriverFleetOverviewDto DriverFleetOverview { get; set; } = new();
    public HomeContractorActivityDto ContractorActivity { get; set; } = new();
}

public class HomeTotalTripsDto
{
    public int Total { get; set; }
    public int Completed { get; set; }
    public int InProgress { get; set; }
    public int Pending { get; set; }
    public int Cancelled { get; set; }
    public PeriodComparisonDto VsYesterday { get; set; } = new();
    public decimal SevenDayAverage { get; set; }
}

public class HomeTripPerformanceDto
{
    public decimal ScheduledRate { get; set; }
    public PeriodComparisonDto ScheduledVsLastWeek { get; set; } = new();
    public decimal InProgressRate { get; set; }
    public decimal CompletedRate { get; set; }
    public PeriodComparisonDto CompletedVsLastWeek { get; set; } = new();
    public decimal AverageTripDurationMinutes { get; set; }
    public int PendingTripsCount { get; set; }
    public decimal CancellationRate { get; set; }
}

public class HomeContractorActivityDto
{
    public int ActiveExternalContractorTrips { get; set; }
    public PeriodComparisonDto VsWeeklyAverage { get; set; } = new();
    public IReadOnlyList<int> Trend { get; set; } = Array.Empty<int>();
}
