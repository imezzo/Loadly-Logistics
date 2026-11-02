using LoadlyAggregator.Domain.Common.Attributes;
using LoadlyAggregator.Domain.Common.BaseClasses;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Entities;

[NoAudit]
public class TripBookingInvitationRound : BaseEntity
{
    public long TripId { get; set; }
    public long? ContractorId { get; set; }
    public TripBookingInvitationRoundStatus Status { get; set; } = TripBookingInvitationRoundStatus.Active;
    public long? AddedBy { get; set; }

    public virtual Trip? Trip { get; set; }
    public virtual Contractor? Contractor { get; set; }
    public virtual ICollection<TripBookingInvitation> BookingInvitations { get; set; } = new List<TripBookingInvitation>();
    public virtual ICollection<TripBookingRequest> BookingRequests { get; set; } = new List<TripBookingRequest>();
}
