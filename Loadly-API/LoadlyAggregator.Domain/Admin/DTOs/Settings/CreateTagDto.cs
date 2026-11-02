namespace LoadlyAggregator.Domain.Admin.DTOs.Settings;

public class CreateTagDto
{
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public bool IsPricing { get; set; } = false;
}
