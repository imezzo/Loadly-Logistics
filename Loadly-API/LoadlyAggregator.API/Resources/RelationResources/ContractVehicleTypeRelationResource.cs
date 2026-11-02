namespace LoadlyAggregator.API.Resources.RelationResources;

/// <summary>Contract vehicle type reference (e.g. in contract, trip).</summary>
public class ContractVehicleTypeRelationResource
{
    public long Id { get; set; }
    public long ContractId { get; set; }
    public long VehicleTypeId { get; set; }
    public decimal? ConsumptionRate { get; set; }
    public int? TripsPerMonth { get; set; }
    public int? TripsPerDay { get; set; }
    public decimal? MaxKms { get; set; }
    public decimal? MonthlyRental { get; set; }
    public decimal? DailyValue { get; set; }
    public decimal? OverConsumption { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal? MinBillableWeightTons { get; set; }
    public decimal? MaxPayloadTons { get; set; }
    public decimal? OverWeightPricePerTon { get; set; }
    public decimal? ContractorOverWeightPricePerTon { get; set; }
    public decimal? ExtraWorkingDayFees { get; set; }
    public long? AddedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public VehicleTypeRelationResource? VehicleType { get; set; }
}
