using System.Text.Json;
using LoadlyAggregator.Domain.Admin.DTOs.TripsManagement;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Entities.EventPayloads;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.TripsManagement;

public class ArriveAtWaypointAction
{
    private readonly ITripRepository _tripRepository;
    private readonly ITripWaypointRepository _tripWaypointRepository;
    private readonly IRecordingRepository _recordingRepository;
    private readonly ICurrentUserService _currentUserService;

    public ArriveAtWaypointAction(
        ITripRepository tripRepository,
        ITripWaypointRepository tripWaypointRepository,
        IRecordingRepository recordingRepository,
        ICurrentUserService currentUserService)
    {
        _tripRepository = tripRepository;
        _tripWaypointRepository = tripWaypointRepository;
        _recordingRepository = recordingRepository;
        _currentUserService = currentUserService;
    }

    public async Task ExecuteAsync(ArriveAtWaypointDto dto)
    {
        var trip = await _tripRepository.GetByIdOrThrowAsync(dto.TripId);
        _ = trip.DriverId ?? throw new ValidationException(TripMessageKeys.VehicleMustHaveAssignedDriver);

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
        {
            return;
        }

        TripValidator.ValidateArriveAtWaypointEligibility(trip, waypoint, allWaypoints);

        var recording = new Recording
        {
            RecordableType = RecordableType.TRIP_WAYPOINT,
            RecordableId = dto.TripId,
            RecordableEventType = RecordableEventType.WAYPOINT_ARRIVED,
            AddedBy = _currentUserService.GetCurrentUserId(),
            AddedByModelType = "Admin",
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
    }
}
