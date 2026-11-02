namespace LoadlyAggregator.Domain.Admin.DTOs.Dashboard;

public class TripsStatisticsDto
{
    public int TotalTrips { get; set; }
    public int ScheduledTrips { get; set; }
    public int InProgressTrips { get; set; }
    public int CanceledTrips { get; set; }
    public int CompletedTrips { get; set; }
    public int OtherTrips { get; set; }
}

public class GetDashboardTripsStatisticsDto
{
    public int? Year { get; set; }
    public bool LastMonth { get; set; }
    public bool LastWeek { get; set; }
}
