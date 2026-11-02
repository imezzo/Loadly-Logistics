using LoadlyAggregator.Domain.Common.DTOs.Relations;

namespace LoadlyAggregator.Domain.Admin.DTOs.Trips;

public class TripContractDestinationDto
{
    public long Id { get; set; }
    public long DestinationId { get; set; }
    public long? ReturnDestinationId { get; set; }
    public decimal OutboundDistanceKm { get; set; }
    public decimal? ReturnDistanceKm { get; set; }
    public long AddedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DestinationRelationDto Destination { get; set; } = null!;
    public DestinationRelationDto? ReturnDestination { get; set; }
}
