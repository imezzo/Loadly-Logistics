using LoadlyAggregator.Domain.Admin.DTOs.TripsManagement;

namespace LoadlyAggregator.Domain.Admin.Actions.TripsManagement;

public class ArriveAndStartLoadingAction
{
    private readonly ArriveAtWaypointAction _arriveAtWaypointAction;
    private readonly StartLoadingCargoAction _startLoadingCargoAction;

    public ArriveAndStartLoadingAction(
        ArriveAtWaypointAction arriveAtWaypointAction,
        StartLoadingCargoAction startLoadingCargoAction)
    {
        _arriveAtWaypointAction = arriveAtWaypointAction;
        _startLoadingCargoAction = startLoadingCargoAction;
    }

    public async Task ExecuteAsync(ArriveAndStartLoadingDto dto)
    {
        await _arriveAtWaypointAction.ExecuteAsync(new ArriveAtWaypointDto
        {
            TripId = dto.TripId,
            WaypointId = dto.WaypointId,
            CurrentLocation = dto.CurrentLocation
        });

        await _startLoadingCargoAction.ExecuteAsync(new StartLoadingCargoDto
        {
            TripId = dto.TripId,
            WaypointId = dto.WaypointId,
            CurrentLocation = dto.CurrentLocation
        });
    }
}
