namespace LoadlyAggregator.Domain.Admin.DTOs.Vehicles;

public class CreateVehicleBrandModelDto
{
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public long VehicleBrandId { get; set; }
}
