using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Domain.Admin.DTOs.TripsManagement;

namespace LoadlyAggregator.Domain.Admin.Actions.TripsManagement;

public class CompleteLoadingAndDepartAction
{
    private readonly CompleteLoadingCargoAction _completeLoadingCargoAction;
    private readonly DepartFromWaypointAction _departFromWaypointAction;

    public CompleteLoadingAndDepartAction(
        CompleteLoadingCargoAction completeLoadingCargoAction,
        DepartFromWaypointAction departFromWaypointAction)
    {
        _completeLoadingCargoAction = completeLoadingCargoAction;
        _departFromWaypointAction = departFromWaypointAction;
    }

    public async Task<TripWaypointDto?> ExecuteAsync(CompleteLoadingAndDepartDto dto)
    {
        await _completeLoadingCargoAction.ExecuteAsync(new CompleteLoadingCargoDto
        {
            TripId = dto.TripId,
            WaypointId = dto.WaypointId,
            Note = dto.Note,
            ReceiptNoteImage = dto.ReceiptNoteImage,
            ShipmentImages = dto.ShipmentImages,
            CurrentLocation = dto.CurrentLocation
        });

        return await _departFromWaypointAction.ExecuteAsync(new DepartFromWaypointDto
        {
            TripId = dto.TripId,
            WaypointId = dto.WaypointId,
            Note = dto.Note,
            DeliveryNoteImage = dto.DeliveryNoteImage,
            Odometer = dto.Odometer,
            OdometerImage = dto.OdometerImage,
            CurrentLocation = dto.CurrentLocation
        });
    }
}
