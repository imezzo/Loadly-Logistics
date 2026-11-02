using LoadlyAggregator.Domain.Admin.Actions.Trips.Responses;
using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Trips;

public class BulkCreateTripsAction
{
    private readonly ITripRepository _tripRepository;
    private readonly IContractRepository _contractRepository;
    private readonly IContractDestinationRepository _contractDestinationRepository;
    private readonly IContractVehicleTypeRepository _contractVehicleTypeRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IDriverRepository _driverRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ITripContractDestinationRepository _tripContractDestinationRepository;
    private readonly TripVehicleAssignmentSnapshotRecorder _TripVehicleAssignmentSnapshotRecorder;

    public BulkCreateTripsAction(
        ITripRepository tripRepository,
        IContractRepository contractRepository,
        IContractDestinationRepository contractDestinationRepository,
        IContractVehicleTypeRepository contractVehicleTypeRepository,
        IVehicleRepository vehicleRepository,
        IDriverRepository driverRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ITripContractDestinationRepository tripContractDestinationRepository,
        TripVehicleAssignmentSnapshotRecorder TripVehicleAssignmentSnapshotRecorder)
    {
        _tripRepository = tripRepository;
        _contractRepository = contractRepository;
        _contractDestinationRepository = contractDestinationRepository;
        _contractVehicleTypeRepository = contractVehicleTypeRepository;
        _vehicleRepository = vehicleRepository;
        _driverRepository = driverRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _tripContractDestinationRepository = tripContractDestinationRepository;
        _TripVehicleAssignmentSnapshotRecorder = TripVehicleAssignmentSnapshotRecorder;
    }

    public async Task<BulkCreateTripsResponse> ExecuteAsync(BulkCreateTripsDto dto)
    {
        if (dto.Trips == null || !dto.Trips.Any())
        {
            throw new ValidationException(TripMessageKeys.ListCannotBeEmpty);
        }

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var contract = await _contractRepository.GetByIdAsync(dto.ContractId) ?? throw new NotFoundException("contracts.not_found_with_id", dto.ContractId);
            TripValidator.ValidateContractIsActive(contract);

            var headVehicleIds = dto.Trips.Select(t => t.VehicleId).Distinct().ToList();
            var driverIds = dto.Trips
                .Where(t => t.DriverId.HasValue)
                .Select(t => t.DriverId!.Value)
                .Distinct()
                .ToList();

            var headVehicles = new Dictionary<long, Vehicle>();
            if (headVehicleIds.Any())
            {
                var vehicles = await _vehicleRepository.FindAsync(v => headVehicleIds.Contains(v.Id));
                var vehiclesList = vehicles.ToList();

                foreach (var vehicleId in headVehicleIds)
                {
                    var vehicle = vehiclesList.FirstOrDefault(v => v.Id == vehicleId) ?? throw new NotFoundException("trips.head_vehicle_not_found_with_id", vehicleId);
                    if (vehicle.HeadVehicleId.HasValue)
                    {
                        throw new LogicalException("trips.head_vehicle_must_be_head", vehicleId);
                    }
                    headVehicles[vehicleId] = vehicle;
                }
            }

            var drivers = new Dictionary<long, Common.Entities.Driver>();
            if (driverIds.Any())
            {
                var foundDrivers = await _driverRepository.FindAsync(d => driverIds.Contains(d.Id));
                var driversList = foundDrivers.ToList();

                foreach (var driverId in driverIds)
                {
                    var driver = driversList.FirstOrDefault(d => d.Id == driverId) ?? throw new NotFoundException("drivers.not_found_with_id", driverId);
                    drivers[driverId] = driver;
                }
            }

            var validationErrors = new List<string>();
            for (int i = 0; i < dto.Trips.Count; i++)
            {
                var tripItem = dto.Trips[i];

                try
                {
                    TripValidator.ValidateDateRange(tripItem.StartDate, tripItem.EndDate);
                }
                catch (ValidationException ex)
                {
                    validationErrors.Add($"Trip #{i + 1}: {ex.Message}");
                }

                try
                {
                    TripValidator.ValidateTripStartDateNotInPast(tripItem.StartDate);
                }
                catch (ValidationException ex)
                {
                    validationErrors.Add($"Trip #{i + 1}: {ex.Message}");
                }

                try
                {
                    TripValidator.ValidateTripDatesWithinContractRange(contract, tripItem.StartDate, tripItem.EndDate);
                }
                catch (LogicalException ex)
                {
                    validationErrors.Add($"Trip #{i + 1}: {ex.Message}");
                }

                try
                {
                    TripValidator.ValidateFinancialDate(contract, tripItem.FinancialDate);
                }
                catch (ValidationException ex)
                {
                    validationErrors.Add($"Trip #{i + 1}: {ex.Message}");
                }

                try
                {
                    TripValidator.ValidateContractDistanceKm(contract, tripItem.ContractDistanceKm);
                }
                catch (ValidationException ex)
                {
                    validationErrors.Add($"Trip #{i + 1}: {ex.Message}");
                }

                var contractVehicleType = await _contractVehicleTypeRepository.GetByContractIdAndVehicleTypeIdAsync(dto.ContractId, tripItem.VehicleTypeId);
                if (contractVehicleType == null)
                    validationErrors.Add($"Trip #{i + 1}: Contract vehicle type for vehicle type ID {tripItem.VehicleTypeId} not found in the specified contract");

                if (!headVehicles.ContainsKey(tripItem.VehicleId))
                {
                    validationErrors.Add($"Trip #{i + 1}: Head vehicle with ID {tripItem.VehicleId} not found or is not a head");
                }
                else
                {
                    var headVehicle = headVehicles[tripItem.VehicleId];
                    try
                    {
                        TripValidator.ValidateHeadVehicle(headVehicle);
                    }
                    catch (Exception ex) when (ex is LogicalException or ValidationException)
                    {
                        validationErrors.Add($"Trip #{i + 1}: {ex.Message}");
                    }
                }

                if (tripItem.DriverId.HasValue)
                {
                    if (!drivers.ContainsKey(tripItem.DriverId.Value))
                    {
                        validationErrors.Add($"Trip #{i + 1}: Driver with ID {tripItem.DriverId.Value} not found");
                    }
                    else
                    {
                        var driver = drivers[tripItem.DriverId.Value];
                        if (headVehicles.ContainsKey(tripItem.VehicleId))
                        {
                            var headVehicle = headVehicles[tripItem.VehicleId];
                            try
                            {
                                TripValidator.ValidateIsActiveDriver(driver);
                            }
                            catch (LogicalException ex)
                            {
                                validationErrors.Add($"Trip #{i + 1}: {ex.Message}");
                            }
                        }
                    }
                }
            }

            foreach (var tripItem in dto.Trips)
            {
                var tripIndex = dto.Trips.IndexOf(tripItem) + 1;

                if (TripValidator.HasOverlappingHeadVehicleTrips(_tripRepository, tripItem.VehicleId, tripItem.StartDate, tripItem.EndDate))
                {
                    validationErrors.Add($"Trip #{tripIndex}: Head vehicle with ID {tripItem.VehicleId} is already assigned to another trip during this time period");
                }

                if (tripItem.DriverId.HasValue)
                {
                    if (TripValidator.HasOverlappingDriverTrips(_tripRepository, tripItem.DriverId.Value, tripItem.StartDate, tripItem.EndDate))
                    {
                        validationErrors.Add($"Trip #{tripIndex}: Driver with ID {tripItem.DriverId.Value} is already assigned to another trip during this time period");
                    }
                }
            }

            for (int i = 0; i < dto.Trips.Count; i++)
            {
                var tripItem1 = dto.Trips[i];
                for (int j = i + 1; j < dto.Trips.Count; j++)
                {
                    var tripItem2 = dto.Trips[j];

                    if (tripItem1.VehicleId == tripItem2.VehicleId)
                    {
                        if (TripValidator.HasOverlappingDateRanges(tripItem1.StartDate, tripItem1.EndDate, tripItem2.StartDate, tripItem2.EndDate))
                        {
                            validationErrors.Add($"Trips #{i + 1} and #{j + 1}: Head vehicle with ID {tripItem1.VehicleId} is assigned to overlapping trips");
                        }
                    }

                    if (tripItem1.DriverId.HasValue && tripItem2.DriverId.HasValue
                        && tripItem1.DriverId == tripItem2.DriverId)
                    {
                        if (TripValidator.HasOverlappingDateRanges(tripItem1.StartDate, tripItem1.EndDate, tripItem2.StartDate, tripItem2.EndDate))
                        {
                            validationErrors.Add($"Trips #{i + 1} and #{j + 1}: Driver with ID {tripItem1.DriverId.Value} is assigned to overlapping trips");
                        }
                    }
                }
            }

            if (validationErrors.Any())
            {
                throw new ValidationException(
                    "trips.bulk_validation_failed",
                    validationErrors.Count,
                    string.Join("\n", validationErrors));
            }

            long? dispatcherId = _currentUserService.GetCurrentUserId();

            var createdTrips = new List<Trip>();
            var tripDestinations = new List<ContractDestination?>();
            foreach (var tripItem in dto.Trips)
            {
                var contractVehicleType = await _contractVehicleTypeRepository.GetByContractIdAndVehicleTypeIdAsync(dto.ContractId, tripItem.VehicleTypeId)
                    ?? throw new NotFoundException("trips.contract_vehicle_type_not_found_for_vehicle_type", tripItem.VehicleTypeId);

                ContractDestination? tripDestination = await TripContractDestinationHelper.GetOptionalContractDestinationForTripAsync(
                    dto.ContractId,
                    tripItem.DestinationId,
                    _contractDestinationRepository);

                var numberOfDays = TripContractDestinationHelper.ResolveNumberOfDays(
                    tripItem.StartDate,
                    tripItem.EndDate,
                    tripDestination?.NumberOfDays);
                var financialDate = TripFinancialDateResolver.Resolve(contract, tripItem.StartDate, tripItem.FinancialDate);

                tripDestinations.Add(tripDestination);
                createdTrips.Add(new Trip
                {
                    ContractId = dto.ContractId,
                    NumberOfDays = numberOfDays,
                    ContractVehicleTypeId = contractVehicleType.Id,
                    ContractSlotId = TripContractSlotHelper.ResolveContractSlotIdForTrip(
                        contract,
                        tripItem.VehicleId,
                        financialDate,
                        numberOfDays,
                        tripItem.StartDate),
                    VehicleId = tripItem.VehicleId,
                    DriverId = tripItem.DriverId,
                    StartDate = tripItem.StartDate,
                    EndDate = tripItem.EndDate,
                    FinancialDate = financialDate,
                    Status = TripStatus.Pending,
                    DispatcherId = dispatcherId,
                    ContractDistanceKm = tripItem.ContractDistanceKm,
                    IsBackfill = false
                });
            }

            foreach (var trip in createdTrips.OrderBy(t => t.StartDate!.Value))
                await _tripRepository.AddAsync(trip);

            await _tripRepository.SaveChangesAsync();

            long addedBy = dispatcherId ?? 0;
            for (int i = 0; i < createdTrips.Count; i++)
            {
                ContractDestination? destination = tripDestinations[i];
                if (destination == null)
                    continue;

                await _tripContractDestinationRepository.AddAsync(new TripContractDestination
                {
                    TripId = createdTrips[i].Id,
                    DestinationId = destination.Id,
                    OutboundDistanceKm = destination.ExpectedKms ?? 0,
                    AddedBy = addedBy,
                });
            }

            await _tripContractDestinationRepository.SaveChangesAsync();

            foreach (var trip in createdTrips.Where(t => t.VehicleId is > 0))
            {
                var headVehicle = await _vehicleRepository.GetByIdAsync(trip.VehicleId!.Value);
                await _TripVehicleAssignmentSnapshotRecorder.SyncForTripAsync(
                    trip.Id,
                    headVehicle,
                    trip.TailVehicleId,
                    tailVehicle: null,
                    freezeSnapshot: false,
                    addedBy: _currentUserService.GetCurrentUserId());
            }

            await _tripRepository.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return new BulkCreateTripsResponse
            {
                Summary = new BulkTripsSummary
                {
                    TotalTrips = dto.Trips.Count,
                    SuccessCount = createdTrips.Count,
                    FailedCount = 0
                },
                CreatedTripIds = createdTrips.Select(t => t.Id).ToList()
            };
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
