using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Time;

namespace LoadlyAggregator.Domain.Common.Helpers;

/// <summary>
/// Attributes trips to contract slots for monthly slot billing.
/// Trips pinned via <see cref="Trip.ContractSlotId"/> bill to that seat when in the slot window,
/// even if a substitute vehicle ran the trip.
/// </summary>
public static class ContractSlotBillingTripMatcher
{
    public static bool MatchesSlot(Trip trip, ContractSlotBillingLoadCriteria slot)
    {
        if (trip.FinancialDate is { } financialDate)
        {
            if (!BillingDaysOverlapSlotAssignment(
                    financialDate,
                    trip.NumberOfDays,
                    slot.AssignedAtUtc,
                    slot.ArchivedAtUtc))
                return false;

            if (trip.ContractSlotId == slot.SlotId)
                return true;

            return trip.ContractSlotId is null
                   && slot.VehicleId is > 0
                   && trip.VehicleId == slot.VehicleId;
        }

        if (!trip.StartDate.HasValue
            || !IsWithinSlotAssignmentWindow(trip.StartDate.Value, slot.AssignedAtUtc, slot.ArchivedAtUtc))
            return false;

        if (trip.ContractSlotId == slot.SlotId)
            return true;

        return trip.ContractSlotId is null
               && slot.VehicleId is > 0
               && trip.VehicleId == slot.VehicleId;
    }

    public static int ResolveBillingDayCount(int? numberOfDays) =>
        numberOfDays is > 0 ? numberOfDays.Value : 1;

    public static (DateOnly Start, DateOnly End) GetBillingDayRange(DateOnly financialDate, int? numberOfDays)
    {
        int days = ResolveBillingDayCount(numberOfDays);
        return (financialDate, financialDate.AddDays(days - 1));
    }

    /// <summary>
    /// Whether a completed trip counts as active on a calendar day (financial date through number of billing days).
    /// </summary>
    public static bool CompletedTripCoversBillingDay(Trip trip, DateOnly date)
    {
        if (trip.Status != TripStatus.Completed || trip.FinancialDate is not { } financialDate)
            return false;

        var (_, billingEnd) = GetBillingDayRange(financialDate, trip.NumberOfDays);
        return date >= financialDate && date <= billingEnd;
    }

    /// <summary>
    /// Whether any billing day (financial date + number of days) falls on the slot's Egypt calendar assignment window.
    /// </summary>
    public static bool BillingDaysOverlapSlotAssignment(
        DateOnly financialDate,
        int? numberOfDays,
        DateTime assignedAtUtc,
        DateTime? archivedAtUtc)
    {
        var (tripStart, tripEnd) = GetBillingDayRange(financialDate, numberOfDays);
        var slotStart = EgyptTime.ToCalendarDate(assignedAtUtc);
        if (tripEnd < slotStart)
            return false;

        if (!archivedAtUtc.HasValue)
            return true;

        var slotEnd = EgyptTime.ToCalendarDate(archivedAtUtc.Value);
        return tripStart <= slotEnd;
    }

    public static bool IsWithinSlotAssignmentWindow(
        DateTime tripStartUtc,
        DateTime assignedAtUtc,
        DateTime? archivedAtUtc) =>
        tripStartUtc >= assignedAtUtc
        && (!archivedAtUtc.HasValue || tripStartUtc < archivedAtUtc.Value);

    /// <summary>
    /// Egypt business-calendar fallback when UTC instants fall just after <see cref="ContractSlot.ArchivedAt"/>
    /// on the same local day (e.g. archived 2 hours before trip start).
    /// </summary>
    public static bool IsWithinSlotAssignmentCalendarWindow(
        DateTime tripStartUtc,
        DateTime assignedAtUtc,
        DateTime? archivedAtUtc)
    {
        var tripDate = EgyptTime.ToCalendarDate(tripStartUtc);
        if (tripDate < EgyptTime.ToCalendarDate(assignedAtUtc))
            return false;

        return !archivedAtUtc.HasValue
               || tripDate <= EgyptTime.ToCalendarDate(archivedAtUtc.Value);
    }
}
