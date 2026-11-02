namespace LoadlyAggregator.Domain.Company.DTOs.Dashboard;

public class CompanyMonthlyTripKilometersDto
{
    public int Month { get; set; }
    public int TotalTrips { get; set; }
    public decimal TotalKilometers { get; set; }
}
