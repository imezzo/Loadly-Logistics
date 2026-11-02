using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum ContractDestinationStatus
{
    [Display("Active", "نشط")]
    ACTIVE = 1,
    [Display("Archived", "مؤرشف")]
    ARCHIVED = 2
}
