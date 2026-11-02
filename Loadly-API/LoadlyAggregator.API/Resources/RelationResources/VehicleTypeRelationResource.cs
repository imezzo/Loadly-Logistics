using LoadlyAggregator.Domain.Common.DTOs.Relations;

namespace LoadlyAggregator.API.Resources.RelationResources;

/// <summary>Master vehicle type reference.</summary>
public class VehicleTypeRelationResource
{
    public long Id { get; set; }

    public string? Name { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public int Category { get; set; }
    public int? VehicleUnitType { get; set; }

    public static VehicleTypeRelationResource? FromDto(VehicleTypeRelationDto? dto) =>
        dto == null ? null : new VehicleTypeRelationResource
        {
            Id = dto.Id,
            Name = dto.NameAr,
            NameEn = dto.NameEn,
            NameAr = dto.NameAr,
            Category = (int)dto.Category,
            VehicleUnitType = dto.VehicleUnitType.HasValue ? (int)dto.VehicleUnitType.Value : null,
        };
}
