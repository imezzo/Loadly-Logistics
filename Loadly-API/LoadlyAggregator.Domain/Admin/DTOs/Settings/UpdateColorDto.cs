namespace LoadlyAggregator.Domain.Admin.DTOs.Settings;

public class UpdateColorDto
{
    public long Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string ColorHash { get; set; } = string.Empty;
}
