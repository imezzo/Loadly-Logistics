using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Common.Entities;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class ContractSlotSeatRootMapper
{
    public static void Apply(
        CalculateIncludedContractSlotChargesResultDto result,
        IReadOnlyList<ContractSlot> allContractSlots)
    {
        var slotsById = allContractSlots.ToDictionary(slot => slot.Id);
        var roots = new List<long>();
        var seen = new HashSet<long>();

        foreach (var calendar in result.Calendar)
        {
            var root = calendar.SlotId;
            if (slotsById.TryGetValue(calendar.SlotId, out var slot))
                root = IncludedContractSlotSeatHelper.ResolveSeatRootId(slot, slotsById);

            calendar.SeatRootSlotId = root;
            if (seen.Add(root))
                roots.Add(root);
        }

        result.SeatRootSlotIds = roots;
    }
}
