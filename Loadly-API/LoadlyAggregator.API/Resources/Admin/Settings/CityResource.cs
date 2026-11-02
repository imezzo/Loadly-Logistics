using LoadlyAggregator.API.Resources.RelationResources;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;

namespace LoadlyAggregator.API.Resources.Admin.Settings;

/// <summary>
/// Resource for City API responses (both listing and single get)
/// </summary>
public class CityResource
{
    public long Id { get; set; }
    public long GovernerateId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public string? NameEn { get; set; }
    public GovernerateRelationResource? Governerate { get; set; }

    public static CityResource FromDto(CityDto dto, GovernerateDto? governerate = null)
    {
        var governerateDto = governerate ?? dto.Governerate;

        return new CityResource
        {
            Id = dto.Id,
            GovernerateId = dto.GovernerateId,
            Name = dto.NameEn,
            NameAr = dto.NameAr,
            NameEn = dto.NameEn,
            Governerate = governerateDto != null
                ? new GovernerateRelationResource
                {
                    Id = governerateDto.Id,
                    Name = governerateDto.NameEn,
                    NameAr = governerateDto.NameAr,
                    NameEn = governerateDto.NameEn
                }
                : null
        };
    }
}
