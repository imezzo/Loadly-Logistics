using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Exceptions;
using LoadlyAggregator.Shared.Time;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class ContractSlotInvoiceReadinessValidator
{
    public static void ValidateForTripsInvoice(
        Contract contract,
        DateOnly periodStart,
        DateOnly periodEnd,
        CalculateIncludedContractSlotChargesResultDto slotCharges)
    {
        var periodStartDate = periodStart;
        var periodEndDate = periodEnd;
        var today = EgyptTime.ToCalendarDate(DateTime.UtcNow);

        var overlappingSlots = contract.ContractSlots
            .Where(slot => IncludedContractSlotChargesComputer.SlotOverlapsPeriod(slot, periodStart, periodEnd))
            .Where(slot => slot.VehicleId.HasValue)
            .ToList();

        var billedSlotIds = slotCharges.Calendar
            .Select(calendar => calendar.SlotId)
            .Concat(slotCharges.Kms.Select(summary => summary.SlotId))
            .ToHashSet();

        var missingSlotIds = overlappingSlots
            .Select(slot => slot.Id)
            .Where(slotId => !billedSlotIds.Contains(slotId))
            .ToList();

        if (missingSlotIds.Count > 0)
        {
            throw new ValidationException(
                "invoices.included_slot_billing_gap",
                string.Join(", ", missingSlotIds));
        }

        var unresolvedDates = slotCharges.Calendar
            .SelectMany(calendar => calendar.Days.Select(day => (calendar.SlotId, day)))
            .Where(entry => entry.day.Date >= periodStartDate
                            && entry.day.Date <= periodEndDate
                            && entry.day.Date < today
                            && entry.day.Disposition == ContractSlotDayDisposition.Unknown)
            .Select(entry => $"{entry.SlotId}:{entry.day.Date:yyyy-MM-dd}")
            .Distinct()
            .ToList();

        if (unresolvedDates.Count > 0)
        {
            throw new ValidationException(
                "invoices.included_slot_unresolved_calendar_days",
                string.Join(", ", unresolvedDates));
        }
    }
}
