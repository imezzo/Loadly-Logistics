namespace LoadlyAggregator.Domain.Admin.DTOs.Settings;

public class UpdateSectorDto
{
    public long Id { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }
}
