using LoadlyAggregator.Domain.Contractor.DTOs.Drivers;

namespace LoadlyAggregator.Domain.Contractor.DTOs.Trips;

public class ContractorChangeTripDriverDto
{
    public long TripId { get; set; }
    public long ContractorId { get; set; }
    public long? DriverId { get; set; }
    public ContractorCreateDriverDto? Driver { get; set; }
    public string? Note { get; set; }
}
