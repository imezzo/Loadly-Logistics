namespace LoadlyAggregator.Domain.Admin.DTOs.Settings;

public class CreateCityDto
{
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public long GovernerateId { get; set; }
}
