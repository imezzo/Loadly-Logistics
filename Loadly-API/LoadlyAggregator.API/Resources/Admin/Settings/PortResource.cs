using LoadlyAggregator.Domain.Admin.DTOs.Settings;

namespace LoadlyAggregator.API.Resources.Admin.Settings;

public class PortResource
{
    public long Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public string Country { get; set; } = string.Empty;
    public string? PortCode { get; set; }
    public int Type { get; set; }
    public int Status { get; set; }
    public long? AddedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public static PortResource FromDto(PortDto dto) => new()
    {
        Id = dto.Id,
        NameAr = dto.NameAr,
        NameEn = dto.NameEn,
        Country = dto.Country,
        PortCode = dto.PortCode,
        Type = dto.Type,
        Status = dto.Status,
        AddedBy = dto.AddedBy,
        CreatedAt = dto.CreatedAt,
        UpdatedAt = dto.UpdatedAt
    };
}
