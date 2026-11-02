using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Admin.DTOs.Vehicles;

public class UpdateVehicleTypeDto
{
    public long Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public VehicleTypeCategory Category { get; set; }
    public VehicleUnitType? VehicleUnitType { get; set; }
    public VehicleTypeStatus Status { get; set; }
    public List<long> TagIds { get; set; } = [];
}

