using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;

namespace LoadlyAggregator.Domain.Admin.DTOs.Vehicles;

public class VehicleTypeDto
{
    public long Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public VehicleTypeCategory Category { get; set; }
    public VehicleUnitType? VehicleUnitType { get; set; }
    public VehicleTypeStatus Status { get; set; }
    public List<TagDto> Tags { get; set; } = [];
}

