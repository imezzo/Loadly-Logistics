using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum TripDispatchingMode
{
    [Display("Contractors", "مقاولون")]
    Contractors = 1,
    [Display("Loadly", "شايل")]
    Loadly = 2
}
