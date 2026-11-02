using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Models;

namespace LoadlyAggregator.Domain.Admin.DTOs.Vehicles;

public class UpdateVehicleDto
{
    public long Id { get; set; }
    public string PlateNo { get; set; } = string.Empty;
    public string PlateLetters { get; set; } = string.Empty;
    public int? MakeYear { get; set; }
    public long? InsuranceId { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public decimal? InitialOdometerReading { get; set; }
    public string? VinNo { get; set; }
    public long? ColorId { get; set; }
    public long? CapacityUnitId { get; set; }
    public decimal? TankCapacity { get; set; }
    public string? VehicleTypeSettings { get; set; }
    public decimal? Capacity { get; set; }
    public long? FuelTypeId { get; set; }
    public string? GovernorateCode { get; set; }
    public string? InsuranceDetails { get; set; }
    public string? TemperatureRangeSettng { get; set; }
    public string? CoolingUnitSpecification { get; set; }
    public string? TemperatureMonotoringCapability { get; set; }
    public double? MaximumLoadHeight { get; set; }
    public double? InternalHeight { get; set; }
    public double? InternalWidth { get; set; }
    public double? InternalLength { get; set; }
    public long? BrandModelID { get; set; }
    public long? ContractorId { get; set; }
    public long? SectorId { get; set; }
    public long VehicleTypeId { get; set; }
    public long? HeadVehicleId { get; set; }
    public List<long> TailVehicleIds { get; set; } = new List<long>() { };
    public LoadingMechanismType? LoadingMechanismType { get; set; }
    public List<UpdateVehicleDocumentItemDto>? Documents { get; set; }
    public int? Status { get; set; }
}

public class UpdateVehicleDocumentItemDto
{
    public long? Id { get; set; }
    public long DocumentId { get; set; }
    public FileUpload? File { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int Status { get; set; }
}
