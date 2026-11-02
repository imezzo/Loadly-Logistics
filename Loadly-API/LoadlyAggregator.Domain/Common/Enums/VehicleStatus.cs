using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum VehicleStatus
{
    [Display("Available", "متاح")]
    Available = 1,
    [Display("In Route", "في الطريق")]
    InRoute = 3,
    [Display("Out Of Service", "خارج الخدمة")]
    OutOfService = 4
}
