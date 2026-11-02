using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum ContractSlotDayDisposition
{
    [Display("Not Active", "غير نشط")]
    NotActive = 0,

    [Display("Out of Service", "خارج الخدمة")]
    Skipped = 1,

    [Display("Working", "يوم عمل")]
    Working = 2,

    [Display("Unknown", "غير معروف")]
    Unknown = 3,

    /// <summary>Included contracts only: billable at 50% of base or extra working-day rate; shares index sequence with full working days.</summary>
    [Display("Half Working Day", "نصف يوم عمل")]
    HalfWorking = 4
}
