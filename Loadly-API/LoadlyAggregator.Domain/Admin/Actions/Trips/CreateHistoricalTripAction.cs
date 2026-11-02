using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Domain.Common.Actions;
using LoadlyAggregator.Domain.Common.DTOs;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Trips;

/// <summary>
/// Backfill missed trips as Completed. Skips assignment/overlap checks; uses request DriverId/TailVehicleId as-is.
/// Head/tail in the request may no longer be attached; contract vehicle type follows historical head-unit rules.
/// </summary>
public class CreateHistoricalTripAction
{
    private readonly ITripRepository _tripRepository;
    private readonly ITripWaypointRepository _tripWaypointRepository;
    private readonly ITripContractDestinationRepository _tripContractDestinationRepository;
    private readonly IContractRepository _contractRepository;
    private readonly IContractDestinationRepository _contractDestinationRepository;
    private readonly IContractVehicleTypeRepository _contractVehicleTypeRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IDriverRepository _driverRepository;
    private readonly ILeadRepository _leadRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly CalculateAndPersistTripChargeAction _calculateAndPersistTripChargeAction;
    private readonly TripVehicleAssignmentSnapshotRecorder _TripVehicleAssignmentSnapshotRecorder;

    public CreateHistoricalTripAction(
        ITripRepository tripRepository,
        ITripWaypointRepository tripWaypointRepository,
        ITripContractDestinationRepository tripContractDestinationRepository,
        IContractRepository contractRepository,
        IContractDestinationRepository contractDestinationRepository,
        IContractVehicleTypeRepository contractVehicleTypeRepository,
        IVehicleRepository vehicleRepository,
        IDriverRepository driverRepository,
        ILeadRepository leadRepository,
        ICurrentUserService currentUserService,
        CalculateAndPersistTripChargeAction calculateAndPersistTripChargeAction,
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
        _leadRepository = leadRepository;
        _currentUserService = currentUserService;
        _calculateAndPersistTripChargeAction = calculateAndPersistTripChargeAction;
        _TripVehicleAssignmentSnapshotRecorder = TripVehicleAssignmentSnapshotRecorder;
    }

    public async Task ExecuteAsync(CreateHistoricalTripDto dto)
    {
        TripValidator.ValidateDateRange(dto.StartDate, dto.EndDate);
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
            TripValidator.ValidateTripDatesWithinContractRange(contract, dto.StartDate, dto.EndDate);
            TripValidator.ValidateActualCargoWeightTons(contract, dto.ActualCargoWeightTons);
            TripValidator.ValidateCargoPrice(contract, dto.CargoPrice);
            TripValidator.ValidateSpotPrice(contract, dto.SpotPrice);
            TripValidator.ValidateContractDistanceKm(contract, dto.ContractDistanceKm);
            TripValidator.ValidateDestinationRequired(contract, dto.DestinationId);
        }

        TripValidator.ValidateFinancialDate(contract, dto.FinancialDate);

        var headVehicle = await _vehicleRepository.GetByIdAsync(dto.VehicleId)
            ?? throw new NotFoundException("trips.head_vehicle_not_found_with_id", dto.VehicleId);

        _ = await _driverRepository.GetByIdAsync(dto.DriverId)
            ?? throw new NotFoundException("drivers.not_found_with_id", dto.DriverId);

        Vehicle? tailVehicle = null;
        if (dto.TailVehicleId is > 0)
        {
            tailVehicle = await _vehicleRepository.GetByIdAsync(dto.TailVehicleId.Value)
                ?? throw new NotFoundException("trips.tail_vehicle_not_found");
        }

        ContractVehicleType? contractVehicleType = null;
        if (dto.ContractId.HasValue)
        {
            var contractProfile = TripContractVehicleTypeHelper.GetContractProfile(contract!.ContractVehicleTypes);

            if (!TripContractVehicleTypeHelper.TryResolveHistoricalVehicleTypeId(
                    headVehicle, dto.VehicleTypeId, tailVehicle, contractProfile, out var vehicleTypeId))
            {
                throw new ValidationException(TripMessageKeys.HistoricalTailOrVehicleTypeRequired);
            }

            contractVehicleType = await _contractVehicleTypeRepository.GetByContractIdAndVehicleTypeIdAsync(
                    dto.ContractId.Value, vehicleTypeId)
                ?? throw new NotFoundException("trips.contract_vehicle_type_not_found_for_vehicle_type", vehicleTypeId);
        }

        TripValidator.ValidateTailVehicleIdWhenContractVehicleTypeIsTail(contractVehicleType, dto.TailVehicleId);

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

        var trip = new Trip
        {
            ContractId = dto.ContractId,
            NumberOfDays = numberOfDays,
            ContractVehicleTypeId = contractVehicleType?.Id,
            ContractSlotId = contractSlotId,
            VehicleId = dto.VehicleId,
            ContractorId = headVehicle.ContractorId,
            TailVehicleId = dto.TailVehicleId is > 0 ? dto.TailVehicleId : null,
            DriverId = dto.DriverId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            FinancialDate = financialDate,
            Status = TripStatus.Completed,
            TripDocumentStatus = TripDocumentStatus.Pending,
            // Completed historical: customs already done or none
            CustomsStatus = dto.HasCustoms ? TripCustomsStatus.Cleared : TripCustomsStatus.None,
            DispatcherId = dispatcherId,
            Type = dto.Type,
            ContractDistanceKm = contractDistanceKm,
            ActualCargoWeightTons = dto.ActualCargoWeightTons,
            CargoPrice = dto.CargoPrice,
            SpotPrice = dto.SpotPrice,
            ContractorPrice = dto.ContractorPrice,
            IsExternalTrip = dto.IsExternalTrip,
            IsBackfill = true,
            LeadId = tripLeadId,
            ContactPersonName = contactPersonName,
            ContactPersonMobile = contactPersonMobile,
            DispatcherNotes = string.IsNullOrWhiteSpace(dto.DispatcherNotes) ? null : dto.DispatcherNotes.Trim(),
            HelpersCount = dto.HelpersCount
        };

        await _tripRepository.AddAsync(trip);
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

        await _TripVehicleAssignmentSnapshotRecorder.SyncForTripAsync(
            trip.Id,
            headVehicle,
            trip.TailVehicleId,
            tailVehicle,
            freezeSnapshot: false,
            addedBy: _currentUserService.GetCurrentUserId());
        await _tripRepository.SaveChangesAsync();

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
                TaskDetails = w.TaskDetails,
                Status = TripWaypointStatus.Completed
            }).ToList();
            await _tripWaypointRepository.AddRangeAsync(waypointEntities);
            await _tripWaypointRepository.SaveChangesAsync();
        }

        await _calculateAndPersistTripChargeAction.ExecuteAsync(new CalculateAndPersistTripChargeDto
        {
            TripId = trip.Id
        });

        await _tripRepository.SaveChangesAsync();
    }
}
