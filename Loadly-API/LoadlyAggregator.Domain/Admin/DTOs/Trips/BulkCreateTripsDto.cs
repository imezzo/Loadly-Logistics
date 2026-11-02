namespace LoadlyAggregator.Domain.Admin.DTOs.Trips;

public class BulkCreateTripsDto
{
    public long ContractId { get; set; }
    public List<BulkTripItemDto> Trips { get; set; } = new();
}

public class BulkTripItemDto
{
    public long VehicleTypeId { get; set; }
    public long? DestinationId { get; set; }
    public long VehicleId { get; set; }  // Head vehicle (required)
    public long? DriverId { get; set; }  // Driver (optional)
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateOnly? FinancialDate { get; set; }
    public decimal? ContractDistanceKm { get; set; }
}
