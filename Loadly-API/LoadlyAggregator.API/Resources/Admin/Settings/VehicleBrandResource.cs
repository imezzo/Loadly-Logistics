using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;

namespace LoadlyAggregator.API.Resources.Admin.Settings;

/// <summary>
/// Resource for VehicleBrand API responses (both listing and single get)
/// </summary>
public class VehicleBrandResource
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? Logo { get; set; }
    public int Status { get; set; }

    public static VehicleBrandResource FromDto(VehicleBrandDto dto) =>
        new VehicleBrandResource
        {
            Id = dto.Id,
            Name = dto.NameEn,
            NameAr = dto.NameAr,
            NameEn = dto.NameEn,
            Logo = dto.Logo,
            Status = dto.Status
        };
}
