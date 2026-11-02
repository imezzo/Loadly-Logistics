using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum OverNightCostBearer
{
    [Display("None", "لا يوجد")]
    None = 0,
    [Display("Customer", "العميل")]
    Customer = 1,
    [Display("Loadly", "صن واي")]
    Loadly = 2
}
