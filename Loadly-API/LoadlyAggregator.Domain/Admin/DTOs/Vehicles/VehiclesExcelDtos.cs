using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Admin.DTOs.Vehicles;

public sealed class VehiclesExcelFileDto
{
    public required byte[] Content { get; init; }
    public required string FileName { get; init; }
}

public sealed class ExportVehiclesExcelDto
{
    public string? Search { get; init; }
    public long? ContractId { get; init; }
    public bool? IsAssignedToContract { get; init; }
    public long? ColorId { get; init; }
    public long? CapacityUnitId { get; init; }
    public long? VehicleTypeId { get; init; }
    public List<long>? VehicleTypeIds { get; init; }
    public bool PickVehiclesOrTheirHeads { get; init; }
    public VehicleUnitType? VehicleUnitType { get; init; }
    public List<VehicleTypeCategory>? VehicleTypeCategories { get; init; }
    public int? Status { get; init; }
    public bool? IsAvailableTail { get; init; }
    public long? DriverId { get; init; }
    public List<long>? ContractorIds { get; init; }
    public List<long>? SectorIds { get; init; }
    public List<long>? CompanyIds { get; init; }
    public List<long>? FacilityIds { get; init; }
    public List<VehicleFacilityType>? FacilityTypes { get; init; }
    public bool? HasContractor { get; init; }
    public bool? HasDriver { get; init; }
    public DateTime? TripsStartDateFrom { get; init; }
    public DateTime? TripsEndDateTo { get; init; }
    public bool? OrderByCompletedTripsDescending { get; init; }
    public List<string>? Columns { get; init; }
}
