namespace LoadlyAggregator.Domain.Admin.DTOs.Contracts;

public class CalculateContractPricingRequestDto
{
    public long ContractId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class CalculateContractPricingDto
{
    public long ContractId { get; set; }
    public decimal ContractBasePrice { get; set; }
    public decimal ActualTotalPrice { get; set; }
    public decimal TotalFuelCost { get; set; }
    public decimal TotalDistance { get; set; }
    public decimal ExtrasTotal { get; set; }
    public PricingSummaryDto Summary { get; set; } = new();
    public List<TripBreakdownDto> TripBreakdowns { get; set; } = new();
    public List<VehicleTypeBreakdownDto> BreakdownByVehicleType { get; set; } = new();
    public List<FuelTypeBreakdownDto> BreakdownByFuelType { get; set; } = new();
    public ExtrasDetailsDto ExtrasDetails { get; set; } = new();
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class TripBreakdownDto
{
    public long TripId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public long VehicleId { get; set; }
    public long VehicleTypeId { get; set; }
    public string VehicleTypeName { get; set; } = string.Empty;
    public decimal? Distance { get; set; }


    public decimal DailyValueCost { get; set; }
    public decimal FuelCost { get; set; }
    public decimal ExtraTripCost { get; set; }
    public decimal ExtraKmCost { get; set; }
    public decimal OverweightPricePerTonCost { get; set; }
    public decimal TotalTripCost { get; set; }


    public bool IsFirstTripOfDay { get; set; }
    public bool IsExtraTrip { get; set; }
}

public class PricingSummaryDto
{
    public decimal BasePrice { get; set; }
    public decimal ActualPrice { get; set; }
    public decimal Extras { get; set; }
    public decimal Difference { get; set; }
}

public class VehicleTypeBreakdownDto
{
    public long VehicleTypeId { get; set; }
    public string VehicleTypeName { get; set; } = string.Empty;
    public int TripsCount { get; set; }
    public int? TripsPerMonth { get; set; }
    public decimal BaseCost { get; set; }
    public decimal OverConsumptionCost { get; set; }
    public decimal OverweightPricePerTonCost { get; set; }
    public decimal TotalCost { get; set; }
}

public class FuelTypeBreakdownDto
{
    public long FuelTypeId { get; set; }
    public string FuelTypeName { get; set; } = string.Empty;
    public decimal TotalFuelQuantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalCost { get; set; }
}

public class ExtrasDetailsDto
{
    public decimal OverConsumption { get; set; }
    public decimal OverWeightPricePerTon { get; set; }
    public decimal AdditionalFuelCosts { get; set; }
    public decimal OtherExtras { get; set; }
}

