using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum OdometerScanContext
{
    [Display("Suggest", "اقتراح")]
    Suggest = 1,

    [Display("Trip Start", "بدء الرحلة")]
    TripStart = 2,

    [Display("Complete End", "إنهاء الرحلة")]
    CompleteEnd = 3
}
