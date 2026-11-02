using System.Text.Json.Serialization;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Admin.DTOs.Contracts;

public class CalculateContractDestinationCostsRequestDto
{
    public ContractType ContractType { get; set; }

    /// <summary>Required for Included/Excluded contract types; used to derive daily value / trips-per-day (same as contract create/update).</summary>
    public int? MonthlyWorkingDays { get; set; }

    public List<CalculateContractDestinationItemDto> Destinations { get; set; } = new();
    public List<CalculateContractVehicleTypeInputDto> VehicleTypes { get; set; } = new();
}

public class CalculateContractDestinationItemDto
{
    public decimal? Kms { get; set; }
    public int? NumberOfDays { get; set; }

    [JsonPropertyName("name_ar")]
    public string NameAr { get; set; } = string.Empty;

    [JsonPropertyName("name_en")]
    public string NameEn { get; set; } = string.Empty;
}

public class CalculateContractVehicleTypeInputDto
{
    public long VehicleTypeId { get; set; }
    public decimal ConsumptionRate { get; set; }
    public decimal MonthlyRental { get; set; }
    public int? TripsPerMonth { get; set; }
}

public class CalculateContractDestinationCostsDto
{
    public List<CalculatedVehicleTypeCostDto> VehicleTypes { get; set; } = new();
}

public class CalculatedVehicleTypeCostDto
{
    public long VehicleTypeId { get; set; }
    public string VehicleTypeNameAr { get; set; } = string.Empty;
    public string VehicleTypeNameEn { get; set; } = string.Empty;
    public List<CalculatedDestinationCostDto> Destinations { get; set; } = new();
}

public class CalculatedDestinationCostDto
{
    public decimal? Kms { get; set; }
    public int? NumberOfDays { get; set; }

    [JsonPropertyName("name_ar")]
    public string NameAr { get; set; } = string.Empty;

    [JsonPropertyName("name_en")]
    public string NameEn { get; set; } = string.Empty;

    public decimal DailyValue { get; set; }
    public decimal FuelPrice { get; set; }
    public decimal TotalTripCost { get; set; }
}
