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

public class CompanyCreateTripAction
{
    private readonly ITripRepository _tripRepository;
    private readonly ITripWaypointRepository _tripWaypointRepository;
    private readonly ITripContractDestinationRepository _tripContractDestinationRepository;
    private readonly IContractRepository _contractRepository;
    private readonly IContractDestinationRepository _contractDestinationRepository;
    private readonly IContractVehicleTypeRepository _contractVehicleTypeRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationEventDispatcher _notificationDispatcher;

    public CompanyCreateTripAction(
        ITripRepository tripRepository,
        ITripWaypointRepository tripWaypointRepository,
        ITripContractDestinationRepository tripContractDestinationRepository,
        IContractRepository contractRepository,
        IContractDestinationRepository contractDestinationRepository,
        IContractVehicleTypeRepository contractVehicleTypeRepository,
        ICurrentUserService currentUserService,
        INotificationEventDispatcher notificationDispatcher)
    {
        _tripRepository = tripRepository;
        _tripWaypointRepository = tripWaypointRepository;
        _tripContractDestinationRepository = tripContractDestinationRepository;
        _contractRepository = contractRepository;
        _contractDestinationRepository = contractDestinationRepository;
        _contractVehicleTypeRepository = contractVehicleTypeRepository;
        _currentUserService = currentUserService;
        _notificationDispatcher = notificationDispatcher;
    }

    public async Task ExecuteAsync(CompanyCreateTripDto dto)
    {
        TripValidator.ValidateDateRange(dto.StartDate, dto.EndDate);
        TripValidator.ValidateTripStartDateNotInPast(dto.StartDate);

        var contract = await _contractRepository.GetByIdAsync(dto.ContractId)
            ?? throw new NotFoundException("contracts.not_found_with_id", dto.ContractId);

        if (contract.CompanyId != dto.CompanyId)
            throw new ForbiddenException(TripMessageKeys.NotAuthorized);

        TripValidator.ValidateContractIsActive(contract);
        TripValidator.ValidateTripDatesWithinContractRange(contract, dto.StartDate, dto.EndDate);
        TripValidator.ValidateActualCargoWeightTons(contract, dto.ActualCargoWeightTons);
        TripValidator.ValidateCargoPrice(contract, dto.CargoPrice);
        TripValidator.ValidateContractDistanceKm(contract, dto.ContractDistanceKm);
        TripValidator.ValidateDestinationRequired(contract, dto.DestinationId);

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

        if (dto.Waypoints.Count != 0)
            TripValidator.ValidateWaypoints(dto.Waypoints);

        var trip = new Trip
        {
            ContractId = dto.ContractId,
            NumberOfDays = TripContractDestinationHelper.ResolveNumberOfDays(dto.StartDate, dto.EndDate, tripDestination?.NumberOfDays),
            ContractVehicleTypeId = contractVehicleType?.Id,
            VehicleId = null,
            DriverId = null,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Status = dto.SubmitRequest ? TripStatus.Pending : TripStatus.CompanyDraft,
            TripDocumentStatus = TripDocumentStatus.Pending,
            CompanyUserId = _currentUserService.GetCurrentUserId(),
            ContractDistanceKm = dto.ContractDistanceKm,
            ActualCargoWeightTons = dto.ActualCargoWeightTons,
            CargoPrice = dto.CargoPrice,
            SpotPrice = dto.SpotPrice,
            HelpersCount = dto.HelpersCount,
            IsBackfill = false,
            ContactPersonName = dto.ContactPersonName,
            ContactPersonMobile = dto.ContactPersonMobile
        };

        await _tripRepository.AddAsync(trip);
        await _tripRepository.SaveChangesAsync();

        if (tripDestination != null)
        {
            await _tripContractDestinationRepository.AddAsync(new TripContractDestination
            {
                TripId = trip.Id,
                DestinationId = tripDestination.Id,
                OutboundDistanceKm = tripDestination.ExpectedKms ?? 0,
                AddedBy = _currentUserService.GetCurrentUserId() ?? 0,
            });
            await _tripContractDestinationRepository.SaveChangesAsync();
        }

        if (dto.Waypoints.Count != 0)
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
