using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;

namespace LoadlyAggregator.API.Resources.Admin.Settings;

public class VehicleTypeResource
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public int Category { get; set; }
    public int? VehicleUnitType { get; set; }
    public int Status { get; set; }
    public List<TagResource> Tags { get; set; } = [];

    public static VehicleTypeResource FromDto(VehicleTypeDto dto) =>
        new VehicleTypeResource
        {
            Id = dto.Id,
            Name = dto.NameEn,
            NameAr = dto.NameAr,
            NameEn = dto.NameEn,
            Category = (int)dto.Category,
            VehicleUnitType = dto.VehicleUnitType.HasValue ? (int)dto.VehicleUnitType.Value : null,
            Status = (int)dto.Status,
            Tags = dto.Tags.Select(TagResource.FromDto).ToList(),
        };
}
