using LoadlyAggregator.Domain.Common.BaseClasses;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Entities;

public class Vehicle : BaseEntity
{
    public long VehicleTypeId { get; set; }
    public long? HeadVehicleId { get; set; }
    public string PlateNo { get; set; } = string.Empty;
    public string PlateLetters { get; set; } = string.Empty;
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
    public long? ContractId { get; set; }
    public long? ContractorId { get; set; }
    public long? SectorId { get; set; }
    public long? BrandModelID { get; set; }
    public decimal? TankCapacity { get; set; }
    public decimal? Capacity { get; set; }
    public long? FuelTypeId { get; set; }
    public string? VehicleTypeSettings { get; set; }
    public long? AddedBy { get; set; }
    public VehicleStatus Status { get; set; } = VehicleStatus.Available;

    // Navigation properties
    public virtual VehicleInsurance? Insurance { get; set; }

    public virtual Color? Color { get; set; }

    public virtual VehicleCapacityUnit? CapacityUnit { get; set; }

    public virtual Driver? Driver { get; set; }

    public virtual VehicleFacility? CurrentServiceFacility { get; set; }

    public virtual VehicleFacility? AssignedParkingFacility { get; set; }

    public virtual VehicleType VehicleType { get; set; } = null!;

    public virtual VehicleBrandModel? VehicleBrandModel { get; set; }

    public virtual Contract? Contract { get; set; }
    public virtual Contractor? Contractor { get; set; }
    public virtual Sector? Sector { get; set; }
    public virtual Vehicle? HeadVehicle { get; set; }

    public virtual ICollection<Vehicle> TailVehicles { get; set; }
        = new List<Vehicle>();

    public virtual VehicleDetail? VehicleDetail { get; set; }

    public virtual FuelType? FuelType { get; set; }

    public virtual ICollection<VehicleDocument> VehicleDocuments { get; set; }
        = new List<VehicleDocument>();

    public virtual ICollection<Trip> Trips { get; set; }
        = new List<Trip>();

    public virtual ICollection<VehicleFacilityVisit> VehicleFacilityVisits { get; set; }
        = new List<VehicleFacilityVisit>();

    public virtual ICollection<VehiclePlateAlias> PlateAliases { get; set; }
        = new List<VehiclePlateAlias>();
}
