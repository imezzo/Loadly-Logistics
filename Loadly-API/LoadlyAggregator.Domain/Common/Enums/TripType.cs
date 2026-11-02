using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum TripType
{
    [Display("One way", "ذهاب فقط")]
    OneWay = 1,

    [Display("Round trip", "ذهاب وعودة لنقطة البداية")]
    RoundTrip = 2,

    [Display("Open jaw", "عودة بموقع مختلف")]
    OpenJaw = 3
}
