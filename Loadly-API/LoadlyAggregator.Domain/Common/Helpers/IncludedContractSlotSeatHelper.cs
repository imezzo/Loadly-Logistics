using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Shared.Time;

namespace LoadlyAggregator.Domain.Common.Helpers;

public readonly record struct IncludedContractSlotSeat(
    long SeatRootId,
    IReadOnlyList<ContractSlot> SlotsInOrder);

/// <summary>
/// Describes one link in a same-day replacement chain. Every link in the chain shares
/// <see cref="BaseOwnerSlotId"/>, the only slot charged for <see cref="Date"/>. <see cref="ExtraOwnerSlotId"/>
/// is set only for a two-slot swap where both vehicles had a completed trip.
/// </summary>
public readonly record struct SameDayReplacementBilling(
    DateOnly Date,
    long PredecessorSlotId,
    long SuccessorSlotId,
    long BaseOwnerSlotId,
    long? ExtraOwnerSlotId);

public static class IncludedContractSlotSeatHelper
{
    public static IReadOnlyList<IncludedContractSlotSeat> GroupIntoSeats(IReadOnlyList<ContractSlot> slots)
    {
        var slotsById = slots.ToDictionary(s => s.Id);
        var groups = new Dictionary<long, List<ContractSlot>>();

        foreach (var slot in slots)
        {
            var rootId = ResolveSeatRootId(slot, slotsById);
            if (!groups.TryGetValue(rootId, out var list))
            {
                list = [];
                groups[rootId] = list;
            }

            list.Add(slot);
        }

        return groups
            .Select(group => new IncludedContractSlotSeat(
                group.Key,
                group.Value.OrderBy(s => s.AssignedAt).ToList()))
            .ToList();
    }

    public static long ResolveSeatRootId(ContractSlot slot, IReadOnlyDictionary<long, ContractSlot> slotsById)
    {
        var current = slot;
        while (current.ReplacedSlotId is long parentId && slotsById.TryGetValue(parentId, out var parent))
            current = parent;

        return current.Id;
    }

    /// <summary>
    /// Returns the earliest assignment date across the slot's replacement chain. Each replacement is assigned
    /// on or after its predecessor's archival, so this is the root's assignment date.
    /// </summary>
    public static DateTime ResolveChainAssignedAt(
        ContractSlot slot,
        IReadOnlyDictionary<long, ContractSlot> slotsById)
    {
        var current = slot;
        var earliest = slot.AssignedAt;

        while (current.ReplacedSlotId is long parentId && slotsById.TryGetValue(parentId, out var parent))
        {
            current = parent;
            if (current.AssignedAt < earliest)
                earliest = current.AssignedAt;
        }

        return earliest;
    }

    /// <summary>
    /// Resolves how the seat is billed on same-day vehicle replacement days (a slot archived the same
    /// calendar day its replacement is assigned). A chain of replacements on that day is one billing day:
    /// the first slot with a completed trip owns it, otherwise the last slot does. Every other slot in the
    /// chain is left off that date. A direct two-slot swap where both vehicles had a trip still adds one
    /// overflow charge on the successor.
    /// </summary>
    public static IReadOnlyList<SameDayReplacementBilling> ResolveSameDayReplacementBillings(
        IReadOnlyList<ContractSlot> seatSlotsInOrder,
        Func<long, IReadOnlyList<Trip>> tripsBySlotId)
    {
        var links = new List<(ContractSlot Predecessor, ContractSlot Successor, DateOnly Date)>();
        foreach (var predecessor in seatSlotsInOrder)
        {
            if (!TryResolveSameDayReplacementSuccessor(predecessor, seatSlotsInOrder, out var successor))
                continue;

            links.Add((predecessor, successor, EgyptTime.ToCalendarDate(predecessor.ArchivedAt!.Value)));
        }

        var billings = new List<SameDayReplacementBilling>();
        var index = 0;
        while (index < links.Count)
        {
            var chain = new List<(ContractSlot Predecessor, ContractSlot Successor, DateOnly Date)>
            {
                links[index]
            };
            index++;

            while (index < links.Count
                   && links[index].Date == chain[^1].Date
                   && links[index].Predecessor.Id == chain[^1].Successor.Id)
            {
                chain.Add(links[index]);
                index++;
            }

            var slots = new List<ContractSlot> { chain[0].Predecessor };
            slots.AddRange(chain.Select(link => link.Successor));
            var swapDate = chain[0].Date;
            var baseOwnerSlotId = ResolveSameDayChainBaseOwner(slots, swapDate, tripsBySlotId);
            long? extraOwnerSlotId = ResolveSameDayChainExtraOwner(
                slots,
                swapDate,
                baseOwnerSlotId,
                tripsBySlotId);

            foreach (var link in chain)
            {
                billings.Add(new SameDayReplacementBilling(
                    swapDate,
                    link.Predecessor.Id,
                    link.Successor.Id,
                    baseOwnerSlotId,
                    extraOwnerSlotId));
            }
        }

        return billings;
    }

    private static long ResolveSameDayChainBaseOwner(
        IReadOnlyList<ContractSlot> chainSlots,
        DateOnly swapDate,
        Func<long, IReadOnlyList<Trip>> tripsBySlotId)
    {
        foreach (var slot in chainSlots)
        {
            if (ContractSlotDayClassifier.HasCompletedTripOnDate(tripsBySlotId(slot.Id), swapDate))
                return slot.Id;
        }

        return chainSlots[^1].Id;
    }

    private static long? ResolveSameDayChainExtraOwner(
        IReadOnlyList<ContractSlot> chainSlots,
        DateOnly swapDate,
        long baseOwnerSlotId,
        Func<long, IReadOnlyList<Trip>> tripsBySlotId)
    {
        if (chainSlots.Count != 2 || baseOwnerSlotId != chainSlots[0].Id)
            return null;

        return ContractSlotDayClassifier.HasCompletedTripOnDate(
            tripsBySlotId(chainSlots[1].Id),
            swapDate)
            ? chainSlots[1].Id
            : null;
    }

    public static bool TryResolveSameDayReplacementSuccessor(
        ContractSlot slot,
        IReadOnlyList<ContractSlot> seatSlotsInOrder,
        out ContractSlot successor)
    {
        successor = null!;

        if (slot.ArchivedAt is not { } archivedAt)
            return false;

        var candidate = seatSlotsInOrder.FirstOrDefault(seatSlot => seatSlot.ReplacedSlotId == slot.Id);
        if (candidate is null)
            return false;

        if (EgyptTime.ToCalendarDate(archivedAt) != EgyptTime.ToCalendarDate(candidate.AssignedAt))
            return false;

        successor = candidate;
        return true;
    }

    public static int CountActiveDaysInMonth(ContractSlot slot, DateOnly monthStart, DateOnly monthEnd)
    {
        var assignedDate = EgyptTime.ToCalendarDate(slot.AssignedAt);
        DateOnly? archivedDate = slot.ArchivedAt.HasValue
            ? EgyptTime.ToCalendarDate(slot.ArchivedAt.Value)
            : null;

        var rangeStart = assignedDate > monthStart ? assignedDate : monthStart;
        var rangeEnd = archivedDate is { } archived && archived < monthEnd ? archived : monthEnd;
        if (rangeEnd < rangeStart)
            return 0;

        return rangeEnd.DayNumber - rangeStart.DayNumber + 1;
    }

    public static int DaysInMonth(DateOnly monthStart, DateOnly monthEnd) =>
        monthEnd.DayNumber - monthStart.DayNumber + 1;

    public static IReadOnlyList<ContractSlot> GetSlotsActiveInMonth(
        IReadOnlyList<ContractSlot> seatSlots,
        DateOnly monthStart,
        DateOnly monthEnd) =>
        seatSlots
            .Where(slot => CountActiveDaysInMonth(slot, monthStart, monthEnd) > 0)
            .ToList();

    /// <summary>
    /// Returns the successor slot active in the month — used for seat-month KM allowance and overage rate
    /// when a replacement chain spans the month.
    /// </summary>
    public static ContractSlot ResolveSeatMonthTermsSlot(
        IReadOnlyList<ContractSlot> seatSlots,
        DateOnly monthStart,
        DateOnly monthEnd)
    {
        var activeInMonth = GetSlotsActiveInMonth(seatSlots, monthStart, monthEnd);
        return activeInMonth.Count > 0 ? activeInMonth[^1] : seatSlots[^1];
    }

    public static bool UsesSharedMonthlyAllowance(
        IReadOnlyList<ContractSlot> seatSlots,
        DateOnly monthStart,
        DateOnly monthEnd) =>
        GetSlotsActiveInMonth(seatSlots, monthStart, monthEnd).Count > 1;

    public static decimal ResolveMonthlyWorkingDayAllowance(
        int monthlyWorkingDays,
        IReadOnlyList<ContractSlot> seatSlots,
        DateOnly monthStart,
        DateOnly monthEnd)
    {
        var activeInMonth = GetSlotsActiveInMonth(seatSlots, monthStart, monthEnd);
        if (activeInMonth.Count == 0)
            return 0m;

        if (activeInMonth.Count > 1)
            return monthlyWorkingDays;

        var activeDays = CountActiveDaysInMonth(activeInMonth[0], monthStart, monthEnd);
        var daysInMonth = DaysInMonth(monthStart, monthEnd);
        return monthlyWorkingDays * activeDays / daysInMonth;
    }

    public static decimal ResolveMonthlyMaxKmsAllowance(
        decimal? maxKms,
        IReadOnlyList<ContractSlot> seatSlots,
        DateOnly monthStart,
        DateOnly monthEnd)
    {
        var allowance = maxKms ?? 0m;
        if (allowance <= 0m)
            return 0m;

        var activeInMonth = GetSlotsActiveInMonth(seatSlots, monthStart, monthEnd);
        if (activeInMonth.Count == 0)
            return 0m;

        if (activeInMonth.Count > 1)
            return allowance;

        var activeDays = CountActiveDaysInMonth(activeInMonth[0], monthStart, monthEnd);
        var daysInMonth = DaysInMonth(monthStart, monthEnd);
        return allowance * activeDays / daysInMonth;
    }
}
