namespace LoadlyAggregator.Domain.Admin.DTOs.Dashboard;

public class GetMonthlyTripsCostDto
{
    public int Year { get; set; }
    public int? Month { get; set; }
}

public class MonthlyTripCostDto
{
    public int Month { get; set; }
    public int TripsCount { get; set; }
    public decimal TotalCost { get; set; }
}
