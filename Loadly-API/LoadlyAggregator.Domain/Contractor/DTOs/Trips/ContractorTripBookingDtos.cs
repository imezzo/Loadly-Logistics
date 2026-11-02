namespace LoadlyAggregator.Domain.Contractor.DTOs.Trips;

public class ContractorCreateBookingRequestDto
{
    public long TripId { get; set; }
    public long ContractorId { get; set; }
}

public class ContractorWithdrawTripBookingDto
{
    public long TripId { get; set; }
    public long ContractorId { get; set; }
}
