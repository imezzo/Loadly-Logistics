namespace LoadlyAggregator.Domain.Admin.DTOs.Trips;

public class CancelTripDto
{
    public long Id { get; set; }
    public string? CancellationReason { get; set; }
}
