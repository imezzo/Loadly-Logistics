using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Domain.Admin.Helpers;
using LoadlyAggregator.Domain.Common.Actions;
using LoadlyAggregator.Domain.Common.DTOs;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Notifications;
using LoadlyAggregator.Domain.Common.Notifications.Channels;
using LoadlyAggregator.Domain.Common.Notifications.EventData;
using LoadlyAggregator.Domain.Common.Services;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Trips;

public class UpdateTripAction
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
    private readonly IAdminRepository _adminRepository;
    private readonly IPermissionService _permissionService;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationEventDispatcher _notificationDispatcher;
    private readonly CalculateAndPersistTripChargeAction _calculateAndPersistTripChargeAction;
    private readonly PushTripToContractorHelper _pushTripToContractorHelper;
    private readonly ITripBookingRequestRepository _tripBookingRequestRepository;
    private readonly ITripBookingInvitationRepository _tripBookingInvitationRepository;
    private readonly ITripBookingInvitationRoundRepository _invitationRoundRepository;
    private readonly TripVehicleAssignmentSnapshotRecorder _TripVehicleAssignmentSnapshotRecorder;


    public UpdateTripAction(
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
        IAdminRepository adminRepository,
        IPermissionService permissionService,
        ICurrentUserService currentUserService,
        INotificationEventDispatcher notificationDispatcher,
        CalculateAndPersistTripChargeAction calculateAndPersistTripChargeAction,
        PushTripToContractorHelper pushTripToContractorHelper,
        ITripBookingRequestRepository tripBookingRequestRepository,
        ITripBookingInvitationRepository tripBookingInvitationRepository,
        ITripBookingInvitationRoundRepository invitationRoundRepository,
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
        _adminRepository = adminRepository;
        _permissionService = permissionService;
        _currentUserService = currentUserService;
        _notificationDispatcher = notificationDispatcher;
        _calculateAndPersistTripChargeAction = calculateAndPersistTripChargeAction;
        _pushTripToContractorHelper = pushTripToContractorHelper;
        _tripBookingRequestRepository = tripBookingRequestRepository;
        _tripBookingInvitationRepository = tripBookingInvitationRepository;
        _invitationRoundRepository = invitationRoundRepository;
        _TripVehicleAssignmentSnapshotRecorder = TripVehicleAssignmentSnapshotRecorder;
    }

    public async Task ExecuteAsync(UpdateTripDto dto)
    {
        var trip = await _tripRepository.GetByIdAsync(dto.Id) ?? throw new NotFoundException("trips.not_found_with_id", dto.Id);
        if (trip.Status == TripStatus.CompanyDraft)
            throw new NotFoundException("trips.not_found_with_id", dto.Id);

        TripValidator.ValidateTripNotAuditedOrInvoiced(trip);

        var currentUserId = _currentUserService.GetCurrentUserId() ?? throw new UnauthorizedException("auth.user_not_authenticated");
        var admin = await _adminRepository.GetByIdOrThrowAsync(currentUserId);
        await TripValidator.ValidateUpdateTripPermissionByStatusAsync(admin, trip, _permissionService);

        TripValidator.ValidateVehicleRequiredWhenInProgress(trip.Status, dto.VehicleId);
        TripValidator.ValidateContractNotChangedWhenInProgressOrLater(trip.Status, trip.ContractId, dto.ContractId);

        var previousDispatchingMode = trip.DispatchingMode;
        var effectiveDispatchingMode = dto.DispatchingMode ?? trip.DispatchingMode;
        TripValidator.ValidateDispatchingChange(trip, dto.DispatchingMode);
        TripValidator.ValidateContractorAssign(effectiveDispatchingMode, dto.ContractorId, trip);

        if (dto.ContractorId.HasValue)
        {
            var contractor = await _contractorRepository.GetByIdOrThrowAsync(dto.ContractorId.Value);
            if (contractor.ContractorStatus != ContractorStatus.ACTIVE)
                throw new ValidationException("contractors.not_active");
        }

        // Re-Enabled: allow updating trips with past start dates
        if (trip.Status is not TripStatus.InProgress and not TripStatus.Completed)
            TripValidator.ValidateTripStartDateNotInPast(dto.StartDate);

        TripValidator.ValidateContractRequired(dto.IsExternalTrip, dto.ContractId);
        TripValidator.ValidateLeadForExternalTrip(
            dto.IsExternalTrip,
            dto.LeadId,
            dto.ContactPersonName,
            dto.ContactPersonMobile);

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
            TripValidator.ValidateActualCargoWeightTons(contract, dto.ActualCargoWeightTons);
            TripValidator.ValidateCargoPrice(contract, dto.CargoPrice);
            TripValidator.ValidateSpotPrice(contract, dto.SpotPrice);
            TripValidator.ValidateContractDistanceKm(contract, dto.ContractDistanceKm);
            TripValidator.ValidateDestinationRequired(contract, dto.DestinationId);
        }

        TripValidator.ValidateFinancialDate(contract, dto.FinancialDate);

        TripValidator.ValidateContractorsDispatchAllowed(contract, effectiveDispatchingMode, dto.ContractorId, dto.PushToAllContractors, isExternalTrip: dto.IsExternalTrip);

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

        TripValidator.ValidateTripTypeWithContractDestination(
            dto.Type,
            tripDestination,
            dto.DestinationId,
            returnTripDestination,
            dto.ReturnDestinationId);

        var existingTripDestinations = (await _tripContractDestinationRepository.GetByTripIdAsync(trip.Id)).ToList();
        long? currentDestinationId = TripContractDestinationHelper.GetCurrentDestinationId(existingTripDestinations);
        bool destinationUpdated = currentDestinationId != tripDestination?.Id;
        TripValidator.ValidateDateRange(dto.StartDate, dto.EndDate);
        TripValidator.ValidateDateLocks(trip, dto.StartDate, dto.EndDate, destinationUpdated);

        if (contract != null)
            TripValidator.ValidateTripDatesWithinContractRange(contract, dto.StartDate, dto.EndDate);

        Vehicle? headVehicle = null;
        Common.Entities.Driver? driver = null;
        // Completed: planned window may overlap later trips; don't re-block edits, keep historical assignment.
        bool isCompletedUpdate = trip.Status == TripStatus.Completed;

        if (dto.VehicleId.HasValue && dto.VehicleId.Value > 0)
        {
            headVehicle = await _vehicleRepository.GetByIdAsync(dto.VehicleId.Value) ?? throw new NotFoundException("trips.head_vehicle_not_found_with_id", dto.VehicleId.Value);
            if (!isCompletedUpdate)
            {
                TripValidator.ValidateHeadVehicle(headVehicle);
                TripValidator.ValidateNoOverlappingHeadVehicleTrips(_tripRepository, dto.VehicleId.Value, dto.StartDate, dto.EndDate, excludeTripId: trip.Id);
                TripValidator.ValidateVehicleLinkedToContract(
                    contract,
                    headVehicle,
                    dto.StartDate,
                    headVehicle.ContractorId,
                    trip.Status);
            }

        }

        long? tailVehicleId = null;
        bool canUpdateDriver = trip.Status < TripStatus.InProgress && !trip.ContractorId.HasValue;
        if (headVehicle != null)
        {
            if (canUpdateDriver)
            {
                long driverId = headVehicle.DriverId ?? throw new NotFoundException("drivers.not_found_with_id");
                driver = await _driverRepository.GetByIdAsync(driverId) ?? throw new NotFoundException("drivers.not_found_with_id", driverId);
                TripValidator.ValidateIsActiveDriver(driver);
                TripValidator.ValidateNoOverlappingDriverTrips(_tripRepository, driver.Id, dto.StartDate, dto.EndDate, excludeTripId: trip.Id);
            }

            tailVehicleId = headVehicle.TailVehicles.FirstOrDefault()?.Id;
        }

        bool driverChanged = trip.DriverId != driver?.Id;
        bool schedulingChanged = trip.VehicleId != dto.VehicleId
            || trip.StartDate != dto.StartDate
            || trip.EndDate != dto.EndDate;

        ContractVehicleType? contractVehicleType = null;
        if (dto.VehicleTypeId is > 0 && dto.ContractId.HasValue)
        {
            contractVehicleType = await _contractVehicleTypeRepository.GetByContractIdAndVehicleTypeIdAsync(dto.ContractId.Value, dto.VehicleTypeId.Value)
                ?? throw new NotFoundException("trips.contract_vehicle_type_not_found_for_vehicle_type", dto.VehicleTypeId.Value);
        }
        else if (contract != null && trip.ContractVehicleTypeId is long existingCvtId)
        {
            contractVehicleType = contract.ContractVehicleTypes.FirstOrDefault(cvt => cvt.Id == existingCvtId);
        }

        TripValidator.ValidateTailAttachedWhenContractVehicleTypeIsTail(contractVehicleType, headVehicle);

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
            dto.StartDate,
            trip.ContractSlotId);

        TripValidator.ValidateContractorPriceUpdateAllowed(trip, dto.ContractorPrice, effectiveDispatchingMode);
        var contractorPrice = TripValidator.ResolveContractorPriceForUpdate(trip, dto.ContractorPrice);
        if (contract != null)
        {
            TripValidator.ValidateSubmittedContractorPrice(
                contract,
                trip.Status,
                contractorPrice,
                dto.ActualCargoWeightTons,
                contractVehicleType,
                tripDestination,
                contractSlotId,
                effectiveDispatchingMode);
        }
        else
        {
            TripValidator.ValidateSubmittedExternalTripContractorPrice(
                dto.IsExternalTrip,
                trip.Status,
                contractorPrice,
                effectiveDispatchingMode);
        }

        bool shouldRecalculateCharge = TripUpdateHelper.IsCompletedTripChargeRecalculationRequired(
            trip,
            currentDestinationId,
            tripDestination?.Id,
            dto.ActualCargoWeightTons,
            dto.ContractDistanceKm);

        if (shouldRecalculateCharge)
        {
            TripValidator.ValidateTripChargesNotInvoiced(trip);
            TripValidator.ValidateTripChargesNotDocumentsApproved(trip);
        }

        trip.ContractId = dto.ContractId;
        trip.IsExternalTrip = dto.IsExternalTrip;
        trip.LeadId = tripLeadId;
        TripContractDestination? existingAssignment = existingTripDestinations
            .Where(tcd => tcd.DeletedAt == null)
            .OrderByDescending(tcd => tcd.Id)
            .FirstOrDefault()
            ?? existingTripDestinations.OrderByDescending(tcd => tcd.Id).FirstOrDefault();

        if (tripDestination == null)
        {
            if (destinationUpdated && existingAssignment != null)
                _tripContractDestinationRepository.Remove(existingAssignment);
        }
        else
        {
            if (existingAssignment != null)
            {
                TripContractDestinationHelper.ApplyContractDestinationAssignment(
                    existingAssignment,
                    dto.Type,
                    tripDestination,
                    returnTripDestination);
                existingAssignment.DeletedAt = null;
                _tripContractDestinationRepository.Update(existingAssignment);
            }
            else
            {
                var assignment = new TripContractDestination
                {
                    TripId = trip.Id,
                    AddedBy = currentUserId,
                };
                TripContractDestinationHelper.ApplyContractDestinationAssignment(
                    assignment,
                    dto.Type,
                    tripDestination,
                    returnTripDestination);
                await _tripContractDestinationRepository.AddAsync(assignment);
            }
        }

        trip.ContractVehicleTypeId = contractVehicleType?.Id;
        // Completed: keep historical driver/tail instead of syncing from the current vehicle.
        trip.TailVehicleId = isCompletedUpdate ? trip.TailVehicleId : tailVehicleId;
        trip.DriverId = !canUpdateDriver ? trip.DriverId : driver?.Id;
        trip.ContractSlotId = contractSlotId;
        trip.VehicleId = dto.VehicleId;
        trip.StartDate = dto.StartDate;
        trip.EndDate = dto.EndDate;
        trip.FinancialDate = financialDate;
        trip.NumberOfDays = numberOfDays;
        trip.ContractDistanceKm = dto.ContractDistanceKm;
        trip.ActualCargoWeightTons = dto.ActualCargoWeightTons;
        trip.CargoPrice = dto.CargoPrice;
        trip.SpotPrice = dto.SpotPrice;
        trip.ContractorPrice = contractorPrice;
        trip.ContactPersonName = contactPersonName;
        trip.ContactPersonMobile = contactPersonMobile;
        trip.DispatcherNotes = string.IsNullOrWhiteSpace(dto.DispatcherNotes) ? null : dto.DispatcherNotes.Trim();
        trip.HelpersCount = dto.HelpersCount;
        trip.DispatcherId = trip.DispatcherId ?? _currentUserService.GetCurrentUserId();
        if (dto.DispatchingMode.HasValue)
            trip.DispatchingMode = dto.DispatchingMode;
        trip.Type = dto.Type;

        if (dto.Type is TripType.RoundTrip or TripType.OpenJaw && dto.Waypoints == null)
        {
            var existingForValidation = (await _tripWaypointRepository.GetByTripIdAsync(dto.Id))
                .OrderBy(w => w.Sequence)
                .Select(w => new TripWaypointDto
                {
                    Id = w.Id,
                    Sequence = w.Sequence,
                    StopType = w.StopType,
                    Latitude = w.Latitude,
                    Longitude = w.Longitude,
                    AddressName = w.AddressName,
                    GooglePlaceId = w.GooglePlaceId,
                    TaskDetails = w.TaskDetails
                })
                .ToList();
            TripValidator.ValidateWaypoints(existingForValidation, dto.Type);
        }

        var previousContractorId = trip.ContractorId;
        trip.ContractorId = dto.ContractorId;

        // Customs section
        TripValidator.ValidateCustomsToggle(trip, dto.HasCustoms);

        if (dto.HasCustoms && trip.CustomsStatus == TripCustomsStatus.None)
        {
            trip.CustomsStatus = TripCustomsStatus.PendingEntrance;
        }
        if (!dto.HasCustoms && trip.CustomsStatus != TripCustomsStatus.None)
        {
            trip.CustomsStatus = TripCustomsStatus.None;
            trip.CustomsTotalAmount = null;
        }

        if (dto.Waypoints != null && trip.Status == TripStatus.Completed)
        {
            bool hasDestinationId = dto.DestinationId is > 0;

            // Any destination change (set / change / clear): same wipe+replace as before (Ids ignored, id 0 OK).
            if (destinationUpdated)
            {
                TripValidator.ValidateWaypoints(dto.Waypoints, dto.Type);

                var existingWaypoints = (await _tripWaypointRepository.GetByTripIdAsync(dto.Id)).ToList();
                if (existingWaypoints.Count > 0)
                    _tripWaypointRepository.RemoveRange(existingWaypoints);

                var replacedWaypoints = dto.Waypoints
                    .Select(w => new TripWaypoint
                    {
                        TripId = trip.Id,
                        Sequence = w.Sequence,
                        StopType = w.StopType,
                        Latitude = w.Latitude,
                        Longitude = w.Longitude,
                        AddressName = w.AddressName ?? string.Empty,
                        GooglePlaceId = w.GooglePlaceId,
                        TaskDetails = w.TaskDetails,
                    }).ToList();

                if (replacedWaypoints.Count > 0)
                    await _tripWaypointRepository.AddRangeAsync(replacedWaypoints);
            }
            // Dest unchanged and null: patch existing waypoints by Id (keep status / recording links).
            else if (!hasDestinationId)
            {
                TripValidator.ValidateWaypoints(dto.Waypoints, dto.Type);

                var existingById = (await _tripWaypointRepository.GetByTripIdAsync(dto.Id))
                    .ToDictionary(w => w.Id);

                foreach (var w in dto.Waypoints)
                {
                    if (w.Id <= 0 || !existingById.TryGetValue(w.Id, out var existing))
                        throw new ValidationException(TripMessageKeys.WaypointNotInTrip);

                    existing.Sequence = w.Sequence;
                    existing.StopType = w.StopType;
                    existing.Latitude = w.Latitude;
                    existing.Longitude = w.Longitude;
                    existing.AddressName = w.AddressName ?? string.Empty;
                    existing.GooglePlaceId = w.GooglePlaceId;
                    existing.TaskDetails = w.TaskDetails;
                    _tripWaypointRepository.Update(existing);
                }
            }
        }

        if (dto.Waypoints != null && trip.Status <= TripStatus.InProgress)
        {
            var existingWaypoints = (await _tripWaypointRepository.GetByTripIdAsync(dto.Id)).ToList();
            var pendingWaypoints = existingWaypoints.Where(w => w.Status == TripWaypointStatus.Pending).ToList();
            var nonPendingWaypoints = existingWaypoints.Where(w => w.Status != TripWaypointStatus.Pending).ToList();
            HashSet<long> nonPendingIds = nonPendingWaypoints.Select(w => w.Id).ToHashSet();
            HashSet<int> nonPendingSequences = nonPendingWaypoints.Select(w => w.Sequence).ToHashSet();

            List<TripWaypointDto> incomingNew = dto.Waypoints
                .Where(w => w.Id == 0 || !nonPendingIds.Contains(w.Id))
                .Where(w => !nonPendingSequences.Contains(w.Sequence))
                .ToList();

            List<TripWaypointDto> mergedForValidation = nonPendingWaypoints
                .Select(w => new TripWaypointDto
                {
                    Id = w.Id,
                    Sequence = w.Sequence,
                    StopType = w.StopType,
                    Latitude = w.Latitude,
                    Longitude = w.Longitude,
                    AddressName = w.AddressName,
                    GooglePlaceId = w.GooglePlaceId
                })
                .Concat(incomingNew)
                .OrderBy(w => w.Sequence)
                .ToList();

            TripValidator.ValidateWaypoints(mergedForValidation, dto.Type);

            _tripWaypointRepository.RemoveRange(pendingWaypoints);

            var newWaypoints = incomingNew
                .Select(w => new TripWaypoint
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

            if (newWaypoints.Count > 0)
                await _tripWaypointRepository.AddRangeAsync(newWaypoints);
        }

        bool hasBeenScheduledBefore = trip.Status == TripStatus.Scheduled;
        TripUpdateHelper.ApplyStatus(
            trip,
            dto.ContractorId,
            trip.DriverId,
            dto.VehicleId,
            dto.StartDate,
            dto.EndDate);

        if (dto.PushToAllContractors && previousContractorId.HasValue && dto.ContractorId is null)
            TripValidator.ValidatePushToAllContractors(trip);

        if (previousDispatchingMode == TripDispatchingMode.Contractors
            && effectiveDispatchingMode == TripDispatchingMode.Loadly)
        {
            await TripBookingHelper.CloseAsync(
                trip.Id,
                _tripBookingRequestRepository,
                _tripBookingInvitationRepository,
                _invitationRoundRepository,
                currentUserId);
        }

        _tripRepository.Update(trip);

        await _TripVehicleAssignmentSnapshotRecorder.SyncForTripAsync(
            trip.Id,
            headVehicle,
            trip.TailVehicleId,
            tailVehicle: null,
            freezeSnapshot: trip.Status == TripStatus.Completed,
            addedBy: currentUserId);

        await _tripRepository.SaveChangesAsync();

        if (shouldRecalculateCharge)
        {
            await _calculateAndPersistTripChargeAction.ExecuteAsync(new CalculateAndPersistTripChargeDto
            {
                TripId = trip.Id
            });
            await _tripRepository.SaveChangesAsync();
        }

        if (canUpdateDriver && !trip.IsExternalTrip && headVehicle != null && headVehicle.DriverId.HasValue && (!hasBeenScheduledBefore || driverChanged || schedulingChanged))
        {
            TripScheduleDriverPushKind pushKind = driverChanged
                ? TripScheduleDriverPushKind.DriverChangedOnTripUpdate
                : TripScheduleDriverPushKind.ScheduleUpdatedOnTripUpdate;

            await _notificationDispatcher.DispatchAsync(new TripScheduledNotificationData
            {
                TripId = trip.Id,
                CompanyId = contract!.CompanyId,
                DriverId = headVehicle.DriverId.Value,
                DriverPushKind = pushKind
            });
        }

        if (dto.PushToAllContractors && previousContractorId.HasValue && dto.ContractorId is null)
        {
            await _pushTripToContractorHelper.InviteContractorsAsync(
                trip,
                sendToAllContractors: true,
                contractorIds: null);
        }

        if (previousContractorId.HasValue && previousContractorId != dto.ContractorId)
        {
            await _notificationDispatcher.DispatchAsync(new TripContractorUnassignedNotificationData
            {
                TripId = trip.Id,
                ContractorId = previousContractorId.Value
            });
        }
    }
}
