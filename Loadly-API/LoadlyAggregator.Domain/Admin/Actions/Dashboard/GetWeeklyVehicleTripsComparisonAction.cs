using LoadlyAggregator.Domain.Admin.DTOs.Dashboard;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Time;

namespace LoadlyAggregator.Domain.Admin.Actions.Dashboard;

public class GetWeeklyVehicleTripsComparisonAction
{
    private readonly ITripRepository _tripRepository;
    private readonly TimeProvider _timeProvider;

    public GetWeeklyVehicleTripsComparisonAction(ITripRepository tripRepository, TimeProvider timeProvider)
    {
        _tripRepository = tripRepository;
        _timeProvider = timeProvider;
    }

    public async Task<WeeklyVehicleTripsComparisonDto> ExecuteAsync()
    {
        var (currentWeekStart, currentWeekEnd) = AppUtcTime.UtcSaturdayWeekRange(AppUtcTime.UtcNow(_timeProvider));
        var previousWeekStart = currentWeekStart.AddDays(-7);

        return new WeeklyVehicleTripsComparisonDto
        {
            CurrentWeekAssignedVehicleTripsCount = await _tripRepository.GetAssignedVehicleTripsCountInDateRangeAsync(currentWeekStart, currentWeekEnd),
            PreviousWeekAssignedVehicleTripsCount = await _tripRepository.GetAssignedVehicleTripsCountInDateRangeAsync(previousWeekStart, currentWeekStart)
        };
    }
}
