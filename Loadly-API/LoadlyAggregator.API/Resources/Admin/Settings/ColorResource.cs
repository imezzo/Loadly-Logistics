using LoadlyAggregator.Domain.Admin.DTOs.Settings;

namespace LoadlyAggregator.API.Resources.Admin.Settings;

/// <summary>
/// Resource for Color API responses (both listing and single get)
/// </summary>
public class ColorResource
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string ColorHash { get; set; } = string.Empty;
    public int Status { get; set; }

    public static ColorResource FromDto(ColorDto dto) =>
        new ColorResource
        {
            Id = dto.Id,
            Name = dto.NameEn,
            NameAr = dto.NameAr,
            NameEn = dto.NameEn,
            ColorHash = dto.ColorHash,
            Status = dto.Status
        };
}
