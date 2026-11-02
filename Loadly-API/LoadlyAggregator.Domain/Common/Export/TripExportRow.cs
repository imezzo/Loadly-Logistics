using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Export;

public sealed class TripExportRow
{
    public long Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateOnly? FinancialDate { get; set; }
    public TripStatus Status { get; set; }
    public TripDocumentStatus TripDocumentStatus { get; set; }
    public int? NumberOfDays { get; set; }
    public bool IsExternalTrip { get; set; }
    public bool? IsBackfill { get; set; }
    public string? ContactPersonName { get; set; }
    public string? ContactPersonMobile { get; set; }
    public int? HelpersCount { get; set; }
    public decimal? ContractDistanceKm { get; set; }
    public decimal? ActualDistanceKm { get; set; }
    public decimal? ActualCargoWeightTons { get; set; }
    public decimal? CargoPrice { get; set; }
    public decimal? SpotPrice { get; set; }
    public decimal? TotalTripCost { get; set; }
    public decimal? TotalAddonsCost { get; set; }
    public OverNightCostBearer? OverNightCostBearer { get; set; }
    public TripCustomsStatus CustomsStatus { get; set; }
    public decimal? CustomsTotalAmount { get; set; }
    public string? DispatcherNotes { get; set; }
    public string? CompanyNameEn { get; set; }
    public string? CompanyNameAr { get; set; }
    public string? ContractReferenceNumber { get; set; }
    public string? DriverName { get; set; }
    public string? DriverNameAr { get; set; }
    public string? DispatcherName { get; set; }
    public string? HeadPlateNo { get; set; }
    public string? HeadPlateLetters { get; set; }
    public string? VehiclePlateNo { get; set; }
    public string? VehiclePlateLetters { get; set; }
    public bool HasContractVehicleType { get; set; }
    public string? ContractVehicleTypeNameEn { get; set; }
    public string? ContractVehicleTypeNameAr { get; set; }
    public string? VehicleTypeNameEn { get; set; }
    public string? VehicleTypeNameAr { get; set; }
    public decimal? ConsumptionRate { get; set; }
    public string? SectorNameEn { get; set; }
    public string? SectorNameAr { get; set; }
    public string? ContractDestinationNameEn { get; set; }
    public string? ContractDestinationNameAr { get; set; }
    public string? ContractorName { get; set; }
    public string? LeadName { get; set; }
    public int? FuelCalculationOption { get; set; }
    public List<TripCharge>? TripCharges { get; set; }
    public List<TripWaypoint>? Waypoints { get; set; }
    public List<Recording>? Recordings { get; set; }
}
