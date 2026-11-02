namespace LoadlyAggregator.Domain.Contractor.DTOs.Trips;

public class ContractorTripPayoutStatisticsDto
{
    public int CompletedPaidTrips { get; set; }
    public int CompletedUnpaidTrips { get; set; }
    public decimal CompletedPaidTripsTotalAmount { get; set; }
    public decimal CompletedUnpaidTripsTotalAmount { get; set; }
}
