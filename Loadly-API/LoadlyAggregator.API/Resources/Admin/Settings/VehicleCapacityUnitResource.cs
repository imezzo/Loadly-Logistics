using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;

namespace LoadlyAggregator.API.Resources.Admin.Settings;

/// <summary>
/// Resource for VehicleCapacityUnit API responses (both listing and single get)
/// </summary>
public class VehicleCapacityUnitResource
{
    public long Id { get; set; }
    public string CapacityType { get; set; } = string.Empty;
    public string CapacityTypeAr { get; set; } = string.Empty;
    public string CapacityTypeEn { get; set; } = string.Empty;
    public int Status { get; set; }

    public static VehicleCapacityUnitResource FromDto(VehicleCapacityUnitDto dto) =>
        new VehicleCapacityUnitResource
        {
            Id = dto.Id,
            CapacityType = dto.CapacityTypeEn,
            CapacityTypeAr = dto.CapacityTypeAr,
            CapacityTypeEn = dto.CapacityTypeEn,
            Status = dto.Status
        };
}
