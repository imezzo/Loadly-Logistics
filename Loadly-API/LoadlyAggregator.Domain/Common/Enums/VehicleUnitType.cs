using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum VehicleUnitType
{
    [Display("Head", "جرار")]
    Head = 1,
    [Display("Tail", "مقطورة")]
    Tail = 2,
    [Display("Rigid", "سيارة مفردة(شاسيه ثابت)")]
    RIGID = 3
}
