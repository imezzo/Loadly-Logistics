using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum PortType
{
    [Display("Land", "بري")]
    LAND = 1,

    [Display("Sea", "بحري")]
    SEA = 2
}
