namespace LoadlyAggregator.Domain.Collector.DTOs.Trips;

public class BulkCollectTripsDocumentsDto
{
    public List<long> TripIds { get; set; } = new();
    public long CollectorId { get; set; }
}
