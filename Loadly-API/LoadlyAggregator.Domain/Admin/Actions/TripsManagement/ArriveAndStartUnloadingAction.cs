using LoadlyAggregator.Domain.Admin.DTOs.TripsManagement;

namespace LoadlyAggregator.Domain.Admin.Actions.TripsManagement;

public class ArriveAndStartUnloadingAction
{
    private readonly ArriveAtWaypointAction _arriveAtWaypointAction;
    private readonly StartUnloadingCargoAction _startUnloadingCargoAction;

    public ArriveAndStartUnloadingAction(
        ArriveAtWaypointAction arriveAtWaypointAction,
        StartUnloadingCargoAction startUnloadingCargoAction)
    {
        _arriveAtWaypointAction = arriveAtWaypointAction;
        _startUnloadingCargoAction = startUnloadingCargoAction;
    }

    public async Task ExecuteAsync(ArriveAndStartUnloadingDto dto)
    {
        await _arriveAtWaypointAction.ExecuteAsync(new ArriveAtWaypointDto
        {
            TripId = dto.TripId,
            WaypointId = dto.WaypointId,
            CurrentLocation = dto.CurrentLocation
        });

        await _startUnloadingCargoAction.ExecuteAsync(new StartUnloadingCargoDto
        {
            TripId = dto.TripId,
            WaypointId = dto.WaypointId,
            CurrentLocation = dto.CurrentLocation
        });
    }
}
