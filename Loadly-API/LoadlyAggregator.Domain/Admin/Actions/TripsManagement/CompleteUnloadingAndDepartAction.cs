using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Domain.Admin.DTOs.TripsManagement;

namespace LoadlyAggregator.Domain.Admin.Actions.TripsManagement;

public class CompleteUnloadingAndDepartAction
{
    private readonly CompleteUnloadingCargoAction _completeUnloadingCargoAction;
    private readonly DepartFromWaypointAction _departFromWaypointAction;

    public CompleteUnloadingAndDepartAction(
        CompleteUnloadingCargoAction completeUnloadingCargoAction,
        DepartFromWaypointAction departFromWaypointAction)
    {
        _completeUnloadingCargoAction = completeUnloadingCargoAction;
        _departFromWaypointAction = departFromWaypointAction;
    }

    public async Task<TripWaypointDto?> ExecuteAsync(CompleteUnloadingAndDepartDto dto)
    {
        await _completeUnloadingCargoAction.ExecuteAsync(new CompleteUnloadingCargoDto
        {
            TripId = dto.TripId,
            WaypointId = dto.WaypointId,
            Note = dto.Note,
            DeliveryNoteImage = dto.DeliveryNoteImage,
            UnloadingImages = dto.UnloadingImages,
            CurrentLocation = dto.CurrentLocation
        });

        return await _departFromWaypointAction.ExecuteAsync(new DepartFromWaypointDto
        {
            TripId = dto.TripId,
            WaypointId = dto.WaypointId,
            Note = dto.Note,
            Odometer = dto.Odometer,
            OdometerImage = dto.OdometerImage,
            CurrentLocation = dto.CurrentLocation
        });
    }
}
