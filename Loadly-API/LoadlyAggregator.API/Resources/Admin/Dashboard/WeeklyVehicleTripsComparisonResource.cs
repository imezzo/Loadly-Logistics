using LoadlyAggregator.Domain.Admin.DTOs.Dashboard;

namespace LoadlyAggregator.API.Resources.Admin.Dashboard;

public class WeeklyVehicleTripsComparisonResource
{
    public int CurrentWeekAssignedVehicleTripsCount { get; set; }
    public int PreviousWeekAssignedVehicleTripsCount { get; set; }

    public static WeeklyVehicleTripsComparisonResource FromDto(WeeklyVehicleTripsComparisonDto dto) => new()
    {
        CurrentWeekAssignedVehicleTripsCount = dto.CurrentWeekAssignedVehicleTripsCount,
        PreviousWeekAssignedVehicleTripsCount = dto.PreviousWeekAssignedVehicleTripsCount
    };
}
