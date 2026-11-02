using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Services.Validation;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class TripUpdateHelper
{
    /// <summary>
    /// Applies Scheduled/Pending transitions.
    /// Contractors mode schedules when a contractor and driver are assigned (no vehicle required)
    /// and re-opens as Pending when the contractor or driver is cleared.
    /// Loadly mode schedules via vehicle/driver assignment attributes and demotes when the vehicle is cleared.
    /// </summary>
    public static void ApplyStatus(
        Trip trip,
        long? contractorId,
        long? driverId,
        long? vehicleId,
        DateTime startDate,
        DateTime endDate)
    {
        bool vehicleAssigned = vehicleId is > 0;
        bool isContractorsMode = trip.DispatchingMode == TripDispatchingMode.Contractors;
        bool contractorCleared = !contractorId.HasValue;
        bool driverCleared = !driverId.HasValue;
        bool hasAssignmentAttributes = TripValidator.HasRequiredAssignmentAttributes(
            driverId,
            vehicleId ?? 0,
            startDate,
            endDate);

        if (trip.Status == TripStatus.Scheduled && isContractorsMode && (contractorCleared || driverCleared))
            trip.Status = TripStatus.Pending;

        if (trip.Status == TripStatus.Scheduled && !isContractorsMode && !vehicleAssigned)
            trip.Status = TripStatus.Pending;

        if (trip.Status == TripStatus.Pending && hasAssignmentAttributes)
            trip.Status = TripStatus.Scheduled;

        if (trip.Status == TripStatus.Pending && isContractorsMode && contractorId.HasValue && driverId.HasValue)
            trip.Status = TripStatus.Scheduled;
    }

    public static bool IsCompletedTripChargeRecalculationRequired(
        Trip trip,
        long? previousContractDestinationId,
        long? newContractDestinationId,
        decimal? newActualCargoWeightTons,
        decimal? newContractDistanceKm)
    {
        if (trip.Status != TripStatus.Completed)
            return false;

        if (previousContractDestinationId != newContractDestinationId)
            return true;

        if (trip.ActualCargoWeightTons != newActualCargoWeightTons)
            return true;

        if (trip.ContractDistanceKm != newContractDistanceKm)
            return true;

        return false;
    }
}
