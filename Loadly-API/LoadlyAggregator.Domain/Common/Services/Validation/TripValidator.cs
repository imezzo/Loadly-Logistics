using System.Linq.Expressions;
using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Models;
using LoadlyAggregator.Domain.Company.DTOs.Trips;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Shared.Time;
using LoadlyAggregator.Shared.Exceptions;
using LoadlyAggregator.Shared.Utilities;

namespace LoadlyAggregator.Domain.Common.Services.Validation;

public static class TripValidator
{
    public static bool HasOverlappingHeadVehicleTrips(
        ITripRepository tripRepository,
        long vehicleId,
        DateTime startDate,
        DateTime endDate,
        long? excludeTripId = null) =>
        HasOverlappingTrips(
            tripRepository,
            t => t.VehicleId == vehicleId,
            startDate,
            endDate,
            excludeTripId);

    public static bool HasOverlappingDriverTrips(
        ITripRepository tripRepository,
        long driverId,
        DateTime startDate,
        DateTime endDate,
        long? excludeTripId = null) =>
        HasOverlappingTrips(
            tripRepository,
            t => t.DriverId == driverId,
            startDate,
            endDate,
            excludeTripId);

    private static bool HasOverlappingTrips(
        ITripRepository tripRepository,
        Expression<Func<Common.Entities.Trip, bool>> entityFilter,
        DateTime startDate,
        DateTime endDate,
        long? excludeTripId)
    {
        var query = tripRepository.QueryWithoutIncludes()
            .Where(entityFilter)
            .Where(t => !t.DeletedAt.HasValue
                && t.Status != TripStatus.Completed
                && t.Status != TripStatus.Cancelled
                && t.StartDate.HasValue
                && t.EndDate.HasValue
                && ((t.StartDate <= startDate && t.EndDate >= startDate)
                    || (t.StartDate <= endDate && t.EndDate >= endDate)
                    || (t.StartDate >= startDate && t.EndDate <= endDate)));

        if (excludeTripId.HasValue)
            query = query.Where(t => t.Id != excludeTripId.Value);

        return query.Any();
    }

    public static void ValidateNoOverlappingHeadVehicleTrips(
        ITripRepository tripRepository,
        long vehicleId,
        DateTime startDate,
        DateTime endDate,
        long? excludeTripId = null)
    {
        if (HasOverlappingHeadVehicleTrips(tripRepository, vehicleId, startDate, endDate, excludeTripId))
            throw new LogicalException("trips.head_vehicle_overlapping_trips");
    }

    public static void ValidateNoOverlappingDriverTrips(
        ITripRepository tripRepository,
        long driverId,
        DateTime startDate,
        DateTime endDate,
        long? excludeTripId = null)
    {
        if (HasOverlappingDriverTrips(tripRepository, driverId, startDate, endDate, excludeTripId))
            throw new LogicalException("trips.driver_overlapping_trips");
    }

    public static bool HasOverlappingDateRanges(
        DateTime startDate1,
        DateTime endDate1,
        DateTime startDate2,
        DateTime endDate2) =>
        (startDate1 <= startDate2 && endDate1 >= startDate2)
        || (startDate1 <= endDate2 && endDate1 >= endDate2)
        || (startDate1 >= startDate2 && endDate1 <= endDate2);

    public static void ValidateHeadVehicle(Common.Entities.Vehicle vehicle)
    {
        if (vehicle.HeadVehicleId.HasValue)
            throw new LogicalException("trips.vehicle_must_be_head");

        if (vehicle.Status is VehicleStatus.OutOfService)
            throw new LogicalException("trips.head_vehicle_not_available");

        if (!vehicle.DriverId.HasValue)
            throw new LogicalException(TripMessageKeys.VehicleMustHaveAssignedDriver);
    }

    public static void ValidateTailVehicleAvailable(Common.Entities.Vehicle? tailVehicle)
    {
        if (tailVehicle is { Status: not VehicleStatus.Available })
            throw new LogicalException(TripMessageKeys.TailVehicleNotAvailable);
    }

    public static void ValidateTripTailMatchesHeadAttachedTail(Common.Entities.Vehicle? headVehicle, long? tripTailVehicleId)
    {
        if (headVehicle == null)
            return;

        var attachedTailId = headVehicle.TailVehicles.FirstOrDefault()?.Id;
        if (attachedTailId != tripTailVehicleId)
            throw new LogicalException(TripMessageKeys.TailVehicleNotAssignedToHead);
    }

    public static void ValidateTailAttachedWhenContractVehicleTypeIsTail(
        ContractVehicleType? contractVehicleType,
        Common.Entities.Vehicle? headVehicle)
    {
        if (headVehicle == null)
            return;

        if (contractVehicleType?.VehicleType?.VehicleUnitType != VehicleUnitType.Tail)
            return;

        if (headVehicle.TailVehicles.FirstOrDefault() == null)
            throw new LogicalException(TripMessageKeys.HeadVehicleMustHaveTailForTailType);
    }

    public static void ValidateIsActiveDriver(Common.Entities.Driver driver)
    {
        if (driver.Status != UserStatus.ACTIVE)
            throw new LogicalException("trips.driver_not_active");
    }

    public static void ValidateContractRequired(bool isExternalTrip, long? contractId)
    {
        if (isExternalTrip && contractId.HasValue)
            throw new ValidationException(TripMessageKeys.ContractIdMustBeNullForExternalTrip);

        if (!isExternalTrip && (!contractId.HasValue || contractId.Value <= 0))
            throw new ValidationException(TripMessageKeys.ContractIdRequiredForNonExternalTrip);
    }

    public static void ValidateVehicleRequiredWhenInProgress(TripStatus status, long? vehicleId)
    {
        if (status >= TripStatus.InProgress && !(vehicleId is > 0))
            throw new ValidationException(TripMessageKeys.VehicleRequiredWhenInProgress);
    }

    public static void ValidateContractNotChangedWhenInProgressOrLater(
        TripStatus status,
        long? existingContractId,
        long? newContractId)
    {
        if (status >= TripStatus.InProgress && existingContractId != newContractId)
            throw new ValidationException(TripMessageKeys.ContractLockedAfterTripStarted);
    }

    public static void ValidateDateLocks(Trip trip, DateTime newStartDate, DateTime newEndDate, bool destinationUpdated = false)
    {
        if (trip.Status is TripStatus.InProgress or TripStatus.Completed
            && trip.StartDate != newStartDate)
            throw new ValidationException(TripMessageKeys.StartDateLockedAfterTripStarted);

        if (trip.Status == TripStatus.Completed && trip.EndDate != newEndDate && !destinationUpdated)
            throw new ValidationException(TripMessageKeys.EndDateLockedAfterTripCompleted);
    }

    public static void ValidateLeadForExternalTrip(
        bool isExternalTrip,
        long? leadId,
        string? contactPersonName,
        string? contactPersonMobile)
    {
        if (!isExternalTrip)
        {
            if (leadId.HasValue)
                throw new ValidationException(TripMessageKeys.LeadIdMustBeNullForNonExternalTrip);
            return;
        }

        if (leadId is > 0)
            return;

        if (string.IsNullOrWhiteSpace(contactPersonName) || string.IsNullOrWhiteSpace(contactPersonMobile))
            throw new ValidationException(TripMessageKeys.LeadIdOrContactRequiredForExternalTrip);
    }

    public static void ValidateContractIsActive(Common.Entities.Contract contract)
    {
        if (contract.Status != ContractStatus.ACTIVE)
            throw new LogicalException("trips.contract_not_active");
    }

    public static void ValidateActualCargoWeightTons(Common.Entities.Contract contract, decimal? actualCargoWeightTons)
    {
        if (contract.ConsumptionType == ContractConsumptionTypes.CARGO_WEIGHTS
            && (!actualCargoWeightTons.HasValue || actualCargoWeightTons.Value <= 0))
            throw new ValidationException("trips.actual_cargo_weight_tons_required");
    }

    public static void ValidateCargoPrice(Common.Entities.Contract contract, decimal? cargoPrice)
    {
        if (contract.ConsumptionType == ContractConsumptionTypes.CARGO_PRICE_COMMISSION
            && (!cargoPrice.HasValue || cargoPrice.Value <= 0))
            throw new ValidationException("trips.cargo_price_required");
    }

    // Per-trip negotiated price required for SPOT_PRICING contracts.
    public static void ValidateSpotPrice(Common.Entities.Contract contract, decimal? spotPrice)
    {
        if (contract.ConsumptionType == ContractConsumptionTypes.SPOT_PRICING
            && (!spotPrice.HasValue || spotPrice.Value <= 0))
            throw new ValidationException("trips.spot_price_required");
    }

    public static void ValidateContractDistanceKm(Common.Entities.Contract contract, decimal? contractDistanceKm)
    {
        if (contract.ConsumptionType != ContractConsumptionTypes.Trips)
            return;

        if (!contractDistanceKm.HasValue)
            throw new ValidationException("validation.trip_contract_distance_required");

        if (contractDistanceKm.Value <= 0)
            throw new ValidationException("validation.trip_contract_distance_gte_zero");

        if (contractDistanceKm.Value > 100000)
            throw new ValidationException("validation.trip_contract_distance_max_km");
    }

    public static void ValidateFinancialDate(Common.Entities.Contract? contract, DateOnly? financialDate)
    {
        if (contract is null || !ContractCalculationHelper.IsMonthlyContract(contract.ContractType))
            return;

        if (!financialDate.HasValue)
            throw new ValidationException("trips.financial_date_required");
    }

    public static bool ShouldSkipContractorPriceValidation(TripStatus status) =>
        status is TripStatus.InProgress or TripStatus.Completed;

    public static void ValidateContractorPrice(
        Common.Entities.Contract contract,
        decimal? contractorPrice,
        TripDispatchingMode? dispatchingMode = null)
    {
        if (dispatchingMode == TripDispatchingMode.Loadly)
            return;

        if (!TripContractorPricingHelper.RequiresContractorPrice(contract))
            return;

        if (!contractorPrice.HasValue || contractorPrice.Value <= 0)
            throw new ValidationException(TripMessageKeys.ContractorPriceRequired);
    }

    public static void ValidateSubmittedContractorPrice(
        Contract contract,
        TripStatus tripStatus,
        decimal? submittedContractorPrice,
        decimal? actualCargoWeightTons,
        ContractVehicleType? contractVehicleType,
        ContractDestination? destination,
        long? contractSlotId,
        TripDispatchingMode? dispatchingMode = null)
    {
        if (ShouldSkipContractorPriceValidation(tripStatus))
            return;

        ValidateContractorPrice(contract, submittedContractorPrice, dispatchingMode);
        if (dispatchingMode == TripDispatchingMode.Loadly)
            return;

        if (!TripContractorPricingHelper.RequiresFormulaValidation(contract.ConsumptionType))
            return;

        var expected = TripContractorPricingHelper.CalculateExpected(
            contract,
            actualCargoWeightTons,
            contractVehicleType,
            destination,
            contractSlotId);

        if (!expected.HasValue || expected.Value <= 0)
            throw new ValidationException(TripMessageKeys.ContractorPriceCalculationUnavailable);

        if (!TripContractorPricingHelper.AmountsMatch(submittedContractorPrice!.Value, expected.Value))
            throw new ValidationException(TripMessageKeys.ContractorPriceMismatch);
    }

    /// <summary>External trips: optional manual contractor price (same entry model as on-demand spot pricing; omit when not covered).</summary>
    public static void ValidateSubmittedExternalTripContractorPrice(
        bool isExternalTrip,
        TripStatus tripStatus,
        decimal? submittedContractorPrice,
        TripDispatchingMode? dispatchingMode = null)
    {
        if (dispatchingMode == TripDispatchingMode.Loadly)
            return;

        if (!isExternalTrip || ShouldSkipContractorPriceValidation(tripStatus))
            return;

        if (submittedContractorPrice is > 0)
            return;

        if (submittedContractorPrice.HasValue)
            throw new ValidationException(TripMessageKeys.ContractorPriceRequired);
    }

    public static void ValidateContractorPriceUpdateAllowed(
        Trip trip,
        decimal? requestedContractorPrice,
        TripDispatchingMode? dispatchingMode = null)
    {
        if (dispatchingMode == TripDispatchingMode.Loadly)
            return;

        if (trip.Status is not TripStatus.InProgress and not TripStatus.Completed)
            return;

        if (requestedContractorPrice != trip.ContractorPrice)
            throw new ValidationException(TripMessageKeys.ContractorPriceLockedWhenInProgressOrCompleted);
    }

    public static decimal? ResolveContractorPriceForUpdate(Trip trip, decimal? requestedContractorPrice) =>
        trip.Status is TripStatus.InProgress or TripStatus.Completed
            ? trip.ContractorPrice
            : requestedContractorPrice;

    public static void ValidateDestinationRequired(Common.Entities.Contract contract, long? destinationId)
    {
        bool requiresDestination = contract.ConsumptionType is
            ContractConsumptionTypes.CARGO_WEIGHTS or
            ContractConsumptionTypes.DESTINATION_FIXED_PRICE;
            // or ContractConsumptionTypes.SPOT_PRICING;

        if (requiresDestination && destinationId is null or <= 0)
            throw new ValidationException(TripMessageKeys.DestinationRequired);
    }

    public static void ValidateTripDatesWithinContractRange(
        Common.Entities.Contract contract,
        DateTime tripStartDate,
        DateTime tripEndDate)
    {
        if (tripStartDate < contract.StartDate || tripEndDate > contract.EndDate)
            throw new LogicalException("trips.trip_dates_outside_contract_range");
    }

    public static void ValidateDateRange(DateTime startDate, DateTime endDate)
    {
        if (endDate <= startDate)
            throw new ValidationException("validation.end_date_before_start_date");
    }

    public static void ValidateTripStartDateNotInPast(DateTime? startDate)
    {
        if (startDate.HasValue && AppUtcTime.UtcDateAtMidnight(startDate.Value) < AppUtcTime.UtcTodayAtMidnight(TimeProvider.System))
            throw new ValidationException(TripMessageKeys.StartDateInPast);
    }

    public static void ValidateTailVehicleIdWhenContractVehicleTypeIsTail(
        ContractVehicleType? contractVehicleType,
        long? tailVehicleId)
    {
        if (contractVehicleType?.VehicleType?.VehicleUnitType != VehicleUnitType.Tail)
            return;

        if (tailVehicleId is null or <= 0)
            throw new ValidationException(TripMessageKeys.TailVehicleIdRequiredForTailType);
    }

    public static void ValidateDriverOwnsTrip(Common.Entities.Trip trip, long driverId)
    {
        if (trip.DriverId != driverId)
            throw new ForbiddenException(TripMessageKeys.DriverOnly);
    }

    public static async Task ValidateDriverHasNoInProgressTripAsync(
        ITripRepository tripRepository,
        long driverId)
    {
        bool hasInProgressTrip = await tripRepository.ExistsAsync(t =>
            t.DriverId == driverId && t.Status == TripStatus.InProgress);

        if (hasInProgressTrip)
            throw new LogicalException(TripMessageKeys.DriverHasInProgressTrip);
    }

    public static async Task ValidateUpdateTripPermissionByStatusAsync(
        Common.Entities.Admin admin,
        Common.Entities.Trip trip,
        IPermissionService permissionService)
    {
        string? requiredPermission = trip.Status switch
        {
            TripStatus.InProgress => "edit_trip:in_progress",
            TripStatus.Completed => "edit_trip:completed",
            _ => null
        };

        if (requiredPermission == null)
            return;

        if (!await permissionService.HasPermissionToAsync(admin, requiredPermission))
            throw new ForbiddenException(TripMessageKeys.NotAuthorized);
    }

    public static void ValidateWaypointBelongsToTrip(Common.Entities.TripWaypoint waypoint, long tripId)
    {
        if (waypoint.TripId != tripId)
            throw new ValidationException(TripMessageKeys.WaypointNotInTrip);
    }

    public static void ValidateWaypointSequencesUnique(IEnumerable<int> sequences)
    {
        var duplicates = sequences
            .GroupBy(s => s)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();
        if (duplicates.Count > 0)
            throw new ValidationException(TripMessageKeys.DuplicateWaypointSequences);
    }

    public static void ValidateWaypointSequencesUniqueAndOrderedFromZero(IEnumerable<int> sequences)
    {
        var list = sequences.ToList();
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] != i)
                throw new ValidationException(TripMessageKeys.WaypointSequencesMustBeOrderedFromZero);
        }
    }

    public static void ValidateTripTypeWithContractDestination(
        TripType tripType,
        ContractDestination? destination,
        long? destinationId = null,
        ContractDestination? returnDestination = null,
        long? returnDestinationId = null)
    {
        if (returnDestinationId is > 0 && destinationId is null or <= 0)
            throw new ValidationException(TripMessageKeys.OpenJawReturnDestinationRequired);

        if (tripType != TripType.OpenJaw && returnDestinationId is > 0)
            throw new ValidationException(TripMessageKeys.ReturnDestinationNotAllowedForTripType);

        if (tripType == TripType.OpenJaw
            && destinationId is > 0
            && returnDestinationId is null or <= 0)
        {
            throw new ValidationException(TripMessageKeys.OpenJawReturnDestinationRequired);
        }

        if (destination == null)
            return;

        if (tripType == TripType.RoundTrip
            && destination.ReturnExpectedKms is not > 0)
        {
            throw new ValidationException(TripMessageKeys.ReturnExpectedKmsRequiredForReturnTrip);
        }

        if (tripType == TripType.OpenJaw
            && returnDestination != null
            && returnDestination.Id == destination.Id)
        {
            throw new ValidationException(TripMessageKeys.OpenJawReturnDestinationMustDifferFromDestination);
        }
    }

    public static void ValidateWaypoints(IEnumerable<TripWaypointDto>? waypoints, TripType? tripType = null)
    {
        var list = waypoints?.ToList() ?? new List<TripWaypointDto>();
        if (tripType == TripType.RoundTrip && list.Count == 0)
            throw new ValidationException(TripMessageKeys.RoundTripWaypointsRequired);

        if (tripType == TripType.OpenJaw && list.Count == 0)
            throw new ValidationException(TripMessageKeys.OpenJawWaypointsRequired);

        if (list.Count == 0)
            return;

        ValidateWaypointSequencesUniqueAndOrderedFromZero(list.Select(w => w.Sequence));
        ValidateWaypointPickupDropoffRules(list.Select(w => (w.Sequence, w.StopType)));
        if (tripType == TripType.OneWay)
            ValidateOneWayWaypoints(list);
        if (tripType == TripType.RoundTrip)
            ValidateRoundTripWaypoints(list);
        if (tripType == TripType.OpenJaw)
            ValidateOpenJawWaypoints(list);
        ValidateNoDuplicateWaypointLocations(list, tripType);
    }

    public static void ValidateWaypoints(IEnumerable<CompanyCreateTripWaypointDto>? waypoints)
    {
        var list = waypoints?.ToList() ?? new List<CompanyCreateTripWaypointDto>();
        if (list.Count == 0)
            return;

        ValidateWaypointSequencesUniqueAndOrderedFromZero(list.Select(w => w.Sequence));
        ValidateWaypointPickupDropoffRules(list.Select(w => (w.Sequence, w.StopType)));
        ValidateNoDuplicateWaypointLocations(
            list.Where(w => w.Latitude.HasValue && w.Longitude.HasValue)
                .Select(w => (w.Latitude!.Value, w.Longitude!.Value)));
    }

    private static void ValidateOneWayWaypoints(IReadOnlyList<TripWaypointDto> waypoints)
    {
        if (waypoints.Any(w => w.StopType == TripWaypointStopType.Depot))
            throw new ValidationException(TripMessageKeys.OneWayDepotNotAllowed);
    }

    private static void ValidateRoundTripWaypoints(IReadOnlyList<TripWaypointDto> waypoints)
    {
        var ordered = waypoints.OrderBy(w => w.Sequence).ToList();
        var finalStop = ordered[^1];
        if (finalStop.StopType is not TripWaypointStopType.Depot and not TripWaypointStopType.Dropoff)
            throw new ValidationException(TripMessageKeys.RoundTripMustEndWithDepotOrDropoff);

        if (!WaypointLocationFieldsExactlyMatch(ordered[0], finalStop))
            throw new ValidationException(TripMessageKeys.RoundTripDepotMustMatchFirstPickup);
    }

    private static void ValidateOpenJawWaypoints(IReadOnlyList<TripWaypointDto> waypoints)
    {
        var ordered = waypoints.OrderBy(w => w.Sequence).ToList();
        
        if (ordered.Count < 2)
            throw new ValidationException(TripMessageKeys.OpenJawWaypointsRequired);
        
        if (!ordered.Any(w => w.StopType == TripWaypointStopType.Dropoff))
            throw new ValidationException(TripMessageKeys.OpenJawMustHaveDropoff);
        
        var finalStop = ordered[^1];
        if (finalStop.StopType is not TripWaypointStopType.Depot and not TripWaypointStopType.Dropoff)
            throw new ValidationException(TripMessageKeys.OpenJawMustEndWithDepotOrDropoff);

        if (WaypointLocationFieldsExactlyMatch(ordered[0], finalStop))
            throw new ValidationException(TripMessageKeys.OpenJawFinalStopMustDifferFromFirstPickup);
    }

    private static bool WaypointLocationFieldsExactlyMatch(TripWaypointDto a, TripWaypointDto b) =>
        a.Latitude == b.Latitude
        && a.Longitude == b.Longitude
        && string.Equals(a.AddressName, b.AddressName, StringComparison.Ordinal)
        && string.Equals(a.GooglePlaceId, b.GooglePlaceId, StringComparison.Ordinal);

    private static void ValidateNoDuplicateWaypointLocations(IEnumerable<TripWaypointDto> waypoints, TripType? tripType = null)
    {
        var list = waypoints?.ToList() ?? new List<TripWaypointDto>();
        if (list.Count == 0)
            return;

        var withCoords = list.Where(w => w.Latitude.HasValue && w.Longitude.HasValue).ToList();
        if (withCoords.Count < 2)
            return;

        int? exemptPickupSequence = null;
        int? exemptLastSequence = null;
        if (tripType == TripType.RoundTrip)
        {
            var ordered = list.OrderBy(w => w.Sequence).ToList();
            var first = ordered[0];
            var last = ordered[^1];
            // For round trips, first pickup and last stop (Depot or Dropoff) may share the same location
            if (first.StopType == TripWaypointStopType.Pickup
                && last.StopType is TripWaypointStopType.Depot or TripWaypointStopType.Dropoff
                && WaypointLocationFieldsExactlyMatch(first, last))
            {
                exemptPickupSequence = first.Sequence;
                exemptLastSequence = last.Sequence;
            }
        }

        const int precision = 6;
        var duplicateGroups = withCoords
            .GroupBy(w => (Lat: Math.Round(w.Latitude!.Value, precision), Lng: Math.Round(w.Longitude!.Value, precision)))
            .Where(g => g.Count() > 1);

        foreach (var group in duplicateGroups)
        {
            var sequences = group.Select(w => w.Sequence).ToHashSet();
            if (exemptPickupSequence.HasValue
                && sequences.Count == 2
                && sequences.Contains(exemptPickupSequence.Value)
                && sequences.Contains(exemptLastSequence!.Value))
            {
                continue;
            }

            throw new ValidationException(TripMessageKeys.DuplicateWaypointLocations);
        }
    }

    private static void ValidateNoDuplicateWaypointLocations(IEnumerable<(decimal Latitude, decimal Longitude)> waypoints)
    {
        var list = waypoints?.ToList() ?? new List<(decimal Latitude, decimal Longitude)>();
        if (list.Count < 2)
            return;

        ValidateNoDuplicateWaypointLocationsCore(list);
    }

    private static void ValidateNoDuplicateWaypointLocationsCore(IEnumerable<(decimal Latitude, decimal Longitude)> waypoints)
    {
        const int precision = 6;
        var byLocation = waypoints
            .GroupBy(w => (Lat: Math.Round(w.Latitude, precision), Lng: Math.Round(w.Longitude, precision)))
            .Where(g => g.Count() > 1)
            .ToList();

        if (byLocation.Count > 0)
            throw new ValidationException(TripMessageKeys.DuplicateWaypointLocations);
    }

    private static void ValidateWaypointPickupDropoffRules(IEnumerable<(int Sequence, TripWaypointStopType StopType)> waypoints)
    {
        var list = waypoints?.ToList() ?? new List<(int Sequence, TripWaypointStopType StopType)>();
        if (list.Count == 0)
            return;

        var ordered = list.OrderBy(w => w.Sequence).ToList();
        if (ordered.First().StopType != TripWaypointStopType.Pickup)
            throw new ValidationException(TripMessageKeys.TripMustStartWithPickup);

        if (!list.Any(w => w.StopType == TripWaypointStopType.Dropoff))
            throw new ValidationException(TripMessageKeys.TripMustHaveAtLeastOneDropoff);
    }

    public static void ValidateWaypointSequenceCompleted(
        List<Common.Entities.TripWaypoint> allWaypoints,
        Common.Entities.TripWaypoint waypoint)
    {
        var previous = allWaypoints.Where(w => w.Sequence < waypoint.Sequence).ToList();
        if (previous.Any(w => w.Status != TripWaypointStatus.Completed))
            throw new ValidationException(TripMessageKeys.WaypointSequenceViolation);

        // Ensure we can only arrive at the "next" waypoint - prevents skipping when sequences
        // are duplicated or when trying to arrive at a later waypoint before an earlier one.
        var nextPending = allWaypoints
            .Where(w => w.Status == TripWaypointStatus.Pending)
            .OrderBy(w => w.Sequence)
            .ThenBy(w => w.Id)
            .FirstOrDefault();
        if (nextPending != null && (nextPending.Sequence != waypoint.Sequence || nextPending.Id != waypoint.Id))
            throw new ValidationException(TripMessageKeys.WaypointSequenceViolation);
    }

    public static void ValidatePreviousWaypointsCompleted(
        List<Common.Entities.TripWaypoint> allWaypoints,
        Common.Entities.TripWaypoint waypoint)
    {
        var previous = allWaypoints.Where(w => w.Sequence < waypoint.Sequence).ToList();
        if (previous.Any(w => w.Status != TripWaypointStatus.Completed))
            throw new ValidationException(TripMessageKeys.WaypointSequenceViolation);
    }

    public static void ValidateStartLoadingCargoEligibility(
        Common.Entities.Trip trip,
        Common.Entities.TripWaypoint waypoint,
        List<Common.Entities.TripWaypoint> allWaypoints)
    {
        if (waypoint.StopType != TripWaypointStopType.Pickup)
            throw new ValidationException(TripMessageKeys.LoadCargoOnlyAtPickup);
        if (trip.Status != TripStatus.InProgress)
            throw new ValidationException(TripMessageKeys.TripMustBeInProgressAndArrivedToLoadCargo);
        if (waypoint.Status != TripWaypointStatus.Arrived)
            throw new ValidationException(TripMessageKeys.WaypointInvalidStatusForStartLoading);
        ValidatePreviousWaypointsCompleted(allWaypoints, waypoint);
    }

    public static void ValidateCompleteLoadingCargoEligibility(
        Common.Entities.Trip trip,
        Common.Entities.TripWaypoint waypoint,
        List<Common.Entities.TripWaypoint> allWaypoints)
    {
        if (waypoint.StopType != TripWaypointStopType.Pickup)
            throw new ValidationException(TripMessageKeys.LoadCargoOnlyAtPickup);
        if (waypoint.Status != TripWaypointStatus.LoadingStarted)
            throw new ValidationException(TripMessageKeys.WaypointInvalidStatusForCompleteLoading);
        if (trip.Status != TripStatus.InProgress)
            throw new ValidationException(TripMessageKeys.InvalidStatusForWaypointAction);
        ValidatePreviousWaypointsCompleted(allWaypoints, waypoint);
    }

    public static void ValidateCompleteLoadingCargoImages(FileUpload? receiptNoteImage, IEnumerable<FileUpload>? shipmentImages)
    {
        if (receiptNoteImage == null || receiptNoteImage.Length == 0)
            throw new ValidationException(TripMessageKeys.ReceiptNoteImageRequired);
        if (shipmentImages == null || !shipmentImages.Any(i => i.Length > 0))
            throw new ValidationException(TripMessageKeys.ShipmentImagesRequired);
    }

    public static void ValidateStartUnloadingCargoEligibility(
        Common.Entities.Trip trip,
        Common.Entities.TripWaypoint waypoint,
        List<Common.Entities.TripWaypoint> allWaypoints)
    {
        if (waypoint.StopType != TripWaypointStopType.Dropoff)
            throw new ValidationException(TripMessageKeys.UnloadCargoOnlyAtDropoff);
        if (trip.Status != TripStatus.InProgress)
            throw new ValidationException(TripMessageKeys.InvalidStatusForWaypointAction);
        if (waypoint.Status != TripWaypointStatus.Arrived)
            throw new ValidationException(TripMessageKeys.WaypointInvalidStatusForStartUnloading);
        ValidatePreviousWaypointsCompleted(allWaypoints, waypoint);
    }

    public static void ValidateCompleteUnloadingCargoEligibility(
        Common.Entities.Trip trip,
        Common.Entities.TripWaypoint waypoint,
        List<Common.Entities.TripWaypoint> allWaypoints)
    {
        if (waypoint.StopType != TripWaypointStopType.Dropoff)
            throw new ValidationException(TripMessageKeys.UnloadCargoOnlyAtDropoff);
        if (waypoint.Status != TripWaypointStatus.UnloadingStarted)
            throw new ValidationException(TripMessageKeys.WaypointInvalidStatusForCompleteUnloading);
        if (trip.Status != TripStatus.InProgress)
            throw new ValidationException(TripMessageKeys.InvalidStatusForWaypointAction);
        ValidatePreviousWaypointsCompleted(allWaypoints, waypoint);
    }

    public static void ValidateCompleteUnloadingCargoImages(FileUpload? deliveryNoteImage, IEnumerable<FileUpload>? unloadingImages)
    {
        if (deliveryNoteImage == null || deliveryNoteImage.Length == 0)
            throw new ValidationException(TripMessageKeys.DeliveryNoteImageRequiredAtDropoff);
        if (unloadingImages == null || !unloadingImages.Any(i => i.Length > 0))
            throw new ValidationException(TripMessageKeys.UnloadingImagesRequired);
    }

    public static void ValidateStartTripEligibility(Common.Entities.Trip trip, List<Common.Entities.TripWaypoint> allWaypoints)
    {
        if (TripContractDestinationHelper.GetCurrentContractDestination(trip)?.Status == ContractDestinationStatus.ARCHIVED)
            throw new ValidationException(TripMessageKeys.ContractDestinationInvalid);

        // Start trip = driver heads to first waypoint; trip must be Scheduled, first waypoint still Pending
        if (trip.Status != TripStatus.Scheduled)
            throw new ValidationException(TripMessageKeys.InvalidStatusForWaypointAction);
        if (allWaypoints.Count == 0)
            throw new ValidationException(TripMessageKeys.NoWaypoints);
        var firstWaypoint = allWaypoints.First();
        if (firstWaypoint.Status != TripWaypointStatus.Pending)
            throw new ValidationException(TripMessageKeys.InvalidStatusForWaypointAction);
    }

    public static void ValidateVehicleLinkedToContract(
        Contract? contract,
        Vehicle? vehicle,
        DateTime startDate,
        long? contractorId = null,
        TripStatus? tripStatus = null)
    {
        if (tripStatus == TripStatus.Completed)
            return;

        if (contractorId.HasValue)
            return;

        if (contract is null)
            return;

        if (contract.AutoAssignMode)
            return;

        if (vehicle is null)
            return;

        if (vehicle.ContractId != contract.Id)
            throw new LogicalException(TripMessageKeys.VehicleNotLinkedToContract);

        var activeSlot = contract.ContractSlots
            .FirstOrDefault(cs => cs.VehicleId == vehicle.Id && cs.Status == ContractSlotStatus.ACTIVE);

        if (activeSlot is null)
            throw new LogicalException(TripMessageKeys.VehicleNotLinkedToContract);

        if (activeSlot.AssignedAt > startDate)
            throw new LogicalException(TripMessageKeys.VehicleContractSlotAssignedAfterTripStart);
    }

    public static void ValidateArriveAtWaypointEligibility(
        Common.Entities.Trip trip,
        Common.Entities.TripWaypoint waypoint,
        List<Common.Entities.TripWaypoint> allWaypoints)
    {
        if (trip.Status != TripStatus.InProgress)
            throw new ValidationException(TripMessageKeys.InvalidStatusForWaypointAction);
        if (waypoint.Status != TripWaypointStatus.Pending)
            throw new ValidationException(TripMessageKeys.WaypointInvalidStatusForArrive);
        ValidateWaypointSequenceCompleted(allWaypoints, waypoint);
    }

    public static bool IsFirstWaypoint(Common.Entities.TripWaypoint waypoint, List<Common.Entities.TripWaypoint> allWaypoints) =>
        allWaypoints.OrderBy(w => w.Sequence).FirstOrDefault()?.Id == waypoint.Id;

    public static void ValidateOdometerRequiredForFirstWaypointDepart(
        Common.Entities.TripWaypoint waypoint,
        List<Common.Entities.TripWaypoint> allWaypoints,
        int? odometer,
        FileUpload? odometerImage)
    {
        if (!IsFirstWaypoint(waypoint, allWaypoints))
            return;
        if (!odometer.HasValue || odometer.Value < 0)
            throw new ValidationException(TripMessageKeys.OdometerRequired);
        if (odometerImage == null || odometerImage.Length == 0)
            throw new ValidationException(TripMessageKeys.OdometerImageRequired);
    }

    public static void ValidateDepartFromWaypointEligibility(
        Common.Entities.Trip trip,
        Common.Entities.TripWaypoint waypoint)
    {
        if (trip.Status != TripStatus.InProgress)
            throw new ValidationException(TripMessageKeys.InvalidStatusForWaypointAction);
        if (waypoint.StopType == TripWaypointStopType.Pickup)
        {
            if (waypoint.Status != TripWaypointStatus.LoadingCompleted)
                throw new ValidationException(TripMessageKeys.WaypointInvalidStatusForDepart);
        }
        else if (waypoint.StopType == TripWaypointStopType.Dropoff)
        {
            if (waypoint.Status != TripWaypointStatus.UnloadingCompleted)
                throw new ValidationException(TripMessageKeys.UnloadingCompletedRequiredForDepart);
        }
        else if (waypoint.StopType == TripWaypointStopType.Depot)
        {
            if (waypoint.Status != TripWaypointStatus.Arrived)
                throw new ValidationException(TripMessageKeys.WaypointInvalidStatusForDepartDepot);
        }
    }

    public static void ValidateCompleteTripEligibility(
        Common.Entities.Trip trip,
        List<Common.Entities.TripWaypoint> allWaypoints)
    {
        if (trip.Status != TripStatus.InProgress)
            throw new ValidationException(TripMessageKeys.InvalidStatusForCompletion);
        if (allWaypoints.Count == 0)
            throw new ValidationException(TripMessageKeys.NoWaypoints);

        var lastWaypoint = allWaypoints.Last();
        var allExceptLast = allWaypoints.Take(allWaypoints.Count - 1).ToList();

        if (allExceptLast.Any(w => w.Status != TripWaypointStatus.Completed))
            throw new ValidationException(TripMessageKeys.AllWaypointsMustBeCompleted);

        if (lastWaypoint.Status == TripWaypointStatus.Completed)
            return;

        if (lastWaypoint.StopType == TripWaypointStopType.Pickup)
        {
            if (lastWaypoint.Status != TripWaypointStatus.LoadingCompleted)
                throw new ValidationException(TripMessageKeys.WaypointInvalidStatusForDepart);
        }
        else if (lastWaypoint.StopType == TripWaypointStopType.Dropoff)
        {
            if (lastWaypoint.Status != TripWaypointStatus.UnloadingCompleted)
                throw new ValidationException(TripMessageKeys.UnloadingCompletedRequiredForDepart);
        }
        else if (lastWaypoint.StopType == TripWaypointStopType.Depot)
        {
            if (lastWaypoint.Status != TripWaypointStatus.Arrived)
                throw new ValidationException(TripMessageKeys.WaypointInvalidStatusForDepartDepot);
        }

        ValidateCustomsClearedForCompletion(trip);
    }

    public static void ValidateCustomsClearedForCompletion(Common.Entities.Trip trip)
    {
        if (trip.CustomsStatus is TripCustomsStatus.PendingEntrance or TripCustomsStatus.UnderInspection)
            throw new ValidationException(TripMessageKeys.CustomsMustBeClearedBeforeCompletion);
    }

    public static void ValidateFinalOdometerAgainstInitial(int finalOdometer, IEnumerable<Recording> tripRecordings)
    {
        int? tripStartOdometer = TripOdometerHelper.GetTripStartOdometerReading(tripRecordings);
        if (tripStartOdometer.HasValue && finalOdometer < tripStartOdometer.Value)
            throw new ValidationException(TripMessageKeys.FinalOdometerMustBeGreaterOrEqualInitial);
    }

    public static bool HasRequiredAssignmentAttributes(long? driverId, long vehicleId, DateTime startDate, DateTime endDate) =>
        driverId.HasValue
        && vehicleId > 0
        && startDate != default
        && endDate > startDate;

    public static bool AllWaypointsHaveCoordinates(IEnumerable<TripWaypointDto>? waypoints)
    {
        var list = waypoints?.ToList() ?? new List<TripWaypointDto>();
        return list.Count == 0 || list.All(w => w.Latitude.HasValue && w.Longitude.HasValue);
    }

    public static void ValidateDriverEntersCustomsEligibility(Common.Entities.Trip trip)
    {
        if (trip.Status != TripStatus.InProgress)
            throw new ValidationException(TripMessageKeys.InvalidStatusForWaypointAction);
        if (trip.CustomsStatus != TripCustomsStatus.PendingEntrance)
            throw new ValidationException(TripMessageKeys.CustomsMustBePendingEntranceToEnter);
    }

    public static void ValidateDriverExitsCustomsEligibility(Common.Entities.Trip trip)
    {
        if (trip.Status != TripStatus.InProgress)
            throw new ValidationException(TripMessageKeys.InvalidStatusForWaypointAction);
        if (trip.CustomsStatus != TripCustomsStatus.UnderInspection)
            throw new ValidationException(TripMessageKeys.CustomsMustBeUnderInspectionToExit);
    }

    public static void ValidateCustomsToggle(Common.Entities.Trip trip, bool hasCustoms)
    {
        bool currentlyHasCustoms = trip.CustomsStatus != TripCustomsStatus.None;
        if (currentlyHasCustoms == hasCustoms)
            return;

        if (trip.Status >= TripStatus.Completed)
            throw new ValidationException(TripMessageKeys.CustomsStatusCannotBeUpdatedAfterCompletion);
    }

    public static void ValidateTripNotAuditedOrInvoiced(Common.Entities.Trip trip)
    {
        if (trip.AuditedAt.HasValue || trip.AuditedById.HasValue)
            throw new ValidationException(TripMessageKeys.CannotUpdateWhenAudited);

        if (trip.InvoiceId.HasValue || trip.ClaimInvoiceId.HasValue)
            throw new ValidationException(TripMessageKeys.CannotUpdateWhenInvoiced);
    }

    public static void ValidateTripCompletedAndNotAudited(Common.Entities.Trip trip)
    {
        if (trip.Status != TripStatus.Completed)
            throw new ValidationException(TripMessageKeys.TripChargesUpdateOnlyWhenCompleted);

        if (trip.AuditedAt.HasValue || trip.AuditedById.HasValue)
            throw new ValidationException(TripMessageKeys.CannotUpdateWhenAudited);
    }

    public static void ValidateTripChargesNotInvoiced(Common.Entities.Trip trip)
    {
        if (trip.InvoiceId.HasValue || trip.ClaimInvoiceId.HasValue)
            throw new ValidationException(TripMessageKeys.TripChargesCannotUpdateWhenInvoiced);
    }

    public static void ValidateTripChargesNotDocumentsApproved(Common.Entities.Trip trip)
    {
        if (trip.TripDocumentStatus == TripDocumentStatus.Approved)
            throw new ValidationException(TripMessageKeys.TripChargesCannotUpdateWhenDocumentsApproved);
    }

    public static bool IsEditable(Common.Entities.Trip trip)
    {
        if (trip.AuditedAt.HasValue || trip.AuditedById.HasValue)
            return false;
        if (trip.InvoiceId.HasValue || trip.ClaimInvoiceId.HasValue)
            return false;
        return true;
    }

    public static void ValidateTripNotAuditedForDelete(Common.Entities.Trip trip)
    {
        if (!IsEditable(trip))
            throw new ValidationException(TripMessageKeys.CannotDeleteWhenAudited);
    }

    public static void ValidateProofOfDeliveryNotTripDerived(long tripId, string? proofOfDelivery)
    {
        if (string.IsNullOrWhiteSpace(proofOfDelivery))
            return;

        if (TripReferenceNumber.IsTripDerivedValue(tripId, proofOfDelivery))
            throw new ValidationException(TripMessageKeys.ProofOfDeliveryMustNotBeTripDerived);
    }

    public static void ValidateDispatchingChange(Common.Entities.Trip trip, TripDispatchingMode? dispatchingMode)
    {
        if (trip.Status >= TripStatus.InProgress
            && dispatchingMode is { } mode && mode != trip.DispatchingMode)
            throw new ValidationException(TripMessageKeys.DispatchingModeLockedAfterInProgress);
    }

    public static void ValidateContractorAssign(
        TripDispatchingMode? dispatchingMode,
        long? contractorId,
        Common.Entities.Trip? trip = null)
    {
        if (trip != null
            && trip.Status >= TripStatus.InProgress
            && contractorId != trip.ContractorId)
            throw new ValidationException(TripMessageKeys.ContractorLockedAfterInProgress);

        if (contractorId.HasValue && dispatchingMode != TripDispatchingMode.Contractors)
            throw new ValidationException(TripMessageKeys.ContractorRequiresContractorsDispatching);
    }

    public static void ValidateContractorsDispatchAllowed(
        Contract? contract, TripDispatchingMode? dispatchingMode = null, long? contractorId = null,
        bool pushToContractors = false, bool isExternalTrip = false)
    {
        if (dispatchingMode != TripDispatchingMode.Contractors && !contractorId.HasValue && !pushToContractors)
            return;
        if (isExternalTrip)
            return;
        if (contract == null)
            throw new ValidationException(TripMessageKeys.ContractRequiredForContractScopedFields);
        if (contract.ContractType != ContractType.OnDemand)
            throw new ValidationException(TripMessageKeys.ContractorsDispatchRequiresOnDemandContract);
    }

    public static void ValidatePushToAllContractors(
        TripDispatchingMode? dispatchingMode,
        long? vehicleId,
        long? contractorId,
        TripStatus? status = null)
    {
        if (dispatchingMode != TripDispatchingMode.Contractors)
            throw new ValidationException(TripMessageKeys.DispatchingModeRequiredForPush);

        if (vehicleId is > 0)
            throw new ValidationException(TripMessageKeys.PushToAllRequiresNoVehicle);

        if (contractorId.HasValue)
            throw new ValidationException(TripMessageKeys.PushToAllRequiresNoContractor);

        if (status is >= TripStatus.InProgress)
            throw new ValidationException(TripMessageKeys.PushToAllRequiresStatusBeforeInProgress);
    }

    public static void ValidatePushToAllContractors(Common.Entities.Trip trip) =>
        ValidatePushToAllContractors(trip.DispatchingMode, trip.VehicleId, trip.ContractorId, trip.Status);

    public static async Task ValidateCancelTripPermissionByStatusAsync(
        Common.Entities.Admin admin,
        Common.Entities.Trip trip,
        IPermissionService permissionService)
    {
        string? requiredPermission = trip.Status switch
        {
            TripStatus.InProgress => "cancel_trip:in_progress",
            _ => null
        };

        if (requiredPermission == null)
            return;

        if (!await permissionService.HasPermissionToAsync(admin, requiredPermission))
            throw new ForbiddenException(TripMessageKeys.NotAuthorized);
    }
}
