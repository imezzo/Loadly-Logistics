using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Domain.Admin.Helpers;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Notifications;
using LoadlyAggregator.Domain.Common.Notifications.Channels;
using LoadlyAggregator.Domain.Common.Notifications.EventData;
using LoadlyAggregator.Domain.Common.Services;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Trips;

public class CreateTripAction
{
    private readonly ITripRepository _tripRepository;
    private readonly ITripWaypointRepository _tripWaypointRepository;
    private readonly ITripContractDestinationRepository _tripContractDestinationRepository;
    private readonly IContractRepository _contractRepository;
    private readonly IContractDestinationRepository _contractDestinationRepository;
    private readonly IContractVehicleTypeRepository _contractVehicleTypeRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IDriverRepository _driverRepository;
    private readonly IContractorRepository _contractorRepository;
    private readonly ILeadRepository _leadRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationEventDispatcher _notificationDispatcher;
    private readonly PushTripToContractorHelper _pushTripToContractorHelper;
    private readonly TripVehicleAssignmentSnapshotRecorder _TripVehicleAssignmentSnapshotRecorder;

    public CreateTripAction(
        ITripRepository tripRepository,
        ITripWaypointRepository tripWaypointRepository,
        ITripContractDestinationRepository tripContractDestinationRepository,
        IContractRepository contractRepository,
        IContractDestinationRepository contractDestinationRepository,
        IContractVehicleTypeRepository contractVehicleTypeRepository,
        IVehicleRepository vehicleRepository,
        IDriverRepository driverRepository,
        IContractorRepository contractorRepository,
        ILeadRepository leadRepository,
        ICurrentUserService currentUserService,
        INotificationEventDispatcher notificationDispatcher,
        PushTripToContractorHelper pushTripToContractorHelper,
        TripVehicleAssignmentSnapshotRecorder TripVehicleAssignmentSnapshotRecorder)
    {
        _tripRepository = tripRepository;
        _tripWaypointRepository = tripWaypointRepository;
        _tripContractDestinationRepository = tripContractDestinationRepository;
        _contractRepository = contractRepository;
        _contractDestinationRepository = contractDestinationRepository;
        _contractVehicleTypeRepository = contractVehicleTypeRepository;
        _vehicleRepository = vehicleRepository;
        _driverRepository = driverRepository;
        _contractorRepository = contractorRepository;
        _leadRepository = leadRepository;
        _currentUserService = currentUserService;
        _notificationDispatcher = notificationDispatcher;
        _pushTripToContractorHelper = pushTripToContractorHelper;
        _TripVehicleAssignmentSnapshotRecorder = TripVehicleAssignmentSnapshotRecorder;
    }

    public async Task ExecuteAsync(CreateTripDto dto)
    {
        TripValidator.ValidateDateRange(dto.StartDate, dto.EndDate);
        TripValidator.ValidateTripStartDateNotInPast(dto.StartDate); // Disabled: allow creating trips with past start dates
        TripValidator.ValidateContractRequired(dto.IsExternalTrip, dto.ContractId);
        TripValidator.ValidateLeadForExternalTrip(
            dto.IsExternalTrip,
            dto.LeadId,
            dto.ContactPersonName,
            dto.ContactPersonMobile);

        TripValidator.ValidateContractorAssign(dto.DispatchingMode, dto.ContractorId);

        if (dto.ContractorId.HasValue)
        {
            var contractor = await _contractorRepository.GetByIdOrThrowAsync(dto.ContractorId.Value);
            if (contractor.ContractorStatus != ContractorStatus.ACTIVE)
                throw new ValidationException("contractors.not_active");
        }

        long? tripLeadId = null;
        string? contactPersonName = dto.ContactPersonName;
        string? contactPersonMobile = dto.ContactPersonMobile;
        if (dto.IsExternalTrip)
        {
            if (dto.LeadId is > 0)
            {
                var lead = await _leadRepository.GetByIdOrThrowAsync(dto.LeadId.Value);
                tripLeadId = lead.Id;
                contactPersonName = lead.FullName;
                contactPersonMobile = lead.Phone;
            }
            else
            {
                tripLeadId = await _leadRepository.UpsertByPhoneAsync(
                    dto.ContactPersonName!.Trim(),
                    dto.ContactPersonMobile!.Trim(),
                    email: null,
                    addedBy: _currentUserService.GetCurrentUserId());
            }
        }

        Contract? contract = null;
        if (dto.ContractId.HasValue)
        {
            contract = await _contractRepository.GetByIdAsync(dto.ContractId.Value)
                ?? throw new NotFoundException("contracts.not_found_with_id", dto.ContractId.Value);
            TripValidator.ValidateContractIsActive(contract);
            TripValidator.ValidateTripDatesWithinContractRange(contract, dto.StartDate, dto.EndDate);
            TripValidator.ValidateActualCargoWeightTons(contract, dto.ActualCargoWeightTons);
            TripValidator.ValidateCargoPrice(contract, dto.CargoPrice);
            TripValidator.ValidateSpotPrice(contract, dto.SpotPrice);
            TripValidator.ValidateContractDistanceKm(contract, dto.ContractDistanceKm);
            TripValidator.ValidateDestinationRequired(contract, dto.DestinationId);
        }

        TripValidator.ValidateFinancialDate(contract, dto.FinancialDate);

        TripValidator.ValidateContractorsDispatchAllowed(contract, dto.DispatchingMode, dto.ContractorId, dto.PushToAllContractors, isExternalTrip: dto.IsExternalTrip);

        Vehicle? headVehicle = null;
        Common.Entities.Driver? driver = null;

        if (dto.VehicleId.HasValue && dto.VehicleId.Value > 0)
        {
            headVehicle = await _vehicleRepository.GetByIdAsync(dto.VehicleId.Value) ?? throw new NotFoundException("trips.head_vehicle_not_found_with_id", dto.VehicleId.Value);
            TripValidator.ValidateHeadVehicle(headVehicle);
            TripValidator.ValidateNoOverlappingHeadVehicleTrips(_tripRepository, dto.VehicleId.Value, dto.StartDate, dto.EndDate);
            TripValidator.ValidateVehicleLinkedToContract(contract, headVehicle, dto.StartDate, headVehicle.ContractorId);
        }

        long? tailVehicleId = null;
        if (headVehicle != null)
        {
            driver = await _driverRepository.GetByIdAsync(headVehicle.DriverId.Value) ?? throw new NotFoundException("drivers.not_found_with_id", headVehicle.DriverId.Value);
            TripValidator.ValidateIsActiveDriver(driver);
            TripValidator.ValidateNoOverlappingDriverTrips(_tripRepository, driver.Id, dto.StartDate, dto.EndDate);

            tailVehicleId = headVehicle.TailVehicles.FirstOrDefault()?.Id;
        }

        ContractVehicleType? contractVehicleType = null;
        if (dto.VehicleTypeId is > 0 && dto.ContractId.HasValue)
        {
            contractVehicleType = await _contractVehicleTypeRepository.GetByContractIdAndVehicleTypeIdAsync(dto.ContractId.Value, dto.VehicleTypeId.Value)
                ?? throw new NotFoundException("trips.contract_vehicle_type_not_found_for_vehicle_type", dto.VehicleTypeId.Value);
        }

        TripValidator.ValidateTailAttachedWhenContractVehicleTypeIsTail(contractVehicleType, headVehicle);

        ContractDestination? tripDestination = dto.ContractId.HasValue
            ? await TripContractDestinationHelper.GetOptionalContractDestinationForTripAsync(
                dto.ContractId.Value,
                dto.DestinationId,
                _contractDestinationRepository)
            : null;

        ContractDestination? returnTripDestination = dto.ContractId.HasValue
            ? await TripContractDestinationHelper.GetOptionalContractDestinationForTripAsync(
                dto.ContractId.Value,
                dto.ReturnDestinationId,
                _contractDestinationRepository)
            : null;

        long? dispatcherId = _currentUserService.GetCurrentUserId();

        TripValidator.ValidateTripTypeWithContractDestination(
            dto.Type,
            tripDestination,
            dto.DestinationId,
            returnTripDestination,
            dto.ReturnDestinationId);
        TripValidator.ValidateWaypoints(dto.Waypoints, dto.Type);
        var contractDistanceKm = TripContractDistanceHelper.ResolveContractDistanceKm(
            dto.Type,
            tripDestination,
            dto.ContractDistanceKm,
            returnTripDestination);

        var numberOfDays = TripContractDestinationHelper.ResolveNumberOfDays(
            dto.StartDate,
            dto.EndDate,
            tripDestination?.NumberOfDays);
        var financialDate = TripFinancialDateResolver.Resolve(contract, dto.StartDate, dto.FinancialDate);

        var contractSlotId = TripContractSlotHelper.ResolveContractSlotIdForTrip(
            contract,
            dto.VehicleId,
            financialDate,
            numberOfDays,
            dto.StartDate);

        if (contract != null)
        {
            TripValidator.ValidateSubmittedContractorPrice(
                contract,
                TripStatus.Pending,
                dto.ContractorPrice,
                dto.ActualCargoWeightTons,
                contractVehicleType,
                tripDestination,
                contractSlotId,
                dto.DispatchingMode);
        }
        else
        {
            TripValidator.ValidateSubmittedExternalTripContractorPrice(
                dto.IsExternalTrip,
                TripStatus.Pending,
                dto.ContractorPrice,
                dto.DispatchingMode);
        }

        var canBeScheduled = TripValidator.HasRequiredAssignmentAttributes(headVehicle?.DriverId, dto.VehicleId.GetValueOrDefault(), dto.StartDate, dto.EndDate);
        // && TripValidator.AllWaypointsHaveCoordinates(dto.Waypoints);

        var trip = new Trip
        {
            ContractId = dto.ContractId,
            NumberOfDays = numberOfDays,
            ContractVehicleTypeId = contractVehicleType?.Id,
            ContractSlotId = contractSlotId,
            VehicleId = dto.VehicleId,
            ContractorId = dto.ContractorId,
            TailVehicleId = tailVehicleId,
            DriverId = headVehicle?.DriverId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            FinancialDate = financialDate,
            Status = canBeScheduled ? TripStatus.Scheduled : TripStatus.Pending,
            TripDocumentStatus = TripDocumentStatus.Pending,
            CustomsStatus = dto.HasCustoms ? TripCustomsStatus.PendingEntrance : TripCustomsStatus.None,
            DispatcherId = dispatcherId,
            ContractDistanceKm = contractDistanceKm,
            ActualCargoWeightTons = dto.ActualCargoWeightTons,
            CargoPrice = dto.CargoPrice,
            SpotPrice = dto.SpotPrice,
            ContractorPrice = dto.ContractorPrice,
            IsExternalTrip = dto.IsExternalTrip,
            IsBackfill = false,
            LeadId = tripLeadId,
            ContactPersonName = contactPersonName,
            ContactPersonMobile = contactPersonMobile,
            DispatcherNotes = string.IsNullOrWhiteSpace(dto.DispatcherNotes) ? null : dto.DispatcherNotes.Trim(),
            HelpersCount = dto.HelpersCount,
            DispatchingMode = dto.DispatchingMode,
            Type = dto.Type
        };

        if (dto.PushToAllContractors)
            TripValidator.ValidatePushToAllContractors(trip);

        await _tripRepository.AddAsync(trip);
        await _tripRepository.SaveChangesAsync();

        await _TripVehicleAssignmentSnapshotRecorder.SyncForTripAsync(
            trip.Id,
            headVehicle,
            tailVehicleId,
            tailVehicle: null,
            freezeSnapshot: false,
            addedBy: _currentUserService.GetCurrentUserId());
        await _tripRepository.SaveChangesAsync();

        if (tripDestination != null)
        {
            var assignment = new TripContractDestination
            {
                TripId = trip.Id,
                AddedBy = _currentUserService.GetCurrentUserId() ?? 0,
            };
            TripContractDestinationHelper.ApplyContractDestinationAssignment(
                assignment,
                dto.Type,
                tripDestination,
                returnTripDestination);
            await _tripContractDestinationRepository.AddAsync(assignment);
            await _tripContractDestinationRepository.SaveChangesAsync();
        }

        if (dto.Waypoints != null && dto.Waypoints.Count > 0)
        {
            var waypointEntities = dto.Waypoints.Select(w => new TripWaypoint
            {
                TripId = trip.Id,
                Sequence = w.Sequence,
                StopType = w.StopType,
                Latitude = w.Latitude,
                Longitude = w.Longitude,
                AddressName = w.AddressName ?? string.Empty,
                GooglePlaceId = w.GooglePlaceId,
                TaskDetails = w.TaskDetails
            }).ToList();
            await _tripWaypointRepository.AddRangeAsync(waypointEntities);
            await _tripWaypointRepository.SaveChangesAsync();
        }

        if (canBeScheduled && !trip.IsExternalTrip && headVehicle != null && headVehicle.DriverId.HasValue)
        {
            await _notificationDispatcher.DispatchAsync(new TripScheduledNotificationData
            {
                TripId = trip.Id,
                CompanyId = contract.CompanyId,
                DriverId = headVehicle.DriverId.Value,
                DriverPushKind = TripScheduleDriverPushKind.NewAssignment
            });
        }

        if (dto.PushToAllContractors)
        {
            await _pushTripToContractorHelper.InviteContractorsAsync(
                trip,
                sendToAllContractors: true,
                contractorIds: null);
        }
    }
}
