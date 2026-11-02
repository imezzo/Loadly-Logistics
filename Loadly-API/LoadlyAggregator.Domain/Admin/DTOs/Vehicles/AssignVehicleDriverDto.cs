namespace LoadlyAggregator.Domain.Admin.DTOs.Vehicles;

public class AssignVehicleDriverDto
{
    public long VehicleId { get; set; }
    public long? DriverId { get; set; }
}
