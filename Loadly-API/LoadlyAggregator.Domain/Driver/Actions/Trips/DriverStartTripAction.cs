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
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Driver.Actions.Trips;

public class DriverStartTripAction
{
    private readonly ITripRepository _tripRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly ITripWaypointRepository _tripWaypointRepository;
    private readonly IRecordingRepository _recordingRepository;
    private readonly IDriverRepository _driverRepository;
    private readonly IVehicleFacilityVisitRepository _visitRepository;
    private readonly INotificationEventDispatcher _notificationDispatcher;
    private readonly IContractRepository _contractRepository;

    public DriverStartTripAction(
        ITripRepository tripRepository,
        IVehicleRepository vehicleRepository,
        ITripWaypointRepository tripWaypointRepository,
        IRecordingRepository recordingRepository,
        IDriverRepository driverRepository,
        IVehicleFacilityVisitRepository visitRepository,
        INotificationEventDispatcher notificationDispatcher,
        IContractRepository contractRepository)
    {
        _tripRepository = tripRepository;
        _vehicleRepository = vehicleRepository;
        _tripWaypointRepository = tripWaypointRepository;
        _recordingRepository = recordingRepository;
        _driverRepository = driverRepository;
        _visitRepository = visitRepository;
        _notificationDispatcher = notificationDispatcher;
        _contractRepository = contractRepository;
    }

    public async Task<TripWaypointDto> ExecuteAsync(DriverStartTripDto dto)
    {
        var trip = await _tripRepository.GetByIdOrThrowAsync(dto.TripId);

        TripValidator.ValidateDriverOwnsTrip(trip, dto.DriverId);

        var allWaypoints = (await _tripWaypointRepository.GetByTripIdAsync(dto.TripId))
            .OrderBy(w => w.Sequence)
            .ToList();

        // (Idempotency by status check) If the trip is already in progress, we don't need to start it again.
        if (trip.Status is TripStatus.InProgress or TripStatus.Completed)
        {   
            if (allWaypoints.Count == 0)
                throw new ValidationException(TripMessageKeys.NoWaypoints);
            return MapToDto(allWaypoints.First());
        }

        TripValidator.ValidateStartTripEligibility(trip, allWaypoints);
        await TripValidator.ValidateDriverHasNoInProgressTripAsync(_tripRepository, dto.DriverId);
        // TripValidator.ValidateTripStartDateNotInPast(trip.StartDate);

        TripValidator.ValidateTripTailMatchesHeadAttachedTail(trip.Vehicle, trip.TailVehicleId);

        if (trip.TailVehicleId.HasValue)
        {
            var tailVehicle = await _vehicleRepository.GetByIdAsync(trip.TailVehicleId.Value);
            TripValidator.ValidateTailVehicleAvailable(tailVehicle);
        }

        if (trip.Contract != null && !trip.Contract.AutoAssignMode)
        {
            var contract = await _contractRepository.GetByIdAsync(trip.Contract.Id)
                ?? throw new NotFoundException("contracts.not_found_with_id", trip.Contract.Id);
            TripContractSlotHelper.ResolveContractSlotIdForTrip(
                contract,
                trip.VehicleId,
                trip.FinancialDate,
                trip.NumberOfDays,
                trip.StartDate,
                trip.ContractSlotId);
        }

        var firstWaypoint = allWaypoints.First();
        var recording = new Recording
        {
            RecordableType = RecordableType.TRIP,
            RecordableId = dto.TripId,
            RecordableEventType = RecordableEventType.TRIP_STARTED,
            AddedBy = dto.DriverId,
            AddedByModelType = "Driver",
            Data = JsonSerializer.SerializeToDocument(new DriverStartTripEventData
            {
                Note = dto.Note,
                Location = dto.CurrentLocation,
                Waypoint = firstWaypoint.ToSnapshot()
            })
        };
        await _recordingRepository.AddAsync(recording);

        trip.Status = TripStatus.InProgress;
        _tripRepository.Update(trip);

        if (trip.Vehicle is not null)
        {
            await VehicleParkingFacilityHelper.RecordExitAndClearIfAssignedAsync(
                trip.Vehicle,
                _visitRepository,
                dto.DriverId,
                addedBy: null,
                trip.ContractSlotId);
        }

        await _vehicleRepository.UpdateStatusAsync(trip.VehicleId!.Value, VehicleStatus.InRoute);

        if (trip.TailVehicleId.HasValue)
        {
            await _vehicleRepository.UpdateStatusAsync(trip.TailVehicleId.Value, VehicleStatus.InRoute);
        }

        await _driverRepository.UpdateWorkingStatusAsync(dto.DriverId, DriverWorkingStatus.Busy);

        await _tripRepository.SaveChangesAsync();

        await _notificationDispatcher.DispatchAsync(new TripProgressNotificationData
        {
            TripId = trip.Id,
            NotificationEventType = NotificationEventType.TRIP_STARTED
        });

        return MapToDto(firstWaypoint);
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
