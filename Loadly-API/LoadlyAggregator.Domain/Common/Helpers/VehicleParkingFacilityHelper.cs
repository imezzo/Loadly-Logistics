using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;

namespace LoadlyAggregator.Domain.Common.Helpers;

/// <summary>All <see cref="VehicleFacilityVisitContext.Parking"/> visits and assignment changes go through here.</summary>
public static class VehicleParkingFacilityHelper
{
    public static async Task ApplyAssignmentAsync(
        Vehicle vehicle,
        long? newParkingFacilityId,
        IVehicleFacilityRepository facilityRepository,
        IVehicleFacilityVisitRepository visitRepository,
        long? driverAtAction,
        long? addedBy,
        DateTime eventDate,
        string? reason = null,
        long? contractSlotId = null)
    {
        var previousId = vehicle.AssignedParkingFacilityId;

        if (newParkingFacilityId.HasValue)
        {
            if (newParkingFacilityId == previousId)
                return;

            var facility = await facilityRepository.GetByIdOrThrowAsync(newParkingFacilityId.Value);
            VehicleFacilityVisitValidator.ValidateParkingFacility(facility);

            if (previousId.HasValue)
            {
                await visitRepository.AddAsync(CreateVisit(
                    vehicle.Id, previousId.Value, VehicleFacilityVisitEvent.EXITS_LOCATION,
                    driverAtAction, addedBy, eventDate, reason, contractSlotId));
            }

            vehicle.AssignedParkingFacilityId = newParkingFacilityId;
            await VehicleFacilityVisitValidator.ValidateEnterEventDateAsync(visitRepository, vehicle.Id, eventDate);
            await visitRepository.AddAsync(CreateVisit(
                vehicle.Id, newParkingFacilityId.Value, VehicleFacilityVisitEvent.ENTERS_LOCATION,
                driverAtAction, addedBy, eventDate, reason, contractSlotId));
            return;
        }

        if (!previousId.HasValue)
            return;

        await ClearAssignedAsync(vehicle, visitRepository, driverAtAction, addedBy, eventDate, reason, contractSlotId);
    }

    public static Task ApplyClearAsync(
        Vehicle vehicle,
        IVehicleFacilityVisitRepository visitRepository,
        long? driverAtAction,
        long? addedBy,
        DateTime eventDate,
        string? reason = null,
        long? contractSlotId = null)
    {
        if (!vehicle.AssignedParkingFacilityId.HasValue)
            return Task.CompletedTask;

        return ClearAssignedAsync(vehicle, visitRepository, driverAtAction, addedBy, eventDate, reason, contractSlotId);
    }

    public static Task RecordExitAndClearIfAssignedAsync(
        Vehicle vehicle,
        IVehicleFacilityVisitRepository visitRepository,
        long? driverId,
        long? addedBy,
        long? contractSlotId = null) =>
        !vehicle.AssignedParkingFacilityId.HasValue
            ? Task.CompletedTask
            : ClearAssignedAsync(vehicle, visitRepository, driverId, addedBy, DateTime.UtcNow, reason: null, contractSlotId);

    private static async Task ClearAssignedAsync(
        Vehicle vehicle,
        IVehicleFacilityVisitRepository visitRepository,
        long? driverAtAction,
        long? addedBy,
        DateTime eventDate,
        string? reason,
        long? contractSlotId = null)
    {
        var facilityId = vehicle.AssignedParkingFacilityId!.Value;
        vehicle.AssignedParkingFacilityId = null;
        await visitRepository.AddAsync(CreateVisit(
            vehicle.Id, facilityId, VehicleFacilityVisitEvent.EXITS_LOCATION,
            driverAtAction, addedBy, eventDate, reason, contractSlotId));
    }

    private static VehicleFacilityVisit CreateVisit(
        long vehicleId,
        long facilityId,
        VehicleFacilityVisitEvent visitEvent,
        long? driverId,
        long? addedBy,
        DateTime eventDate,
        string? reason,
        long? contractSlotId = null) =>
        new()
        {
            VehicleId = vehicleId,
            VehicleFacilityId = facilityId,
            ContractSlotId = contractSlotId,
            Event = visitEvent,
            Context = VehicleFacilityVisitContext.Parking,
            EventDate = eventDate,
            DriverId = driverId,
            Reason = reason,
            AddedBy = addedBy
        };
}
