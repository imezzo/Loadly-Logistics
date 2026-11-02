using LoadlyAggregator.API.Resources.Admin.Settings;
using LoadlyAggregator.API.Resources.RelationResources;

namespace LoadlyAggregator.API.Resources.Admin.Vehicles;

/// <summary>
/// Resource for Vehicle API responses (both listing and single get)
/// </summary>
public class VehicleResource
{
    public long Id { get; set; }
    public string PlateNo { get; set; } = string.Empty;
    public string PlateLetters { get; set; } = string.Empty;
    public List<VehiclePlateChangeHistoryResource> PlateChangeHistory { get; set; } = new();
    public int? MakeYear { get; set; }
    public long? InsuranceId { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public decimal? InitialOdometerReading { get; set; }
    public string? VinNo { get; set; }
    public long? ColorId { get; set; }
    public long? CapacityUnitId { get; set; }
    public long? DriverId { get; set; }
    public long? CurrentServiceFacilityId { get; set; }
    public long? AssignedParkingFacilityId { get; set; }
    public long? SectorId { get; set; }
    public long VehicleTypeId { get; set; }
    public decimal? TankCapacity { get; set; }
    public string? VehicleTypeSettings { get; set; }
    public decimal? Capacity { get; set; }
    public long? FuelTypeId { get; set; }
    public FuelTypeResource? FuelType { get; set; }
    public int Status { get; set; }
    public long? AddedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public long? HeadVehicleId { get; set; }
    public long? TailVehicleId { get; set; }
    public long? BrandModelID { get; set; }
    public VehicleInsuranceResource? Insurance { get; set; }
    public VehicleBrandModelResource? VehicleBrandModel { get; set; }
    public ColorResource? Color { get; set; }
    public VehicleTypeRelationResource? VehicleType { get; set; }
    public CoupledVehicleRelationResource? HeadVehicle { get; set; }
    public List<CoupledVehicleRelationResource> TailVehicles { get; set; } = new();
    public VehicleDetailResource? VehicleDetail { get; set; }
    public DriverRelationResource? Driver { get; set; }
    public ContractorRelationResource? Contractor { get; set; }
    public SectorRelationResource? Sector { get; set; }
    public ContractRelationResource? Contract { get; set; }
    public VehicleCapacityUnitResource? VehicleCapacityUnit { get; set; }
    public List<VehicleDocumentRelationResource> VehicleDocuments { get; set; } = new();
    public int CompletedTripsCount { get; set; }
    public VehicleFacilityRelationResource? CurrentServiceFacility { get; set; }
    public VehicleFacilityRelationResource? AssignedParkingFacility { get; set; }
    public bool WaitingForLoading { get; set; }
    public List<VehicleFacilityVisitOnVehicleRelationResource> VehicleFacilityVisits { get; set; } = new();
}

public class VehicleFacilityVisitOnVehicleRelationResource
{
    public long Id { get; set; }
    public int Event { get; set; }
    public string EventName { get; set; } = string.Empty;
    public int Context { get; set; }
    public string ContextName { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public string? Reason { get; set; }
    public DateTime CreatedAt { get; set; }
    public VehicleFacilityRelationResource? VehicleFacility { get; set; }
    public DriverRelationResource? Driver { get; set; }
    public AdminRelationResource? AddedBy { get; set; }
}

public class VehicleDocumentRelationResource
{
    public long Id { get; set; }
    public long DocumentId { get; set; }
    public long VehicleId { get; set; }
    public string? Path { get; set; }
    public string? FileUrl { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int Status { get; set; }
    public long? AddedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DocumentRelationResource? Document { get; set; }
}
