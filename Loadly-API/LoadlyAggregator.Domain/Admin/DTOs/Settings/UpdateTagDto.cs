namespace LoadlyAggregator.Domain.Admin.DTOs.Settings;

public class UpdateTagDto
{
    public long Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public bool IsPricing { get; set; }
}
