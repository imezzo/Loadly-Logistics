using LoadlyAggregator.Domain.Admin.DTOs.Dashboard;

namespace LoadlyAggregator.API.Resources.Admin.Dashboard;

public class MonthlyTripCostResource
{
    public int Month { get; set; }
    public string MonthName { get; set; } = string.Empty;
    public int TripsCount { get; set; }
    public decimal TotalCost { get; set; }

    private static readonly string[] MonthNames = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

    public static MonthlyTripCostResource FromDto(MonthlyTripCostDto dto) => new()
    {
        Month = dto.Month,
        MonthName = dto.Month >= 1 && dto.Month <= 12 ? MonthNames[dto.Month - 1] : string.Empty,
        TripsCount = dto.TripsCount,
        TotalCost = dto.TotalCost
    };
}
