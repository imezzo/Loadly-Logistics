using LoadlyAggregator.Domain.Admin.DTOs.Dashboard;

namespace LoadlyAggregator.API.Resources.Admin.Dashboard;

public class MonthlyTripsAvailableDriversResource
{
    public int Month { get; set; }
    public int TripsCount { get; set; }
    public int AvailableDriversCount { get; set; }

    public static MonthlyTripsAvailableDriversResource FromDto(MonthlyTripsAvailableDriversDto dto) => new()
    {
        Month = dto.Month,
        TripsCount = dto.TripsCount,
        AvailableDriversCount = dto.AvailableDriversCount
    };
}
