using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Domain.Common.Actions;
using LoadlyAggregator.Domain.Common.DTOs;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Trips;

public class CompleteTripAction
{
    private readonly ITripRepository _tripRepository;
    private readonly IDriverRepository _driverRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly CalculateAndPersistTripChargeAction _calculateAndPersistTripChargeAction;

    public CompleteTripAction(
        ITripRepository tripRepository,
        IDriverRepository driverRepository,
        IVehicleRepository vehicleRepository,
        CalculateAndPersistTripChargeAction calculateAndPersistTripChargeAction)
    {
        _tripRepository = tripRepository;
        _driverRepository = driverRepository;
        _vehicleRepository = vehicleRepository;
        _calculateAndPersistTripChargeAction = calculateAndPersistTripChargeAction;
    }

    public async Task ExecuteAsync(CompleteTripDto dto)
    {
        var trip = await _tripRepository.GetByIdOrThrowAsync(dto.Id);

        if (trip.Status is not (TripStatus.Scheduled or TripStatus.InProgress))
        {
            throw new ValidationException(TripMessageKeys.InvalidStatusForCompletion);
        }

        TripValidator.ValidateCustomsClearedForCompletion(trip);

        if (trip.Status == TripStatus.InProgress)
        {
            await _driverRepository.UpdateWorkingStatusAsync(trip.DriverId.Value, DriverWorkingStatus.Available);
            await _vehicleRepository.UpdateStatusAsync(trip.VehicleId.Value, VehicleStatus.Available);
            if (trip.TailVehicleId.HasValue)
            {
                await _vehicleRepository.UpdateStatusAsync(trip.TailVehicleId.Value, VehicleStatus.Available);
            }
        }

        trip.Status = TripStatus.Completed;
        _tripRepository.Update(trip);
        // Persist completion state first so charge calculation runs on Completed trips.
        await _tripRepository.SaveChangesAsync();

        await _calculateAndPersistTripChargeAction.ExecuteAsync(new CalculateAndPersistTripChargeDto
        {
            TripId = dto.Id
        });

        await _tripRepository.SaveChangesAsync();
    }
}
