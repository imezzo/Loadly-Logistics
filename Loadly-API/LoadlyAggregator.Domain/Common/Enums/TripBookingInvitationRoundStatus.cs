using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum TripBookingInvitationRoundStatus
{
    [Display("Active", "نشط")]
    Active = 0,

    [Display("Completed", "مكتمل")]
    Completed = 1
}
