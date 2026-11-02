namespace LoadlyAggregator.Domain.Common.DTOs.Relations;

/// <summary>Vehicle in a head/tail coupling (head, tail, or rigid).</summary>
public class CoupledVehicleRelationDto
{
    public long Id { get; set; }
    public long VehicleTypeId { get; set; }
    public string PlateNo { get; set; } = string.Empty;
    public string PlateLetters { get; set; } = string.Empty;
    public string? ChassisNumber { get; set; }
    public int? Year { get; set; }
    public string? BrandName { get; set; }
    public string? ModelName { get; set; }
    public long? DriverId { get; set; }
    public int Status { get; set; }
    public VehicleTypeRelationDto? VehicleType { get; set; }
}
