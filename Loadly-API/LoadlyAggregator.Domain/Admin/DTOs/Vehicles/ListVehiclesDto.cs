using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Admin.DTOs.Vehicles;

public class ListVehiclesDto : BasePagedRequest
{
    public string? Search { get; set; }
    public long? DriverId { get; set; }
    public List<long>? ContractorIds { get; set; }
    public List<long>? SectorIds { get; set; }
    public List<long>? CompanyIds { get; set; }
    public List<long>? FacilityIds { get; set; }
    public List<VehicleFacilityType>? FacilityTypes { get; set; }
    public bool? HasContractor { get; set; }
    public long? ContractId { get; set; }
    public long? ColorId { get; set; }
    public long? CapacityUnitId { get; set; }
    public long? VehicleTypeId { get; set; }
    public List<long>? VehicleTypeIds { get; set; }
    public int? Status { get; set; }
    public bool? IsAssignedToContract { get; set; }
    public bool PickVehiclesOrTheirHeads { get; set; }
    public bool? IsAvailableTail { get; set; }
    public VehicleUnitType? VehicleUnitType { get; set; }
    public List<VehicleTypeCategory>? VehicleTypeCategories { get; set; }
    public bool? HasDriver { get; set; }
    public DateTime? TripsStartDateFrom { get; set; }
    public DateTime? TripsEndDateTo { get; set; }
    public bool? OrderByCompletedTripsDescending { get; set; }
    public bool? IsWaitingForLoading { get; set; }
}
