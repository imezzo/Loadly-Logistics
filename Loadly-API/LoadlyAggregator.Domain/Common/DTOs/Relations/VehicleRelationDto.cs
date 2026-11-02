namespace LoadlyAggregator.Domain.Common.DTOs.Relations;

/// <summary>Vehicle reference (plate, driver, tail vehicles). Used in Contract.</summary>
public class VehicleRelationDto
{
    public long Id { get; set; }
    public long VehicleTypeId { get; set; }
    public string PlateNo { get; set; } = string.Empty;
    public string PlateLetters { get; set; } = string.Empty;
    public string? ChassisNumber { get; set; }
    public string? BrandName { get; set; }
    public string? ModelName { get; set; }
    public int? Year { get; set; }
    public long? SectorId { get; set; }
    public VehicleTypeRelationDto? VehicleType { get; set; }
    public DriverRelationDto? Driver { get; set; }
    public SectorRelationDto? Sector { get; set; }
    public List<CoupledVehicleRelationDto> TailVehicles { get; set; } = new();
}
