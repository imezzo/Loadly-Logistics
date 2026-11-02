using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum VerificationStatus
{
    [Display("Pending", "قيد المراجعة")]
    PENDING = 0,
    [Display("Verified", "معتمد")]
    VERIFIED = 1,
    [Display("Rejected", "مرفوض")]
    REJECTED = 2
}
