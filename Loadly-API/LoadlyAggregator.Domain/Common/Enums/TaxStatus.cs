using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum TaxStatus
{
    [Display("Inactive", "غير نشط")]
    INACTIVE = 0,
    [Display("Active", "نشط")]
    ACTIVE = 1
}
