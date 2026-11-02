namespace LoadlyAggregator.Domain.Admin.DTOs.Settings;

public class CityDto
{
    public long Id { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public long GovernerateId { get; set; }
    public GovernerateDto? Governerate { get; set; }
    public long? AddedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
