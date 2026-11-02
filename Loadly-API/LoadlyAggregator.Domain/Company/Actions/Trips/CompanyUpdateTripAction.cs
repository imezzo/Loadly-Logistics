using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Notifications;
using LoadlyAggregator.Domain.Common.Notifications.EventData;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Domain.Company.DTOs.Trips;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Company.Actions.Trips;

public class CompanyUpdateTripAction
{
    private readonly ITripRepository _tripRepository;
    private readonly ITripWaypointRepository _tripWaypointRepository;
    private readonly ITripContractDestinationRepository _tripContractDestinationRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IContractRepository _contractRepository;
    private readonly IContractDestinationRepository _contractDestinationRepository;
    private readonly IContractVehicleTypeRepository _contractVehicleTypeRepository;
    private readonly INotificationEventDispatcher _notificationDispatcher;

    public CompanyUpdateTripAction(
        ITripRepository tripRepository,
        ITripWaypointRepository tripWaypointRepository,
        ITripContractDestinationRepository tripContractDestinationRepository,
        ICurrentUserService currentUserService,
        IContractRepository contractRepository,
        IContractDestinationRepository contractDestinationRepository,
        IContractVehicleTypeRepository contractVehicleTypeRepository,
        INotificationEventDispatcher notificationDispatcher)
    {
        _tripRepository = tripRepository;
        _tripWaypointRepository = tripWaypointRepository;
        _tripContractDestinationRepository = tripContractDestinationRepository;
        _currentUserService = currentUserService;
        _contractRepository = contractRepository;
        _contractDestinationRepository = contractDestinationRepository;
        _contractVehicleTypeRepository = contractVehicleTypeRepository;
        _notificationDispatcher = notificationDispatcher;
    }

    public async Task ExecuteAsync(CompanyUpdateTripDto dto)
    {
        TripValidator.ValidateDateRange(dto.StartDate, dto.EndDate);
        TripValidator.ValidateTripStartDateNotInPast(dto.StartDate);
        TripValidator.ValidateWaypoints(dto.Waypoints);

        var trip = await _tripRepository.GetByIdOrThrowAsync(dto.TripId);
        if (trip.Contract?.CompanyId != dto.CompanyId)
            throw new ForbiddenException(TripMessageKeys.NotAuthorized);

        if (trip.Status != TripStatus.CompanyDraft)
            throw new ValidationException("trips.can_only_update_company_draft");

        var contract = await _contractRepository.GetByIdAsync(dto.ContractId)
            ?? throw new NotFoundException("contracts.not_found_with_id", dto.ContractId);

        if (contract.CompanyId != dto.CompanyId)
            throw new ForbiddenException(TripMessageKeys.NotAuthorized);

        TripValidator.ValidateContractIsActive(contract);
        TripValidator.ValidateTripDatesWithinContractRange(contract, dto.StartDate, dto.EndDate);
        TripValidator.ValidateActualCargoWeightTons(contract, dto.ActualCargoWeightTons);
        TripValidator.ValidateCargoPrice(contract, dto.CargoPrice);
        TripValidator.ValidateContractDistanceKm(contract, dto.ContractDistanceKm);

        ContractVehicleType? contractVehicleType = null;
        if (dto.VehicleTypeId is > 0)
        {
            contractVehicleType = await _contractVehicleTypeRepository.GetByContractIdAndVehicleTypeIdAsync(dto.ContractId, dto.VehicleTypeId.Value)
                ?? throw new NotFoundException("trips.contract_vehicle_type_not_found_for_vehicle_type", dto.VehicleTypeId.Value);
        }

        ContractDestination? tripDestination = await TripContractDestinationHelper.GetOptionalContractDestinationForTripAsync(
            dto.ContractId,
            dto.DestinationId,
            _contractDestinationRepository);

        trip.ContractId = dto.ContractId;
        trip.ContractVehicleTypeId = contractVehicleType?.Id;
        trip.StartDate = dto.StartDate;
        trip.EndDate = dto.EndDate;
        trip.NumberOfDays = TripContractDestinationHelper.ResolveNumberOfDays(dto.StartDate, dto.EndDate, tripDestination?.NumberOfDays);
        trip.Status = dto.SubmitRequest ? TripStatus.Pending : TripStatus.CompanyDraft;
        trip.ContractDistanceKm = dto.ContractDistanceKm;
        trip.ActualCargoWeightTons = dto.ActualCargoWeightTons;
        trip.CargoPrice = dto.CargoPrice;
        trip.SpotPrice = dto.SpotPrice;
        trip.HelpersCount = dto.HelpersCount;
        trip.ContactPersonName = dto.ContactPersonName;
        trip.ContactPersonMobile = dto.ContactPersonMobile;

        var existingWaypoints = (await _tripWaypointRepository.GetByTripIdAsync(trip.Id)).ToList();
        if (existingWaypoints.Count > 0)
            _tripWaypointRepository.RemoveRange(existingWaypoints);

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
        if (waypointEntities.Count > 0)
            await _tripWaypointRepository.AddRangeAsync(waypointEntities);

        _tripRepository.Update(trip);
        await _tripRepository.SaveChangesAsync();

        var existingDestinations = (await _tripContractDestinationRepository.GetByTripIdAsync(trip.Id)).ToList();
        TripContractDestination? existingAssignment = existingDestinations
            .OrderByDescending(tcd => tcd.Id)
            .FirstOrDefault();
        long addedBy = _currentUserService.GetCurrentUserId() ?? 0;

        if (tripDestination == null)
        {
            if (existingAssignment != null)
                _tripContractDestinationRepository.Remove(existingAssignment);
        }
        else if (existingAssignment != null)
        {
            existingAssignment.DestinationId = tripDestination.Id;
            existingAssignment.OutboundDistanceKm = tripDestination.ExpectedKms ?? 0;
            existingAssignment.DeletedAt = null;
            _tripContractDestinationRepository.Update(existingAssignment);
        }
        else
        {
            await _tripContractDestinationRepository.AddAsync(new TripContractDestination
            {
                TripId = trip.Id,
                DestinationId = tripDestination.Id,
                OutboundDistanceKm = tripDestination.ExpectedKms ?? 0,
                AddedBy = addedBy,
            });
        }

        await _tripContractDestinationRepository.SaveChangesAsync();

        if (dto.SubmitRequest)
        {
            await _notificationDispatcher.DispatchAsync(new TripRequestedByCompanyNotificationData
            {
                TripId = trip.Id,
                CompanyId = contract.CompanyId
            });
        }
    }
}
