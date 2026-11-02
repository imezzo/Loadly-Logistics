using LoadlyAggregator.Domain.Admin.DTOs.Settings;

namespace LoadlyAggregator.API.Resources.Admin.Settings;

public class ShippingLineResource
{
    public long Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public int Status { get; set; }
    public long? AddedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public static ShippingLineResource FromDto(ShippingLineDto dto) => new()
    {
        Id = dto.Id,
        NameAr = dto.NameAr,
        NameEn = dto.NameEn,
        Status = dto.Status,
        AddedBy = dto.AddedBy,
        CreatedAt = dto.CreatedAt,
        UpdatedAt = dto.UpdatedAt
    };
}
