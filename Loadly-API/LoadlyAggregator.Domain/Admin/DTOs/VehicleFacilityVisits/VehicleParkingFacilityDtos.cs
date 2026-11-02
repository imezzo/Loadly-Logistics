namespace LoadlyAggregator.Domain.Admin.DTOs.VehicleFacilityVisits;

public class EnterVehicleParkingFacilityDto
{
    public long VehicleId { get; set; }
    public long VehicleFacilityId { get; set; }
    public DateTime EventDate { get; set; }
    public string? Reason { get; set; }
}

public class ExitVehicleParkingFacilityDto
{
    public long VehicleId { get; set; }
    public long? VehicleFacilityId { get; set; }
    public DateTime EventDate { get; set; }
    public string? Reason { get; set; }
}
