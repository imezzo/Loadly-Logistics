namespace LoadlyAggregator.Domain.Admin.DTOs.Vehicles;

public class UpdateVehicleBrandDto
{
    public long Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? Logo { get; set; }
}
