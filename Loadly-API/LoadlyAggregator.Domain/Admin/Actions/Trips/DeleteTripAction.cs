using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Trips;

public class DeleteTripAction
{
    private readonly ITripRepository _tripRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IDriverRepository _driverRepository;

    public DeleteTripAction(
        ITripRepository tripRepository,
        IVehicleRepository vehicleRepository,
        IDriverRepository driverRepository)
    {
        _tripRepository = tripRepository;
        _vehicleRepository = vehicleRepository;
        _driverRepository = driverRepository;
    }

    public async Task ExecuteAsync(DeleteTripDto dto)
    {
        var trip = await _tripRepository.GetByIdOrThrowAsync(dto.Id);
        if (trip.Status == TripStatus.CompanyDraft)
            throw new NotFoundException("trips.not_found_with_id", dto.Id);

        TripValidator.ValidateTripNotAuditedForDelete(trip);

        if (trip.Status == TripStatus.InProgress)
            await ReleaseInProgressResourcesAsync(trip.VehicleId, trip.TailVehicleId, trip.DriverId);

        _tripRepository.Remove(trip);
        await _tripRepository.SaveChangesAsync();
    }

    private async Task ReleaseInProgressResourcesAsync(long? vehicleId, long? tailVehicleId, long? driverId)
    {
        if (vehicleId.HasValue)
        {
            await _vehicleRepository.UpdateStatusAsync(vehicleId.Value, VehicleStatus.Available);
            if (tailVehicleId.HasValue)
                await _vehicleRepository.UpdateStatusAsync(tailVehicleId.Value, VehicleStatus.Available);
        }

        if (driverId.HasValue)
            await _driverRepository.UpdateWorkingStatusAsync(driverId.Value, DriverWorkingStatus.Available);
    }
}
