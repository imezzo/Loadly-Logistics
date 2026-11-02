using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.DTOs.Documents;
using LoadlyAggregator.Domain.Common.DTOs.Relations;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Admin.DTOs.Vehicles;

public class VehicleDto
{
    public long Id { get; set; }
    public string PlateNo { get; set; } = string.Empty;
    public string PlateLetters { get; set; } = string.Empty;
    public List<VehiclePlateChangeHistoryDto> PlateChangeHistory { get; set; } = new();
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
    public long? ContractorId { get; set; }
    public long? SectorId { get; set; }
    public long VehicleTypeId { get; set; }
    public decimal? TankCapacity { get; set; }
    public string? VehicleTypeSettings { get; set; }
    public decimal? Capacity { get; set; }
    public long? FuelTypeId { get; set; }
    public FuelTypeDto? FuelType { get; set; }
    public int Status { get; set; }
    public long? AddedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public long? HeadVehicleId { get; set; }
    public List<CoupledVehicleRelationDto> TailVehicles { get; set; } = new();
    public long? BrandModelID { get; set; }
    public VehicleInsuranceDto? Insurance { get; set; }
    public VehicleBrandModelDto? VehicleBrandModel { get; set; }
    public VehicleDetailDto? VehicleDetail { get; set; }
    public ColorDto? Color { get; set; }
    public VehicleTypeRelationDto? VehicleType { get; set; }
    public CoupledVehicleRelationDto? HeadVehicle { get; set; }
    public DriverRelationDto? Driver { get; set; }
    public ContractorRelationDto? Contractor { get; set; }
    public SectorRelationDto? Sector { get; set; }
    public VehicleCapacityUnitDto? VehicleCapacityUnit { get; set; }
    public List<VehicleDocumentDataDto> VehicleDocuments { get; set; } = new();
    public int CompletedTripsCount { get; set; }
    public ContractRelationDto? Contract { get; set; }
    public List<VehicleFacilityVisitOnVehicleRelationDto> VehicleFacilityVisits { get; set; } = new();
    public VehicleFacilityRelationDto? CurrentServiceFacility { get; set; }
    public VehicleFacilityRelationDto? AssignedParkingFacility { get; set; }
    public bool WaitingForLoading => AssignedParkingFacilityId.HasValue;

}

public class VehicleDocumentDataDto
{
    public long Id { get; set; }
    public long DocumentId { get; set; }
    public long VehicleId { get; set; }
    public string? Path { get; set; }
    public string? FileUrl { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DocumentVerificationStatus Status { get; set; }
    public long? AddedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DocumentDto? Document { get; set; }
}
