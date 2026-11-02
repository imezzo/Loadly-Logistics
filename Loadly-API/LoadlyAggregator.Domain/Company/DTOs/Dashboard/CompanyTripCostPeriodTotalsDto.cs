namespace LoadlyAggregator.Domain.Company.DTOs.Dashboard;

public class CompanyTripCostPeriodTotalsDto
{
    public decimal TotalCostLastSevenDays { get; set; }
    public decimal TotalCostLastThirtyDays { get; set; }
    public decimal TotalCostLastThreeMonths { get; set; }
}

public class CompanyGetTripCostPeriodTotalsDto
{
    public long CompanyId { get; set; }
}
