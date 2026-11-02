using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum ApprovalStage
{
    [Display("Sales Manager", "مدير المبيعات")]
    SALES_MANAGER = 1,
    [Display("Finance Manager", "المدير المالي")]
    FINANCE_MANAGER = 2,
    [Display("Operations Manager", "مدير التشغيل")]
    OPERATIONS_MANAGER = 3
}
