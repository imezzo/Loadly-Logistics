using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Common.Services.Validation;

public static class VehicleFacilityVisitValidator
{
    public static void ValidateServiceFacilityForEnter(VehicleFacility location)
    {
        if (location.Status != VehicleFacilityStatus.ACTIVE)
            throw new ValidationException("vehicle_facilities.inactive");

        ValidateServiceFacilityNotParking(location);
    }

    public static void ValidateServiceFacilityNotParking(VehicleFacility location)
    {
        if (location.FacilityType == VehicleFacilityType.Parking)
            throw new ValidationException("vehicle_facilities.parking_not_allowed_for_service_visit");
    }

    public static void ValidateParkingFacility(VehicleFacility location)
    {
        if (location.Status != VehicleFacilityStatus.ACTIVE)
            throw new ValidationException("vehicle_facilities.inactive");

        if (location.FacilityType != VehicleFacilityType.Parking)
            throw new ValidationException("vehicles.assigned_parking_facility_must_be_parking_type");
    }

    public static void ValidateCanExitParking(Vehicle vehicle, long? requestedFacilityId)
    {
        if (!vehicle.AssignedParkingFacilityId.HasValue)
            throw new ValidationException("vehicles.no_assigned_parking_facility");

        if (requestedFacilityId.HasValue && vehicle.AssignedParkingFacilityId != requestedFacilityId)
            throw new ValidationException("vehicles.parking_location_mismatch");
    }

    public static void ValidateCanEnter(Vehicle vehicle)
    {
        if (vehicle.Status == VehicleStatus.InRoute)
            throw new ValidationException("vehicles.cannot_enter_location_while_in_route");

        if (vehicle.CurrentServiceFacilityId.HasValue || vehicle.Status == VehicleStatus.OutOfService)
            throw new ValidationException("vehicles.already_parked_at_location");
    }

    public static void ValidateCanExit(Vehicle vehicle, long facilityId)
    {
        if (vehicle.Status != VehicleStatus.OutOfService)
            throw new ValidationException("vehicles.not_parked_at_location");

        if (vehicle.CurrentServiceFacilityId != facilityId)
            throw new ValidationException("vehicles.parked_location_mismatch");
    }

    public static async Task ValidateNoInProgressTripsAsync(
        ITripRepository tripRepository,
        IEnumerable<Vehicle> vehicles)
    {
        foreach (var vehicle in vehicles)
        {
            if (await tripRepository.ExistsAsync(t => t.VehicleId == vehicle.Id && t.Status == TripStatus.InProgress))
                throw new ValidationException("vehicles.cannot_enter_location_with_in_progress_trip");
        }
    }

    public static void ValidateEnterEventDate(DateTime eventDate, DateTime? lastVisitEventDate)
    {
        if (lastVisitEventDate is { } last && eventDate < last)
            throw new ValidationException("vehicle_facility_visits.event_date_before_last_visit");
    }

    public static async Task ValidateEnterEventDateAsync(
        IVehicleFacilityVisitRepository visitRepository,
        long vehicleId,
        DateTime eventDate)
    {
        ValidateEnterEventDate(eventDate, await visitRepository.GetLatestEventDateAsync(vehicleId));
    }

    public static async Task ValidateEnterEventDatesAsync(
        IVehicleFacilityVisitRepository visitRepository,
        IEnumerable<long> vehicleIds,
        DateTime eventDate)
    {
        foreach (var vehicleId in vehicleIds)
            await ValidateEnterEventDateAsync(visitRepository, vehicleId, eventDate);
    }
}
