using System.Text.Json;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Entities.EventPayloads;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Notifications;
using LoadlyAggregator.Domain.Common.Notifications.EventData;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Domain.Driver.DTOs.Trips;
using LoadlyAggregator.Shared.DTOs.Common;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Driver.Actions.Trips;

public class DriverCompleteLoadingCargoAction
{
    private readonly ITripRepository _tripRepository;
    private readonly ITripWaypointRepository _tripWaypointRepository;
    private readonly IRecordingRepository _recordingRepository;
    private readonly IMediaService _mediaService;
    private readonly INotificationEventDispatcher _notificationDispatcher;

    public DriverCompleteLoadingCargoAction(
        ITripRepository tripRepository,
        ITripWaypointRepository tripWaypointRepository,
        IRecordingRepository recordingRepository,
        IMediaService mediaService,
        INotificationEventDispatcher notificationDispatcher)
    {
        _tripRepository = tripRepository;
        _tripWaypointRepository = tripWaypointRepository;
        _recordingRepository = recordingRepository;
        _mediaService = mediaService;
        _notificationDispatcher = notificationDispatcher;
    }

    public async Task ExecuteAsync(DriverCompleteLoadingCargoDto dto)
    {
        var trip = await _tripRepository.GetByIdOrThrowAsync(dto.TripId);

        TripValidator.ValidateDriverOwnsTrip(trip, dto.DriverId);

        var waypoint = await _tripWaypointRepository.GetByIdAsync(dto.WaypointId)
            ?? throw new NotFoundException("waypoints.not_found_with_id", dto.WaypointId);

        TripValidator.ValidateWaypointBelongsToTrip(waypoint, dto.TripId);

        var allWaypoints = (await _tripWaypointRepository.GetByTripIdAsync(dto.TripId))
            .OrderBy(w => w.Sequence)
            .ToList();

        if (waypoint.Status is TripWaypointStatus.LoadingCompleted
            or TripWaypointStatus.Departed
            or TripWaypointStatus.Completed)
            return;

        TripValidator.ValidateCompleteLoadingCargoEligibility(trip, waypoint, allWaypoints);
        TripValidator.ValidateCompleteLoadingCargoImages(dto.ReceiptNoteImage, dto.ShipmentImages);

        var receiptNoteImagePath = await _mediaService.UploadAndGetPathAsync(
            dto.ReceiptNoteImage!,
            TripStoragePathHelper.ReceiptNote(dto.TripId, waypoint.Id));

        var shipmentImagePaths = await _mediaService.UploadImagesAsync(
            dto.ShipmentImages!,
            TripStoragePathHelper.ShipmentImages(dto.TripId, waypoint.Id));

        var recording = new Recording
        {
            RecordableType = RecordableType.TRIP_WAYPOINT,
            RecordableId = dto.TripId,
            RecordableEventType = RecordableEventType.WAYPOINT_LOADING_COMPLETED,
            AddedBy = dto.DriverId,
            AddedByModelType = "Driver",
            Data = JsonSerializer.SerializeToDocument(new DriverCompleteLoadingCargoEventData
            {
                Note = dto.Note,
                ReceiptNoteImagePath = receiptNoteImagePath,
                ShipmentImagePaths = shipmentImagePaths,
                Location = dto.CurrentLocation,
                Waypoint = waypoint.ToSnapshot()
            })
        };
        await _recordingRepository.AddAsync(recording);

        waypoint.Status = TripWaypointStatus.LoadingCompleted;
        _tripWaypointRepository.Update(waypoint);
        await _tripWaypointRepository.SaveChangesAsync();

        await _notificationDispatcher.DispatchAsync(new TripProgressNotificationData
        {
            TripId = trip.Id,
            NotificationEventType = NotificationEventType.TRIP_WAYPOINT_LOADING_COMPLETED,
            WaypointId = waypoint.Id,
            WaypointName = waypoint.AddressName
        });
    }
}
