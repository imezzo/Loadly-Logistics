using LoadlyAggregator.Domain.Common.BaseClasses;

namespace LoadlyAggregator.Domain.Common.Entities;

public class TripContractDestination : BaseEntity
{
    public long TripId { get; set; }
    public long DestinationId { get; set; }
    public long? ReturnDestinationId { get; set; }
    public decimal OutboundDistanceKm { get; set; }
    public decimal? ReturnDistanceKm { get; set; }
    public long AddedBy { get; set; }

    public virtual Trip Trip { get; set; } = null!;
    public virtual ContractDestination Destination { get; set; } = null!;
    public virtual ContractDestination? ReturnDestination { get; set; }
    public virtual Admin AddedByAdmin { get; set; } = null!;
}
