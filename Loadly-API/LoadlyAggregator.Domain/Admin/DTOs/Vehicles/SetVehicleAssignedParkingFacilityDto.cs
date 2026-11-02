namespace LoadlyAggregator.Domain.Admin.DTOs.Vehicles;

public class SetVehicleAssignedParkingFacilityDto
{
    public long VehicleId { get; set; }
    public long? AssignedParkingFacilityId { get; set; }
}
