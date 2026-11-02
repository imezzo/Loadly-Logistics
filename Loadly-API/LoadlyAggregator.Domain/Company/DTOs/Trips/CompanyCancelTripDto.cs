namespace LoadlyAggregator.Domain.Company.DTOs.Trips;

public class CompanyCancelTripDto
{
    public long TripId { get; set; }
    public long CompanyId { get; set; }
    public string? Reason { get; set; }
}
