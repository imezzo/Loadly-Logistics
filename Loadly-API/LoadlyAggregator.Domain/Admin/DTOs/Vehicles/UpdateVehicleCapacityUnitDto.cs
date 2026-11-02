namespace LoadlyAggregator.Domain.Admin.DTOs.Vehicles;

public class UpdateVehicleCapacityUnitDto
{
    public long Id { get; set; }
    public string CapacityTypeEn { get; set; } = string.Empty;
    public string CapacityTypeAr { get; set; } = string.Empty;
}
