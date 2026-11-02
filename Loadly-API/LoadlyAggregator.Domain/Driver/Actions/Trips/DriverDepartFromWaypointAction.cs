using System.Text.Json;
using LoadlyAggregator.Domain.Admin.DTOs.Trips;
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

public class DriverDepartFromWaypointAction
{
    private readonly ITripRepository _tripRepository;
    private readonly ITripWaypointRepository _tripWaypointRepository;
    private readonly IRecordingRepository _recordingRepository;
    private readonly IMediaService _mediaService;
    private readonly INotificationEventDispatcher _notificationDispatcher;

    public DriverDepartFromWaypointAction(
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

    public async Task<TripWaypointDto?> ExecuteAsync(DriverDepartFromWaypointDto dto)
    {
        var trip = await _tripRepository.GetByIdOrThrowAsync(dto.TripId);

        TripValidator.ValidateDriverOwnsTrip(trip, dto.DriverId);

        var waypoint = await _tripWaypointRepository.GetByIdAsync(dto.WaypointId)
            ?? throw new NotFoundException("waypoints.not_found_with_id", dto.WaypointId);

        TripValidator.ValidateWaypointBelongsToTrip(waypoint, dto.TripId);

        var allWaypoints = (await _tripWaypointRepository.GetByTripIdAsync(dto.TripId))
            .OrderBy(w => w.Sequence)
            .ToList();

        if (waypoint.Status is TripWaypointStatus.Completed or TripWaypointStatus.Departed)
        {
            var nextAfterIdempotent = allWaypoints
                .Where(w => w.Sequence > waypoint.Sequence)
                .OrderBy(w => w.Sequence)
                .FirstOrDefault();
            return nextAfterIdempotent != null ? MapToDto(nextAfterIdempotent) : null;
        }

        TripValidator.ValidateDepartFromWaypointEligibility(trip, waypoint);

        bool isFirstWaypoint = TripValidator.IsFirstWaypoint(waypoint, allWaypoints);

        if (isFirstWaypoint)
            TripValidator.ValidateOdometerRequiredForFirstWaypointDepart(
                waypoint,
                allWaypoints,
                dto.Odometer,
                dto.OdometerImage);

        string? deliveryNoteImagePath = null;
        if (dto.DeliveryNoteImage != null && dto.DeliveryNoteImage.Length > 0)
        {
            deliveryNoteImagePath = await _mediaService.UploadAndGetPathAsync(
                dto.DeliveryNoteImage!,
                TripStoragePathHelper.DeliveryNote(dto.TripId, waypoint.Id));
        }

        string? odometerImagePath = null;
        if (isFirstWaypoint && dto.OdometerImage != null && dto.OdometerImage.Length > 0)
        {
            odometerImagePath = await _mediaService.UploadAndGetPathAsync(
                dto.OdometerImage,
                TripStoragePathHelper.OdometerStart(dto.TripId));
        }

        int? persistedOdometer = isFirstWaypoint ? dto.Odometer : null;
        string? persistedOdometerImagePath = isFirstWaypoint ? odometerImagePath : null;

        var recording = new Recording
        {
            RecordableType = RecordableType.TRIP_WAYPOINT,
            RecordableId = dto.TripId,
            RecordableEventType = RecordableEventType.WAYPOINT_DEPARTED,
            AddedBy = dto.DriverId,
            AddedByModelType = "Driver",
            Data = JsonSerializer.SerializeToDocument(new DriverDepartFromWaypointEventData
            {
                Note = dto.Note,
                DeliveryNoteImagePath = deliveryNoteImagePath,
                Odometer = persistedOdometer,
                OdometerImagePath = persistedOdometerImagePath,
                Location = dto.CurrentLocation,
                Waypoint = waypoint.ToSnapshot()
            })
        };
        await _recordingRepository.AddAsync(recording);

        waypoint.Status = TripWaypointStatus.Completed;
        _tripWaypointRepository.Update(waypoint);

        await _tripWaypointRepository.SaveChangesAsync();

        await _notificationDispatcher.DispatchAsync(new TripProgressNotificationData
        {
            TripId = trip.Id,
            NotificationEventType = NotificationEventType.TRIP_WAYPOINT_DEPARTED,
            WaypointId = waypoint.Id,
            WaypointName = waypoint.AddressName
        });

        var nextWaypoint = allWaypoints
            .Where(w => w.Sequence > waypoint.Sequence)
            .OrderBy(w => w.Sequence)
            .FirstOrDefault();

        return nextWaypoint != null ? MapToDto(nextWaypoint) : null;
    }

    private static TripWaypointDto MapToDto(TripWaypoint w) => new()
    {
        Id = w.Id,
        Sequence = w.Sequence,
        StopType = w.StopType,
        Latitude = w.Latitude,
        Longitude = w.Longitude,
        AddressName = w.AddressName,
        GooglePlaceId = w.GooglePlaceId,
        TaskDetails = w.TaskDetails,
        Status = w.Status
    };
}
