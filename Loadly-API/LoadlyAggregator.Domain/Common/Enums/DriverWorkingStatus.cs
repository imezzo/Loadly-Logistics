using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum DriverWorkingStatus
{
    [Display("Available", "متاح")]
    Available = 1,
    [Display("Busy", "في رحلة")]
    Busy = 2,
    [Display("On Break", "في استراحة")]
    OnBreak = 3,
    [Display("Offline", "غير متصل")]
    Offline = 4,
    [Display("Off Duty", "خارج الخدمة")]
    OffDuty = 5
}
