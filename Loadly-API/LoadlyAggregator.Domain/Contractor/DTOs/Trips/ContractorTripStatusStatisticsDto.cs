namespace LoadlyAggregator.Domain.Contractor.DTOs.Trips;

public class ContractorTripStatusStatisticsDto
{
    public int CompletedTrips { get; set; }
    public int ScheduledTrips { get; set; }
    public int InProgressTrips { get; set; }
    public int PendingTrips { get; set; }
}
