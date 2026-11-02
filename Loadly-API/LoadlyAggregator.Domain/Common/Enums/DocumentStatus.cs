using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum DocumentStatus
{
    [Display("Inactive", "غير ساري")]
    INACTIVE = 0,
    [Display("Active", "ساري")]
    ACTIVE = 1
}
