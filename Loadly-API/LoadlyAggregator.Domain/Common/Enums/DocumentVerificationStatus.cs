using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum DocumentVerificationStatus
{
    [Display("Pending", "قيد المراجعة")]
    PENDING = 0,
    [Display("Verified", "معتمد")]
    VERIFIED = 1,
    [Display("Rejected", "مرفوض")]
    REJECTED = 2,
    [Display("Expired", "منتهي الصلاحية")]
    EXPIRED = 3
}
