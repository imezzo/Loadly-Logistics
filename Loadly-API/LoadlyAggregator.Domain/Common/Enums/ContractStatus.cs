using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum ContractStatus
{
    [Display("Draft", "مسودة")]
    DRAFT = 0,
    [Display("Pending Approval", "بانتظار الاعتماد")]
    PENDING_APPROVAL = 1,
    [Display("Active", "ساري")]
    ACTIVE = 2,
    [Display("Inactive", "غير ساري")]
    INACTIVE = 3,
    [Display("Expired", "منتهي الصلاحية")]
    EXPIRED = 4,
    [Display("Renewed", "مجدد")]
    RENEWED = 5,
    [Display("Terminated", "ملغى")]
    TERMINATED = 6
}
