using LoadlyAggregator.Domain.Company.DTOs.Dashboard;

namespace LoadlyAggregator.API.Resources.Company.Dashboard;

public class CompanyMonthlyTripKilometersResource
{
    public int Month { get; set; }
    public string MonthName { get; set; } = string.Empty;
    public int TotalTrips { get; set; }
    public decimal TotalKilometers { get; set; }

    private static readonly string[] MonthNames =
    {
        "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
    };

    public static CompanyMonthlyTripKilometersResource FromDto(CompanyMonthlyTripKilometersDto dto) => new()
    {
        Month = dto.Month,
        MonthName = dto.Month >= 1 && dto.Month <= 12 ? MonthNames[dto.Month - 1] : string.Empty,
        TotalTrips = dto.TotalTrips,
        TotalKilometers = dto.TotalKilometers
    };
}
