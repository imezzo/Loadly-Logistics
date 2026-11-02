using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum WaitingTimeFeeCalculation
{
    [Display("Prorated by minutes", "احتساب بالدقائق الفعليه")]
    ProratedMinutes = 1,
    [Display("Round up to full hour", "تقريب لأقرب ساعة")]
    RoundToHour = 2,
    [Display("15-minute blocks", "كل 15 دقيقة")]
    Round15MinBlock = 3,
    [Display("Free waiting period (minutes)", "فترة سماح (بالدقائق)")]
    FreeWaitingPeriodMinutes = 4,
    [Display("Extra fees for waiting", "رسوم انتظار إضافية")]
    ExtraFeesForWaiting = 5
}
