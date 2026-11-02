using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.API.Resources.Admin.Trips;

public class PreviewTripContractorPriceResource
{
    public ContractConsumptionTypes ConsumptionType { get; set; }
    public bool IsApplicable { get; set; }
    public bool FormulaValidationApplies { get; set; }
    public bool ManualEntryRequired { get; set; }
    public decimal? ExpectedContractorPrice { get; set; }
    public string? PricingSource { get; set; }
    public PreviewTripContractorPriceBreakdownResource? Breakdown { get; set; }

    public static PreviewTripContractorPriceResource FromDto(PreviewTripContractorPriceResultDto dto) =>
        new()
        {
            ConsumptionType = dto.ConsumptionType,
            IsApplicable = dto.IsApplicable,
            FormulaValidationApplies = dto.FormulaValidationApplies,
            ManualEntryRequired = dto.ManualEntryRequired,
            ExpectedContractorPrice = dto.ExpectedContractorPrice,
            PricingSource = dto.PricingSource,
            Breakdown = dto.Breakdown is null
                ? null
                : PreviewTripContractorPriceBreakdownResource.FromDto(dto.Breakdown)
        };
}

public class PreviewTripContractorPriceBreakdownResource
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

    public static PreviewTripContractorPriceBreakdownResource FromDto(PreviewTripContractorPriceBreakdownDto dto) =>
        new()
        {
            ActualCargoWeightTons = dto.ActualCargoWeightTons,
            MinBillableWeightTons = dto.MinBillableWeightTons,
            BillableTons = dto.BillableTons,
            ContractorPricePerTon = dto.ContractorPricePerTon,
            BaseCost = dto.BaseCost,
            MaxPayloadTons = dto.MaxPayloadTons,
            ContractorOverWeightPricePerTon = dto.ContractorOverWeightPricePerTon,
            OverweightCost = dto.OverweightCost,
            ContractorFixedTripPrice = dto.ContractorFixedTripPrice
        };
}
