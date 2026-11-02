using LoadlyAggregator.Domain.Admin.DTOs.Settings;

namespace LoadlyAggregator.API.Resources.Admin.Settings;

/// <summary>
/// Resource for FuelType API responses (both listing and single get)
/// </summary>
public class FuelTypeResource
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }

    public static FuelTypeResource FromDto(FuelTypeDto dto) =>
        new FuelTypeResource
        {
            Id = dto.Id,
            Name = dto.NameEn,
            NameAr = dto.NameAr,
            NameEn = dto.NameEn,
            UnitPrice = dto.UnitPrice
        };
}
