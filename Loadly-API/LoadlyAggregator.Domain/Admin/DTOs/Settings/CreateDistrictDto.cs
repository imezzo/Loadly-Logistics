namespace LoadlyAggregator.Domain.Admin.DTOs.Settings;

public class CreateDistrictDto
{
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public long CityId { get; set; }
}
