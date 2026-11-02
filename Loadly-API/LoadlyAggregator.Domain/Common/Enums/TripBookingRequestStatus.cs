using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum TripBookingRequestStatus
{
    [Display("Awaiting dispatcher approval", "في انتظار موافقة المشرف")]
    Pending = 0,
    [Display("Approved", "موافق عليه")]
    Approved = 1,
    [Display("Rejected", "مرفوض")]
    Rejected = 2,

    [Display("Withdrawn", "مسحوب")]
    Withdrawn = 3
}
