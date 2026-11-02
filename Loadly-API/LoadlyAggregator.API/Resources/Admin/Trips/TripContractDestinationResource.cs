using LoadlyAggregator.API.Resources.RelationResources;

namespace LoadlyAggregator.API.Resources.Admin.Trips;

public class TripContractDestinationResource
{
    public long Id { get; set; }
    public long DestinationId { get; set; }
    public long? ReturnDestinationId { get; set; }
    public decimal OutboundDistanceKm { get; set; }
    public decimal? ReturnDistanceKm { get; set; }
    public long AddedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DestinationRelationResource Destination { get; set; } = null!;
    public DestinationRelationResource? ReturnDestination { get; set; }
}
