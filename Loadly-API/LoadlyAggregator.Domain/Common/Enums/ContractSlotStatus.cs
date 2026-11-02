using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum ContractSlotStatus
{
    [Display("Active", "نشط")]
    ACTIVE = 1,
    [Display("Archived", "مؤرشف")]
    ARCHIVED = 2
}
