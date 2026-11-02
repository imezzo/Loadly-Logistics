using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum ContractBillingSnapshotStatus
{
    [Display("Draft", "مسودة")]
    Draft = 0,

    [Display("Finalized", "مفوترة")]
    Finalized = 1,

    [Display("Partially Invoiced", "مفوترة جزئياً")]
    PartiallyInvoiced = 2
}
