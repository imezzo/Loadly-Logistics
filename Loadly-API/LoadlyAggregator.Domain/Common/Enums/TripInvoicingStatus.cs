using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum TripInvoicingStatus
{
    [Display("Not Invoiced", "غير مفوترة")]
    NotInvoiced = 0,

    [Display("Partially Invoiced", "مفوترة جزئياً")]
    PartiallyInvoiced = 1,

    [Display("Fully Invoiced", "مفوترة بالكامل")]
    FullyInvoiced = 2
}
