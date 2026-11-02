using LoadlyAggregator.Domain.Company.DTOs.Dashboard;

namespace LoadlyAggregator.API.Resources.Company.Dashboard;

public class CompanyTripCostPeriodTotalsResource
{
    public decimal TotalCostLastSevenDays { get; set; }
    public decimal TotalCostLastThirtyDays { get; set; }
    public decimal TotalCostLastThreeMonths { get; set; }

    public static CompanyTripCostPeriodTotalsResource FromDto(CompanyTripCostPeriodTotalsDto dto) => new()
    {
        TotalCostLastSevenDays = dto.TotalCostLastSevenDays,
        TotalCostLastThirtyDays = dto.TotalCostLastThirtyDays,
        TotalCostLastThreeMonths = dto.TotalCostLastThreeMonths
    };
}
