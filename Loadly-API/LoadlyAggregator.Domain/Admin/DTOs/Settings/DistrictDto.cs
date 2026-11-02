namespace LoadlyAggregator.Domain.Admin.DTOs.Settings;

public class DistrictDto
{
    public long Id { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public long CityId { get; set; }
    public CityDto? City { get; set; }
    public long? AddedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
