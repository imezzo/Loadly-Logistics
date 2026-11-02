using LoadlyAggregator.Domain.Common.DTOs.Relations;

namespace LoadlyAggregator.Domain.Admin.DTOs.Contracts;

/// <summary>Contract vehicle type for creation (no Id, ContractId, audit fields).</summary>
public class CreateContractVehicleTypeDto
{
    public long VehicleTypeId { get; set; }
    public decimal? ConsumptionRate { get; set; }
    public int? TripsPerMonth { get; set; }
    public decimal? MaxKms { get; set; }
    public decimal? MonthlyRental { get; set; }
    public decimal? OverConsumption { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal? MinBillableWeightTons { get; set; }
    public decimal? MaxPayloadTons { get; set; }
    public decimal? OverWeightPricePerTon { get; set; }
    public decimal? ContractorOverWeightPricePerTon { get; set; }
    public decimal? ExtraWorkingDayFees { get; set; }
}

public class ContractVehicleTypeDto
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
    public int Quantity { get; set; }
    public decimal? MinBillableWeightTons { get; set; }
    public decimal? MaxPayloadTons { get; set; }
    public decimal? OverWeightPricePerTon { get; set; }
    public decimal? ContractorOverWeightPricePerTon { get; set; }
    public decimal? ExtraWorkingDayFees { get; set; }
    public long? AddedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ContractDto? Contract { get; set; }
    public VehicleTypeRelationDto? VehicleType { get; set; }
}
