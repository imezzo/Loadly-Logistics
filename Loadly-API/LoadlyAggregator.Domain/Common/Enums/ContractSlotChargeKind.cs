using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum ContractSlotChargeKind
{
    [Display("Base Working Day", "يوم عمل أساسي")]
    BaseWorkingDay = 1,

    [Display("Extra Working Day", "يوم عمل إضافي")]
    ExtraWorkingDay = 2,

    [Display("Extra KM Overage", "تجاوز الكيلومترات")]
    ExtraKmOverage = 3,

    [Display("Extra Trip", "رحلة إضافية")]
    ExtraTrip = 4
}
