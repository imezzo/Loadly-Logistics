using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum PortStatus
{
    [Display("Inactive", "غير نشط")]
    INACTIVE = 0,

    [Display("Active", "نشط")]
    ACTIVE = 1,

    [Display("Suspended", "موقوف")]
    SUSPENDED = 2
}
