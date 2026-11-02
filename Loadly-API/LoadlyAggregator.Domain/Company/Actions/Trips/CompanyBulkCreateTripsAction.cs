using LoadlyAggregator.Domain.Admin.DTOs.Maps;
using LoadlyAggregator.Domain.Admin.DTOs.Trips;
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

public class CompanyBulkCreateTripsAction
{
    private readonly ITripRepository _tripRepository;
    private readonly IContractRepository _contractRepository;
    private readonly IContractVehicleTypeRepository _contractVehicleTypeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IGoogleMapsPlaceExtractor _placeExtractor;
    private readonly INotificationEventDispatcher _notificationDispatcher;

    public CompanyBulkCreateTripsAction(
        ITripRepository tripRepository,
        IContractRepository contractRepository,
        IContractVehicleTypeRepository contractVehicleTypeRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IGoogleMapsPlaceExtractor placeExtractor,
        INotificationEventDispatcher notificationDispatcher)
    {
        _tripRepository = tripRepository;
        _contractRepository = contractRepository;
        _contractVehicleTypeRepository = contractVehicleTypeRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _placeExtractor = placeExtractor;
        _notificationDispatcher = notificationDispatcher;
    }

    public async Task<BulkImportTripsResultDto> ExecuteAsync(CompanyBulkCreateTripsDto dto)
    {
        if (dto.Trips is not { Count: > 0 })
            throw new ValidationException(TripMessageKeys.ListCannotBeEmpty);

        var contract = await _contractRepository.GetByIdOrThrowAsync(dto.ContractId);

        if (contract.CompanyId != dto.CompanyId)
            throw new ForbiddenException(TripMessageKeys.NotAuthorized);

        TripValidator.ValidateContractIsActive(contract);

        var contractVehicleType = await _contractVehicleTypeRepository
                .GetByContractIdAndVehicleTypeIdAsync(dto.ContractId, dto.VehicleTypeId)
            ?? throw new NotFoundException("trips.contract_vehicle_type_not_found_for_vehicle_type", dto.VehicleTypeId);

        var trips = new List<Trip>();
        var errors = new List<string>();

        for (var i = 0; i < dto.Trips.Count; i++)
        {
            try
            {
                trips.Add(await BuildTripAsync(dto.Trips[i], dto, contract, contractVehicleType));
            }
            catch (Exception ex) when (ex is ValidationException or LogicalException or NotFoundException or ForbiddenException)
            {
                errors.Add($"Item {i + 1}: {GetErrorMessage(ex)}");
            }
        }

        if (errors.Count > 0)
            throw new ValidationException("validation.bulk_import_row_errors", string.Join("; ", errors));

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            await _tripRepository.AddRangeAsync(trips.OrderBy(t => t.StartDate!.Value));

            await _tripRepository.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            foreach (var trip in trips)
            {
                await _notificationDispatcher.DispatchAsync(new TripRequestedByCompanyNotificationData
                {
                    TripId = trip.Id,
                    CompanyId = contract.CompanyId
                });
            }

            return new BulkImportTripsResultDto { Created = trips.Count };
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    private async Task<Trip> BuildTripAsync(
        CompanyBulkCreateTripItemDto item,
        CompanyBulkCreateTripsDto dto,
        Contract contract,
        ContractVehicleType contractVehicleType)
    {
        var from = await _placeExtractor.ExtractAsync(item.From?.Trim() ?? "");
        var to = await _placeExtractor.ExtractAsync(item.To?.Trim() ?? "");

        var waypoints = new List<TripWaypoint>
        {
            ToWaypoint(from, 0, TripWaypointStopType.Pickup),
            ToWaypoint(to, 1, TripWaypointStopType.Dropoff)
        };

        TripValidator.ValidateContractDistanceKm(contract, item.DistanceKms);
        TripValidator.ValidateDateRange(item.StartDate, item.EndDate);
        TripValidator.ValidateTripStartDateNotInPast(item.StartDate);
        TripValidator.ValidateTripDatesWithinContractRange(contract, item.StartDate, item.EndDate);
        TripValidator.ValidateDestinationRequired(contract, null);
        TripValidator.ValidateWaypoints(waypoints.ConvertAll(w => new CompanyCreateTripWaypointDto
        {
            Sequence = w.Sequence,
            StopType = w.StopType,
            Latitude = w.Latitude,
            Longitude = w.Longitude,
            AddressName = w.AddressName,
            GooglePlaceId = w.GooglePlaceId
        }));

        return new Trip
        {
            ContractId = dto.ContractId,
            ContractVehicleTypeId = contractVehicleType.Id,
            StartDate = item.StartDate,
            EndDate = item.EndDate,
            Status = TripStatus.Pending,
            TripDocumentStatus = TripDocumentStatus.Pending,
            CompanyUserId = _currentUserService.GetCurrentUserId(),
            ContractDistanceKm = item.DistanceKms,
            HelpersCount = item.HelpersCount,
            ContactPersonName = item.ContactName?.Trim(),
            ContactPersonMobile = item.ContactMobile?.Trim(),
            IsBackfill = false,
            Waypoints = waypoints
        };
    }

    private static TripWaypoint ToWaypoint(GoogleMapsPlaceDto place, int sequence, TripWaypointStopType stopType) =>
        new()
        {
            Sequence = sequence,
            StopType = stopType,
            Latitude = place.Latitude,
            Longitude = place.Longitude,
            AddressName = place.Address ?? string.Empty,
            GooglePlaceId = place.PlaceId
        };

    private static string GetErrorMessage(Exception ex) =>
        ex is ILocalizedException localized ? localized.MessageKey : ex.Message;
}
