namespace LoadlyAggregator.Domain.Collector.DTOs.Trips;

public class BulkConfirmTripsDocumentsDto
{
    public List<long> TripIds { get; set; } = new();
    public long FinanceCollectorId { get; set; }
}
