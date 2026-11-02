namespace LoadlyAggregator.Domain.Admin.DTOs.Vehicles;

public class VehicleCapacityUnitDto
{
    public long Id { get; set; }
    public string CapacityTypeEn { get; set; } = string.Empty;
    public string CapacityTypeAr { get; set; } = string.Empty;
    public int Status { get; set; }
    public long? AddedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
