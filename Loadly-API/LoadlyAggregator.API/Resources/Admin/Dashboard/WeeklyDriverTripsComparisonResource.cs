using LoadlyAggregator.Domain.Admin.DTOs.Dashboard;

namespace LoadlyAggregator.API.Resources.Admin.Dashboard;

public class WeeklyDriverTripsComparisonResource
{
    public int CurrentWeekAssignedDriverTripsCount { get; set; }
    public int PreviousWeekAssignedDriverTripsCount { get; set; }

    public static WeeklyDriverTripsComparisonResource FromDto(WeeklyDriverTripsComparisonDto dto) => new()
    {
        CurrentWeekAssignedDriverTripsCount = dto.CurrentWeekAssignedDriverTripsCount,
        PreviousWeekAssignedDriverTripsCount = dto.PreviousWeekAssignedDriverTripsCount
    };
}
