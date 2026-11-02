using LoadlyAggregator.Domain.Admin.DTOs.Dashboard;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Time;

namespace LoadlyAggregator.Domain.Admin.Actions.Dashboard;

public class GetWeeklyDriverTripsComparisonAction
{
    private readonly ITripRepository _tripRepository;
    private readonly TimeProvider _timeProvider;

    public GetWeeklyDriverTripsComparisonAction(ITripRepository tripRepository, TimeProvider timeProvider)
    {
        _tripRepository = tripRepository;
        _timeProvider = timeProvider;
    }

    public async Task<WeeklyDriverTripsComparisonDto> ExecuteAsync()
    {
        var (currentWeekStart, currentWeekEnd) = AppUtcTime.UtcSaturdayWeekRange(AppUtcTime.UtcNow(_timeProvider));
        var previousWeekStart = currentWeekStart.AddDays(-7);

        return new WeeklyDriverTripsComparisonDto
        {
            CurrentWeekAssignedDriverTripsCount = await _tripRepository.GetAssignedDriverTripsCountInDateRangeAsync(currentWeekStart, currentWeekEnd),
            PreviousWeekAssignedDriverTripsCount = await _tripRepository.GetAssignedDriverTripsCountInDateRangeAsync(previousWeekStart, currentWeekStart)
        };
    }
}
