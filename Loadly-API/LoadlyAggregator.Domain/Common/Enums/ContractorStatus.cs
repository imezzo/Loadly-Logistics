using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum ContractorStatus
{
    [Display("Inactive", "غير نشط")]
    INACTIVE = 0,
    [Display("Active", "نشط")]
    ACTIVE = 1,
    [Display("Requested", "طلب انضمام")]
    REQUESTED = 2
}
