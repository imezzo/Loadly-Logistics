using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum ApprovalStatus
{
    [Display("Pending", "قيد الانتظار")]
    PENDING = 0,
    [Display("Approved", "معتمد")]
    APPROVED = 1,
    [Display("Rejected", "مرفوض")]
    REJECTED = 2
}
