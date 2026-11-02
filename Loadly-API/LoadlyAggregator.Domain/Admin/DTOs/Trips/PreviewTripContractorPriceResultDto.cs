using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Admin.DTOs.Trips;

public class PreviewTripContractorPriceResultDto
{
    public ContractConsumptionTypes ConsumptionType { get; set; }
    public bool IsApplicable { get; set; }
    public bool FormulaValidationApplies { get; set; }
    public bool ManualEntryRequired { get; set; }
    public decimal? ExpectedContractorPrice { get; set; }
    public string? PricingSource { get; set; }
    public PreviewTripContractorPriceBreakdownDto? Breakdown { get; set; }
}

public class PreviewTripContractorPriceBreakdownDto
{
    public decimal ActualCargoWeightTons { get; set; }
    public decimal MinBillableWeightTons { get; set; }
    public decimal BillableTons { get; set; }
    public decimal ContractorPricePerTon { get; set; }
    public decimal BaseCost { get; set; }
    public decimal? MaxPayloadTons { get; set; }
    public decimal? ContractorOverWeightPricePerTon { get; set; }
    public decimal OverweightCost { get; set; }
    public decimal? ContractorFixedTripPrice { get; set; }
}
