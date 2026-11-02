using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Admin.DTOs.Vehicles;

public class ListVehicleTypesDto : BasePagedRequest
{
    public VehicleTypeCategory? Category { get; set; }
    public List<VehicleUnitType>? VehicleUnitTypes { get; set; }
    public VehicleTypeStatus? Status { get; set; }
}
