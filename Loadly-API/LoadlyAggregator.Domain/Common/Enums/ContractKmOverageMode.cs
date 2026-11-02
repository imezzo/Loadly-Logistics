using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum ContractKmOverageMode
{
    [Display("Per Slot", "لكل مركبة على حدى")]
    PerSlot = 1,

    [Display("Pooled Across Slots", "جميع المركبات")]
    PooledAcrossSlots = 2
}
