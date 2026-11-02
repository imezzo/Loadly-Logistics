using LoadlyAggregator.Domain.Common.Attributes;
using LoadlyAggregator.Domain.Common.BaseClasses;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Entities;

[NoAudit]
public class TripBookingRequest : BaseEntity
{
    public long TripId { get; set; }
    public long ContractorId { get; set; }
    public long? TripBookingInvitationRoundId { get; set; }
    public TripBookingRequestStatus Status { get; set; } = TripBookingRequestStatus.Pending;
    public DateTime? ReviewedAt { get; set; }
    public long? ReviewedByAdminId { get; set; }

    public virtual Trip? Trip { get; set; }
    public virtual Contractor? Contractor { get; set; }
    public virtual Admin? ReviewedByAdmin { get; set; }
    public virtual TripBookingInvitationRound? TripBookingInvitationRound { get; set; }
}
