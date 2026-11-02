using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum TripStatus
{
    [Display("Company Draft", "مسودة رحلة")]
    CompanyDraft = -1,
    [Display("New", "جديدة")]
    Pending = 1,
    [Display("Scheduled", "أبلغنا السائق")]
    Scheduled = 2,
    [Display("In Progress", "فى الطريق")]
    InProgress = 5,
    [Display("Completed", "مكتملة")]
    Completed = 6,
    [Display("Cancelled", "ملغاة")]
    Cancelled = 8
}

