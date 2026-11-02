using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Shared.Time;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class ContractSlotBillingVisitAttribution
{
    public static IReadOnlyDictionary<long, IReadOnlyList<VehicleFacilityVisit>> PartitionBySlot(
        IReadOnlyList<ContractSlotBillingLoadCriteria> slots,
        IReadOnlyList<VehicleFacilityVisit> visits)
    {
        if (slots.Count == 0)
            return new Dictionary<long, IReadOnlyList<VehicleFacilityVisit>>();

        var lists = slots.ToDictionary(slot => slot.SlotId, _ => new List<VehicleFacilityVisit>());
        var slotsByVehicleId = slots
            .Where(slot => slot.VehicleId is > 0)
            .GroupBy(slot => slot.VehicleId!.Value)
            .ToDictionary(group => group.Key, group => group.ToList());

        foreach (var visit in visits)
        {
            if (!slotsByVehicleId.TryGetValue(visit.VehicleId, out var vehicleSlots))
                continue;

            if (visit.ContractSlotId is long explicitSlotId)
            {
                if (!lists.ContainsKey(explicitSlotId))
                    continue;

                if (!vehicleSlots.Any(slot => slot.SlotId == explicitSlotId))
                    continue;

                var explicitSlot = vehicleSlots.First(slot => slot.SlotId == explicitSlotId);
                if (!IsEventWithinSlotWindow(visit.EventDate, explicitSlot))
                    continue;

                lists[explicitSlotId].Add(visit);
                continue;
            }

            var candidates = vehicleSlots
                .Where(slot => IsEventWithinSlotWindow(visit.EventDate, slot))
                .ToList();
            if (candidates.Count == 0)
                continue;

            foreach (var candidate in candidates)
                lists[candidate.SlotId].Add(visit);
        }

        foreach (var slot in slots)
        {
            AppendPreAssignmentCarryIn(lists[slot.SlotId], slot, visits);
        }

        return lists.ToDictionary(
            entry => entry.Key,
            entry => (IReadOnlyList<VehicleFacilityVisit>)[.. entry.Value
                .OrderBy(visit => visit.EventDate)
                .ThenBy(visit => visit.Id)]);
    }

    /// <summary>
    /// Facility day classification needs open parking/service state from before <see cref="ContractSlotBillingLoadCriteria.AssignedAtUtc"/>.
    /// </summary>
    private static void AppendPreAssignmentCarryIn(
        List<VehicleFacilityVisit> slotVisits,
        ContractSlotBillingLoadCriteria slot,
        IReadOnlyList<VehicleFacilityVisit> allVisits)
    {
        var existingIds = slotVisits.Select(visit => visit.Id).ToHashSet();

        foreach (var visit in allVisits)
        {
            if (slot.VehicleId is not > 0
                || visit.VehicleId != slot.VehicleId
                || EgyptTime.ToCalendarDate(visit.EventDate) >= EgyptTime.ToCalendarDate(slot.AssignedAtUtc))
                continue;

            if (existingIds.Add(visit.Id))
                slotVisits.Add(visit);
        }
    }

    public static bool IsEventWithinSlotWindow(DateTime eventDateUtc, ContractSlotBillingLoadCriteria slot)
    {
        var eventDate = EgyptTime.ToCalendarDate(eventDateUtc);
        if (eventDate < EgyptTime.ToCalendarDate(slot.AssignedAtUtc))
            return false;

        return !slot.ArchivedAtUtc.HasValue || eventDate <= EgyptTime.ToCalendarDate(slot.ArchivedAtUtc.Value);
    }
}
