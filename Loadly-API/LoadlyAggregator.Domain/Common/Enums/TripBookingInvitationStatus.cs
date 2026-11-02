using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum TripBookingInvitationStatus
{
    [Display("Pending", "قيد الانتظار")]
    Pending = 0,

    [Display("Accepted", "مقبول")]
    Accepted = 1,

    [Display("Cancelled", "ملغى")]
    Cancelled = 2
}
