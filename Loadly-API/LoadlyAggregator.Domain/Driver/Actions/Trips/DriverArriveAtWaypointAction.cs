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

public class DriverArriveAtWaypointAction
{
    private readonly ITripRepository _tripRepository;
    private readonly ITripWaypointRepository _tripWaypointRepository;
    private readonly IRecordingRepository _recordingRepository;
    private readonly INotificationEventDispatcher _notificationDispatcher;

    public DriverArriveAtWaypointAction(
        ITripRepository tripRepository,
        ITripWaypointRepository tripWaypointRepository,
        IRecordingRepository recordingRepository,
        INotificationEventDispatcher notificationDispatcher)
    {
        _tripRepository = tripRepository;
        _tripWaypointRepository = tripWaypointRepository;
        _recordingRepository = recordingRepository;
        _notificationDispatcher = notificationDispatcher;
    }

    public async Task ExecuteAsync(DriverArriveAtWaypointDto dto)
    {
        var trip = await _tripRepository.GetByIdOrThrowAsync(dto.TripId);

        TripValidator.ValidateDriverOwnsTrip(trip, dto.DriverId);

        var waypoint = await _tripWaypointRepository.GetByIdAsync(dto.WaypointId)
            ?? throw new NotFoundException("waypoints.not_found_with_id", dto.WaypointId);

        TripValidator.ValidateWaypointBelongsToTrip(waypoint, dto.TripId);

        var allWaypoints = (await _tripWaypointRepository.GetByTripIdAsync(dto.TripId))
            .OrderBy(w => w.Sequence)
            .ToList();

        if (waypoint.Status is TripWaypointStatus.Arrived
            or TripWaypointStatus.LoadingStarted
            or TripWaypointStatus.LoadingCompleted
            or TripWaypointStatus.UnloadingStarted
            or TripWaypointStatus.UnloadingCompleted
            or TripWaypointStatus.Departed
            or TripWaypointStatus.Completed)
            return;

        TripValidator.ValidateArriveAtWaypointEligibility(trip, waypoint, allWaypoints);

        var recording = new Recording
        {
            RecordableType = RecordableType.TRIP_WAYPOINT,
            RecordableId = dto.TripId,
            RecordableEventType = RecordableEventType.WAYPOINT_ARRIVED,
            AddedBy = dto.DriverId,
            AddedByModelType = "Driver",
            Data = JsonSerializer.SerializeToDocument(new DriverArriveAtWaypointEventData
            {
                Location = dto.CurrentLocation,
                Waypoint = waypoint.ToSnapshot()
            })
        };
        await _recordingRepository.AddAsync(recording);

        waypoint.Status = TripWaypointStatus.Arrived;
        _tripWaypointRepository.Update(waypoint);
        await _tripWaypointRepository.SaveChangesAsync();

        await _notificationDispatcher.DispatchAsync(new TripProgressNotificationData
        {
            TripId = trip.Id,
            NotificationEventType = NotificationEventType.TRIP_WAYPOINT_ARRIVED,
            WaypointId = waypoint.Id,
            WaypointName = waypoint.AddressName
        });
    }
}
