using LoadlyAggregator.Domain.Admin.DTOs.Settings;

namespace LoadlyAggregator.API.Resources.Admin.Settings;

/// <summary>
/// Resource for Sector API responses (both listing and single get)
/// </summary>
public class SectorResource
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public string? NameEn { get; set; }

    public static SectorResource FromDto(SectorDto dto) =>
        new SectorResource
        {
            Id = dto.Id,
            Name = dto.NameEn,
            NameAr = dto.NameAr,
            NameEn = dto.NameEn
        };
}
