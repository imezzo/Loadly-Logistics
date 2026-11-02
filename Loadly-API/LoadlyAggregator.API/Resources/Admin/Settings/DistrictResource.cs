using LoadlyAggregator.API.Resources.RelationResources;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;

namespace LoadlyAggregator.API.Resources.Admin.Settings;

/// <summary>
/// Resource for District API responses (both listing and single get)
/// </summary>
public class DistrictResource
{
    public long Id { get; set; }
    public long CityId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public string? NameEn { get; set; }
    public CityRelationResource? City { get; set; }

    public static DistrictResource FromDto(DistrictDto dto) =>
        new DistrictResource
        {
            Id = dto.Id,
            CityId = dto.CityId,
            Name = dto.NameEn,
            NameAr = dto.NameAr,
            NameEn = dto.NameEn,
            City = dto.City != null
                ? new CityRelationResource
                {
                    Id = dto.City.Id,
                    Name = dto.City.NameEn,
                    NameAr = dto.City.NameAr,
                    NameEn = dto.City.NameEn
                }
                : null
        };
}
