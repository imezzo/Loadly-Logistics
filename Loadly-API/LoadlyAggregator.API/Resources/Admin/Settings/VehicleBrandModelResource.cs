using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;

namespace LoadlyAggregator.API.Resources.Admin.Settings;

/// <summary>
/// Resource for VehicleBrandModel API responses (both listing and single get)
/// </summary>
public class VehicleBrandModelResource
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public long VehicleBrandId { get; set; }
    public int Status { get; set; }
    public VehicleBrandRelationResource? VehicleBrand { get; set; }

    public static VehicleBrandModelResource FromDto(VehicleBrandModelDto dto, VehicleBrandDto? vehicleBrand = null) =>
        new VehicleBrandModelResource
        {
            Id = dto.Id,
            Name = dto.NameEn,
            NameAr = dto.NameAr,
            NameEn = dto.NameEn,
            Status = dto.Status,
            VehicleBrandId = dto.VehicleBrandId,
            VehicleBrand = vehicleBrand != null ? new VehicleBrandRelationResource
            {
                Id = vehicleBrand.Id,
                Name = vehicleBrand.NameEn,
                NameAr = vehicleBrand.NameAr,
                NameEn = vehicleBrand.NameEn,
                Logo = vehicleBrand.Logo,
                Status = vehicleBrand.Status
            } : null
        };
}

public class VehicleBrandRelationResource
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public string? NameAr { get; set; }
    public string? NameEn { get; set; }
    public string? Logo { get; set; }
    public int Status { get; set; }
}
