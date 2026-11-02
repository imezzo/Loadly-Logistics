using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum ContractType
{
    [Display("Contract Excluded Solar Price", "عقد بدون سعر السولار")]
    Excluded = 1,
    [Display("Contract Included Solar Price", "عقد شامل سعر السولار")]
    Included = 2,
    [Display("Contract On Demand (Per Trip)", "عقد حسب الطلب (لكل رحلة)")]
    OnDemand = 3,
    [Display("Seasonal Contract", "عقد موسمي")]
    Seasonal = 4
}
