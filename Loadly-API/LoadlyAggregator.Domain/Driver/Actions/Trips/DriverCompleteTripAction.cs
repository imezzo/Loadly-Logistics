using System.Text.Json;
using LoadlyAggregator.Domain.Common.Actions;
using LoadlyAggregator.Domain.Common.DTOs;
using LoadlyAggregator.Domain.Common.Notifications;
using LoadlyAggregator.Domain.Common.Notifications.EventData;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Entities.EventPayloads;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Domain.Driver.DTOs.Trips;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.DTOs.Common;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Driver.Actions.Trips;

public class DriverCompleteTripAction
{
    private readonly ITripRepository _tripRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly ITripWaypointRepository _tripWaypointRepository;
    private readonly IRecordingRepository _recordingRepository;
    private readonly IMediaService _mediaService;
    private readonly IDriverRepository _driverRepository;
    private readonly CalculateAndPersistTripChargeAction _calculateAndPersistTripChargeAction;
    private readonly INotificationEventDispatcher _notificationDispatcher;

    public DriverCompleteTripAction(
        ITripRepository tripRepository,
        IVehicleRepository vehicleRepository,
        ITripWaypointRepository tripWaypointRepository,
        IRecordingRepository recordingRepository,
        IMediaService mediaService,
        IDriverRepository driverRepository,
        CalculateAndPersistTripChargeAction calculateAndPersistTripChargeAction,
        INotificationEventDispatcher notificationDispatcher)
    {
        _tripRepository = tripRepository;
        _vehicleRepository = vehicleRepository;
        _tripWaypointRepository = tripWaypointRepository;
        _recordingRepository = recordingRepository;
        _mediaService = mediaService;
        _driverRepository = driverRepository;
        _calculateAndPersistTripChargeAction = calculateAndPersistTripChargeAction;
        _notificationDispatcher = notificationDispatcher;
    }

    public async Task ExecuteAsync(DriverCompleteTripDto dto)
    {
        var trip = await _tripRepository.GetByIdOrThrowAsync(dto.TripId);

        TripValidator.ValidateDriverOwnsTrip(trip, dto.DriverId);

        var allWaypoints = (await _tripWaypointRepository.GetByTripIdAsync(dto.TripId))
            .OrderBy(w => w.Sequence)
            .ToList();

        // (Idempotency by status check) If the trip is already completed, we don't need to complete it again.
        if (trip.Status is TripStatus.Completed)
            return;

        TripValidator.ValidateCompleteTripEligibility(trip, allWaypoints);

        var lastWaypoint = allWaypoints.Last();

        if (lastWaypoint.Status != TripWaypointStatus.Completed)
        {
            // For Dropoff, status is UnloadingCompleted (validated) - delivery note was already uploaded during complete unloading
            var waypointDepartRecording = new Recording
            {
                RecordableType = RecordableType.TRIP_WAYPOINT,
                RecordableId = dto.TripId,
                RecordableEventType = RecordableEventType.WAYPOINT_DEPARTED,
                AddedBy = dto.DriverId,
                AddedByModelType = "Driver",
                Data = JsonSerializer.SerializeToDocument(new DriverDepartFromWaypointEventData
                {
                    DeliveryNoteImagePath = null,
                    Location = dto.CurrentLocation,
                    Waypoint = lastWaypoint.ToSnapshot()
                })
            };
            await _recordingRepository.AddAsync(waypointDepartRecording);

            lastWaypoint.Status = TripWaypointStatus.Completed;
            _tripWaypointRepository.Update(lastWaypoint);
            await _tripWaypointRepository.SaveChangesAsync();
        }
        var odometerImagePath = await _mediaService.UploadAndGetPathAsync(
            dto.OdometerImage!,
            TripStoragePathHelper.OdometerEnd(dto.TripId));

        var tollgateImagePaths = await _mediaService.UploadImagesAsync(
            dto.TollgateImages ?? [],
            TripStoragePathHelper.Tollgates(dto.TripId));

        var tollgateWeightImagePaths = await _mediaService.UploadImagesAsync(
            dto.TollgateWeightImages ?? [],
            TripStoragePathHelper.TollgateWeights(dto.TripId));

        string? deliveryNoteImagePath = null;
        if (dto.DeliveryNoteImage != null && dto.DeliveryNoteImage.Length > 0)
        {
            deliveryNoteImagePath = await _mediaService.UploadAndGetPathAsync(
                dto.DeliveryNoteImage,
                TripStoragePathHelper.DeliveryNote(dto.TripId, lastWaypoint.Id));
        }

        var recordableTypes = new[] { RecordableType.TRIP, RecordableType.TRIP_WAYPOINT };
        var tripRecordings = (await _recordingRepository.GetTimelineByRecordableAsync(
            dto.TripId,
            recordableTypes)).ToList();
        TripValidator.ValidateFinalOdometerAgainstInitial(dto.Odometer, tripRecordings);

        trip.Status = TripStatus.Completed;
        TripContractDestinationHelper.ApplyNumberOfDays(
            trip,
            TripContractDestinationHelper.GetCurrentContractDestination(trip));
        var actualDistance = TripOdometerHelper.CalculateActualDistanceKm(tripRecordings, dto.Odometer);
        if (actualDistance.HasValue)
            trip.ActualDistanceKm = actualDistance.Value;

        _tripRepository.Update(trip);

       


        var tripCompletedRecording = new Recording
        {
            RecordableType = RecordableType.TRIP,
            RecordableId = dto.TripId,
            RecordableEventType = RecordableEventType.TRIP_COMPLETED,
            AddedBy = dto.DriverId,
            AddedByModelType = "Driver",
            Data = JsonSerializer.SerializeToDocument(new DriverCompleteTripEventData
            {
                Odometer = dto.Odometer,
                TollgatesTotalAmount = dto.TollgatesTotalAmount,
                TollgatesWeightTotalAmount = dto.TollgatesWeightTotalAmount,
                OdometerImagePath = odometerImagePath,
                Note = dto.Note,
                TollgateImagePaths = tollgateImagePaths,
                TollgateWeightImagePaths = tollgateWeightImagePaths,
                Location = dto.CurrentLocation,
                Waypoint = lastWaypoint.ToSnapshot()
            })
        };
        await _recordingRepository.AddAsync(tripCompletedRecording);

        await _driverRepository.UpdateWorkingStatusAsync(dto.DriverId, DriverWorkingStatus.Available);
        await _vehicleRepository.UpdateStatusAsync(trip.VehicleId.Value, VehicleStatus.Available);
        if (trip.TailVehicleId.HasValue)
        {
            await _vehicleRepository.UpdateStatusAsync(trip.TailVehicleId.Value, VehicleStatus.Available);
        }
        // Persist completion state first so charge calculation can run on Completed trips.
        await _tripRepository.SaveChangesAsync();

        await _calculateAndPersistTripChargeAction.ExecuteAsync(new CalculateAndPersistTripChargeDto
        {
            TripId = dto.TripId
        });

        await _tripRepository.SaveChangesAsync();

        await _notificationDispatcher.DispatchAsync(new TripCompletedByDriverNotificationData
        {
            TripId = trip.Id
        });
    }
}
