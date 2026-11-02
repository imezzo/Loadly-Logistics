using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.API.Resources.Admin.Settings;

/// <summary>
/// Resource for Discount API responses (both listing and single get)
/// </summary>
public class DiscountResource
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public int Type { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public int Status { get; set; }
    public string StatusName { get; set; } = string.Empty;

    public static DiscountResource FromDto(DiscountDto dto) =>
        new DiscountResource
        {
            Id = dto.Id,
            Name = dto.NameEn,
            NameAr = dto.NameAr,
            NameEn = dto.NameEn,
            Type = (int)dto.Type,
            TypeName = dto.Type.ToLocalizedDisplay(),
            Value = dto.Value,
            Status = (int)dto.Status,
            StatusName = dto.Status.ToLocalizedDisplay()
        };
}
