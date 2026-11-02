using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum InvoiceType
{
    [Display("Trips", "رحلات")]
    Trips = 1,

    [Display("Claims", "مطالبات")]
    Claims = 2,
}
