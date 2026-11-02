namespace LoadlyAggregator.Domain.Admin.DTOs.Trips;

public class TrackTripsDto
{
    public long? CompanyId { get; set; }
    public string? ContractReferenceNumber { get; set; }
    public long? DriverId { get; set; }
    public long? VehicleId { get; set; }
}
