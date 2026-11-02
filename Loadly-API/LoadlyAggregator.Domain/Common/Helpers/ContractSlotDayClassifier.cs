using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Time;

namespace LoadlyAggregator.Domain.Common.Helpers;

public readonly record struct ContractSlotDayEvaluation(
    DateOnly Date,
    ContractSlotDayDisposition Disposition,
    int? WorkingDayIndex,
    VehicleFacilityType? FacilityType,
    long? FacilityId = null,
    string? FacilityName = null);

public readonly record struct VehicleFacilityBillingInfo(
    VehicleFacilityType FacilityType,
    string Name);

/// <summary>
/// Predecessor slot context for same-day vehicle replacement: used when the successor vehicle has no
/// trip or facility exposure on the swap date so the day inherits the predecessor's disposition.
/// </summary>
public readonly record struct SameDaySwapPredecessorDayContext(
    DateOnly AssignedDate,
    DateOnly? ArchivedDate,
    DateTime AssignedAtUtc,
    DateTime? ArchivedAtUtc,
    long? VehicleId,
    IReadOnlyList<Trip> Trips,
    IReadOnlyList<VehicleFacilityVisit> Visits,
    IReadOnlyDictionary<long, VehicleFacilityBillingInfo> FacilityById);

/// <summary>
/// Classifies calendar days for monthly contract slot billing (Included and Excluded).
/// </summary>
public static class ContractSlotDayClassifier
{
    public static (ContractSlotDayDisposition Disposition, VehicleFacilityType? FacilityType, long? FacilityId, string? FacilityName) ClassifyDay(
        DateOnly date,
        DateOnly slotAssignedDate,
        DateOnly? slotArchivedDate,
        DateTime slotAssignedAtUtc,
        DateTime? slotArchivedAtUtc,
        long? vehicleId,
        IReadOnlyList<Trip> trips,
        IReadOnlyList<VehicleFacilityVisit> visits,
        IReadOnlyDictionary<long, VehicleFacilityBillingInfo> facilityById,
        ContractWorkingDayWindow workingDayWindow)
    {
        if (!ContractSlotBillingActiveWindow.IsWorkingDayActiveForSlot(
                date,
                slotAssignedDate,
                slotArchivedDate,
                slotAssignedAtUtc,
                slotArchivedAtUtc,
                workingDayWindow))
            return (ContractSlotDayDisposition.NotActive, null, null, null);

        if (date > EgyptTime.ToCalendarDate(DateTime.UtcNow))
            return (ContractSlotDayDisposition.Unknown, null, null, null);

        if (HasCompletedTripOnDate(trips, date))
            return (ContractSlotDayDisposition.Working, null, null, null);

        if (vehicleId.HasValue)
        {
            var (wasAtParking, wasInService, serviceFacilityType, serviceFacilityId, serviceFacilityName, parkingFacilityType, parkingFacilityId, parkingFacilityName) =
                ResolveDayFacilityExposure(
                    vehicleId.Value,
                    date,
                    visits,
                    facilityById);

            if (wasAtParking)
                return (
                    ContractSlotDayDisposition.Working,
                    parkingFacilityType ?? VehicleFacilityType.Parking,
                    parkingFacilityId,
                    parkingFacilityName);

            if (wasInService)
                return (ContractSlotDayDisposition.Skipped, serviceFacilityType, serviceFacilityId, serviceFacilityName);
        }

        return (ContractSlotDayDisposition.Working, null, null, null);
    }

    public static IReadOnlyList<ContractSlotDayEvaluation> EvaluatePeriod(
        DateOnly periodStart,
        DateOnly periodEnd,
        DateOnly slotAssignedDate,
        DateOnly? slotArchivedDate,
        DateTime slotAssignedAtUtc,
        DateTime? slotArchivedAtUtc,
        long? vehicleId,
        IReadOnlyList<Trip> trips,
        IReadOnlyList<VehicleFacilityVisit> visits,
        IReadOnlyDictionary<long, VehicleFacilityBillingInfo> facilityById,
        ContractWorkingDayWindow workingDayWindow,
        int workingDayIndexOffset = 0,
        IReadOnlySet<DateOnly>? nonBillableDates = null,
        DateOnly? sameDayReplacementSwapDate = null,
        SameDaySwapPredecessorDayContext? swapPredecessor = null)
    {
        if (periodEnd < periodStart)
            throw new ArgumentException("periodEnd must be on or after periodStart.");

        var results = new List<ContractSlotDayEvaluation>();
        var workingDayIndex = workingDayIndexOffset;

        for (var date = periodStart; date <= periodEnd; date = date.AddDays(1))
        {
            if (nonBillableDates?.Contains(date) == true)
            {
                results.Add(new ContractSlotDayEvaluation(date, ContractSlotDayDisposition.NotActive, null, null));
                continue;
            }

            var (disposition, facilityType, facilityId, facilityName) = ClassifyDay(
                date,
                slotAssignedDate,
                slotArchivedDate,
                slotAssignedAtUtc,
                slotArchivedAtUtc,
                vehicleId,
                trips,
                visits,
                facilityById,
                workingDayWindow);

            if (date == sameDayReplacementSwapDate
                && swapPredecessor is { } predecessor
                && ShouldInheritPredecessorDispositionOnSwapDay(
                    date,
                    vehicleId,
                    trips,
                    visits,
                    facilityById,
                    disposition))
            {
                (disposition, facilityType, facilityId, facilityName) = ClassifyDay(
                    date,
                    predecessor.AssignedDate,
                    predecessor.ArchivedDate,
                    predecessor.AssignedAtUtc,
                    predecessor.ArchivedAtUtc,
                    predecessor.VehicleId,
                    predecessor.Trips,
                    predecessor.Visits,
                    predecessor.FacilityById,
                    workingDayWindow);
            }

            int? index = null;
            if (IncludedContractHalfWorkingDayRules.ParticipatesInWorkingDayIndex(disposition))
            {
                workingDayIndex++;
                index = workingDayIndex;
            }

            results.Add(new ContractSlotDayEvaluation(
                date,
                disposition,
                index,
                facilityType,
                facilityId,
                facilityName));
        }

        return results;
    }

    public static int CountCompletedTripsOnDate(IReadOnlyList<Trip> trips, DateOnly date) =>
        trips.Count(t => ContractSlotBillingTripMatcher.CompletedTripCoversBillingDay(t, date));

    public static IReadOnlyList<long> ListCompletedTripIdsOnDate(IReadOnlyList<Trip> trips, DateOnly date) =>
        trips
            .Where(trip => ContractSlotBillingTripMatcher.CompletedTripCoversBillingDay(trip, date))
            .Select(trip => trip.Id)
            .Distinct()
            .OrderBy(id => id)
            .ToArray();

    public static bool HasCompletedTripOnDate(IReadOnlyList<Trip> trips, DateOnly date) =>
        trips.Any(t => ContractSlotBillingTripMatcher.CompletedTripCoversBillingDay(t, date));

    /// <summary>
    /// True when this vehicle already decides the swap day: a completed trip, parking, or time in service.
    /// A slot with no signal inherits an earlier slot's disposition.
    /// </summary>
    public static bool HasOwnSwapDaySignal(
        DateOnly date,
        long? vehicleId,
        IReadOnlyList<Trip> trips,
        IReadOnlyList<VehicleFacilityVisit> visits,
        IReadOnlyDictionary<long, VehicleFacilityBillingInfo> facilityById)
    {
        if (HasCompletedTripOnDate(trips, date))
            return true;

        if (!vehicleId.HasValue)
            return false;

        var (wasAtParking, wasInService, _, _, _, _, _, _) = ResolveDayFacilityExposure(
            vehicleId.Value,
            date,
            visits,
            facilityById);

        return wasAtParking || wasInService;
    }

    /// <summary>
    /// On a same-day replacement, the successor owns the swap date for billing when it has its own signal
    /// (trip, parking, or service). Otherwise the predecessor's disposition applies (e.g. out of service all day).
    /// </summary>
    private static bool ShouldInheritPredecessorDispositionOnSwapDay(
        DateOnly date,
        long? vehicleId,
        IReadOnlyList<Trip> trips,
        IReadOnlyList<VehicleFacilityVisit> visits,
        IReadOnlyDictionary<long, VehicleFacilityBillingInfo> facilityById,
        ContractSlotDayDisposition successorDisposition) =>
        successorDisposition != ContractSlotDayDisposition.Skipped
        && !HasOwnSwapDaySignal(date, vehicleId, trips, visits, facilityById);

    private static (
        bool WasAtParking,
        bool WasInService,
        VehicleFacilityType? ServiceFacilityType,
        long? ServiceFacilityId,
        string? ServiceFacilityName,
        VehicleFacilityType? ParkingFacilityType,
        long? ParkingFacilityId,
        string? ParkingFacilityName) ResolveDayFacilityExposure(
        long vehicleId,
        DateOnly date,
        IReadOnlyList<VehicleFacilityVisit> visits,
        IReadOnlyDictionary<long, VehicleFacilityBillingInfo> facilityById)
    {
        var vehicleVisits = visits
            .Where(v => v.VehicleId == vehicleId)
            .OrderBy(v => v.EventDate)
            .ThenBy(v => v.Id)
            .ToList();

        var atParking = false;
        var inService = false;
        VehicleFacilityType? currentServiceFacilityType = null;
        long? currentServiceFacilityId = null;
        string? currentServiceFacilityName = null;
        VehicleFacilityType? currentParkingFacilityType = null;
        long? currentParkingFacilityId = null;
        string? currentParkingFacilityName = null;

        foreach (var visit in vehicleVisits.Where(v => EgyptTime.ToCalendarDate(v.EventDate) < date))
            ApplyVisitTransition(
                visit,
                facilityById,
                ref atParking,
                ref inService,
                ref currentServiceFacilityType,
                ref currentServiceFacilityId,
                ref currentServiceFacilityName,
                ref currentParkingFacilityType,
                ref currentParkingFacilityId,
                ref currentParkingFacilityName);

        // The state carried into the day counts for the whole day: a vehicle that is in service when the day
        // opens keeps the day skipped even if it leaves partway through.
        var wasAtParking = atParking;
        var wasInService = inService;
        var serviceFacilityTypeDuringDay = inService ? currentServiceFacilityType : null;
        var serviceFacilityIdDuringDay = inService ? currentServiceFacilityId : null;
        var serviceFacilityNameDuringDay = inService ? currentServiceFacilityName : null;
        var parkingFacilityTypeDuringDay = atParking ? currentParkingFacilityType : null;
        var parkingFacilityIdDuringDay = atParking ? currentParkingFacilityId : null;
        var parkingFacilityNameDuringDay = atParking ? currentParkingFacilityName : null;

        foreach (var visit in vehicleVisits.Where(v => EgyptTime.ToCalendarDate(v.EventDate) == date))
        {
            ApplyVisitTransition(
                visit,
                facilityById,
                ref atParking,
                ref inService,
                ref currentServiceFacilityType,
                ref currentServiceFacilityId,
                ref currentServiceFacilityName,
                ref currentParkingFacilityType,
                ref currentParkingFacilityId,
                ref currentParkingFacilityName);
            wasAtParking |= atParking;
            wasInService |= inService;

            if (inService && currentServiceFacilityType.HasValue)
            {
                serviceFacilityTypeDuringDay = currentServiceFacilityType;
                serviceFacilityIdDuringDay = currentServiceFacilityId;
                serviceFacilityNameDuringDay = currentServiceFacilityName;
            }

            if (atParking && currentParkingFacilityId.HasValue)
            {
                parkingFacilityTypeDuringDay = currentParkingFacilityType;
                parkingFacilityIdDuringDay = currentParkingFacilityId;
                parkingFacilityNameDuringDay = currentParkingFacilityName;
            }
        }

        return (
            wasAtParking,
            wasInService,
            serviceFacilityTypeDuringDay,
            serviceFacilityIdDuringDay,
            serviceFacilityNameDuringDay,
            parkingFacilityTypeDuringDay,
            parkingFacilityIdDuringDay,
            parkingFacilityNameDuringDay);
    }

    private static void ApplyVisitTransition(
        VehicleFacilityVisit visit,
        IReadOnlyDictionary<long, VehicleFacilityBillingInfo> facilityById,
        ref bool atParking,
        ref bool inService,
        ref VehicleFacilityType? activeServiceFacilityType,
        ref long? activeServiceFacilityId,
        ref string? activeServiceFacilityName,
        ref VehicleFacilityType? activeParkingFacilityType,
        ref long? activeParkingFacilityId,
        ref string? activeParkingFacilityName)
    {
        if (visit.Context == VehicleFacilityVisitContext.Parking)
        {
            atParking = visit.Event == VehicleFacilityVisitEvent.ENTERS_LOCATION;
            if (!atParking)
                return;

            activeParkingFacilityId = visit.VehicleFacilityId;
            if (facilityById.TryGetValue(visit.VehicleFacilityId, out var parking))
            {
                activeParkingFacilityType = parking.FacilityType;
                activeParkingFacilityName = parking.Name;
                return;
            }

            activeParkingFacilityType = VehicleFacilityType.Parking;
            activeParkingFacilityName = null;
            return;
        }

        if (visit.Context != VehicleFacilityVisitContext.Service)
            return;

        if (!facilityById.TryGetValue(visit.VehicleFacilityId, out var facility)
            || facility.FacilityType == VehicleFacilityType.Parking)
            return;

        inService = visit.Event == VehicleFacilityVisitEvent.ENTERS_LOCATION;

        if (inService)
        {
            activeServiceFacilityType = facility.FacilityType;
            activeServiceFacilityId = visit.VehicleFacilityId;
            activeServiceFacilityName = facility.Name;
        }
    }
}
