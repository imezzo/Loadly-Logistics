using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Contractor.DTOs.Vehicles;

public class ContractorListVehiclesDto : BasePagedRequest
{
    public long ContractorId { get; set; }
    public long? VehicleTypeId { get; set; }
    public List<long>? VehicleTypeIds { get; set; }
    public VehicleUnitType? VehicleUnitType { get; set; }
    public List<VehicleTypeCategory>? VehicleTypeCategories { get; set; }
    public int? Status { get; set; }
    public bool? IsAvailableTail { get; set; }
    public long? DriverId { get; set; }
    public bool? HasDriver { get; set; }
}
