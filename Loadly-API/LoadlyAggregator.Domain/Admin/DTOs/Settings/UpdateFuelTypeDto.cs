namespace LoadlyAggregator.Domain.Admin.DTOs.Settings;

public class UpdateFuelTypeDto
{
    public long Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
}
