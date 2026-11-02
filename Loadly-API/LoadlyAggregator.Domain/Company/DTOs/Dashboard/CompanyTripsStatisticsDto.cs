namespace LoadlyAggregator.Domain.Company.DTOs.Dashboard;

public class CompanyTripsStatisticsDto
{
    public int TotalTrips { get; set; }
    public int ScheduledTrips { get; set; }
    public int InProgressTrips { get; set; }
    public int CanceledTrips { get; set; }
    public int CompletedTrips { get; set; }
    public int OtherTrips { get; set; }
}

public class CompanyGetDashboardTripsStatisticsDto
{
    public long CompanyId { get; set; }
    public int? Year { get; set; }
    public bool LastMonth { get; set; }
    public bool LastWeek { get; set; }
}
