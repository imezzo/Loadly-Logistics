namespace LoadlyAggregator.Domain.Company.DTOs.Dashboard;

public class CompanyGetDashboardMonthlyTripsDto
{
    public long CompanyId { get; set; }
    public int Year { get; set; }
    public int? Month { get; set; }
}
