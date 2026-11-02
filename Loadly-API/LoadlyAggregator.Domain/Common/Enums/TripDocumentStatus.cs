using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum TripDocumentStatus
{
    [Display("Pending", "قيد الانتظار")]
    Pending = 1,

    [Display("Partially Collected", "مجمّع جزئياً")]
    PartiallyCollected = 2,

    [Display("With Factory Collector", "عند مندوب المصنع")]
    WithFactoryCollector = 3,

    [Display("Partially Confirmed Receiving By Finance Collector", "مؤكد جزئيا من مندوب المالية")]
    PartiallyConfirmed = 4,

    [Display("With Finance Team", "تم إستلام مندوب المالية")]
    WithFinanceTeam = 5,

    [Display("Approved By Finance Dept.", "تم الموافقة من الإدارة المالية")]
    Approved = 6
}
