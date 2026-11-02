using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class TripContractSlotHelper
{
    public static bool ShouldDeriveContractSlot(Contract contract) =>
        contract.ContractType is ContractType.Excluded or ContractType.Included;

    public static long? ResolveContractSlotIdForTrip(
        Contract? contract,
        long? vehicleId,
        DateOnly? financialDate = null,
        int? numberOfDays = null,
        DateTime? tripStartUtc = null,
        long? existingContractSlotId = null)
    {
        if (contract is null || contract.AutoAssignMode || vehicleId is not > 0 || !ShouldDeriveContractSlot(contract))
            return null;

        if (existingContractSlotId is long existingId
            && contract.ContractSlots.FirstOrDefault(cs => cs.Id == existingId) is { } pinned
            && (financialDate is { } pinnedBillingStart
                ? ContractSlotBillingTripMatcher.BillingDaysOverlapSlotAssignment(
                    pinnedBillingStart, numberOfDays, pinned.AssignedAt, pinned.ArchivedAt)
                : tripStartUtc is { } pinnedTripStart
                  && ContractSlotBillingTripMatcher.IsWithinSlotAssignmentWindow(
                      pinnedTripStart, pinned.AssignedAt, pinned.ArchivedAt)))
            return existingId;

        var slotsForVehicle = contract.ContractSlots.Where(cs => cs.VehicleId == vehicleId.Value);

        var match = financialDate is { } billingStart
            ? NewestMatchingSlot(slotsForVehicle, cs =>
                ContractSlotBillingTripMatcher.BillingDaysOverlapSlotAssignment(
                    billingStart, numberOfDays, cs.AssignedAt, cs.ArchivedAt))
            : tripStartUtc is { } tripStart
                ? NewestMatchingSlot(slotsForVehicle, cs =>
                    ContractSlotBillingTripMatcher.IsWithinSlotAssignmentWindow(
                        tripStart, cs.AssignedAt, cs.ArchivedAt))
                : null;

        match ??= slotsForVehicle.FirstOrDefault(cs => cs.Status == ContractSlotStatus.ACTIVE);
        match ??= financialDate is null && tripStartUtc is { } calendarStart
            ? NewestMatchingSlot(slotsForVehicle, cs =>
                ContractSlotBillingTripMatcher.IsWithinSlotAssignmentCalendarWindow(
                    calendarStart, cs.AssignedAt, cs.ArchivedAt))
            : null;

        return match?.Id
            ?? throw new ValidationException(TripMessageKeys.ContractSlotRequired);
    }

    private static ContractSlot? NewestMatchingSlot(
        IEnumerable<ContractSlot> slots,
        Func<ContractSlot, bool> predicate) =>
        slots.Where(predicate).OrderByDescending(cs => cs.AssignedAt).FirstOrDefault();
}
