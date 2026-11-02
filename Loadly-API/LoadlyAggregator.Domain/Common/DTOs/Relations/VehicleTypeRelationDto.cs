using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.DTOs.Relations;

/// <summary>Master vehicle type reference.</summary>
public class VehicleTypeRelationDto
{
    public long Id { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public VehicleTypeCategory Category { get; set; }
    public VehicleUnitType? VehicleUnitType { get; set; }
}
