using LoadlyAggregator.Domain.Common.BaseClasses;
namespace LoadlyAggregator.Domain.Common.Entities;

public class ContractExtraFee : BaseEntity
{
    public long ContractId { get; set; }
    public int ExtraFeeTypeId { get; set; }
    public int? CalculationOption { get; set; }
    public decimal? Value { get; set; }
    public int? Threshold { get; set; }
    public string? Notes { get; set; }
    public bool? AllowOverride { get; set; }
    public long? AddedBy { get; set; }

    public Contract? Contract { get; set; }
}
