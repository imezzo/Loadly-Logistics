using LoadlyAggregator.Domain.Admin.DTOs.Settings;

namespace LoadlyAggregator.Domain.Admin.DTOs.Contracts;

public class ContractExtraFeeDto
{
    public long Id { get; set; }
    public long ContractId { get; set; }
    public long ExtraFeeTypeId { get; set; }
    public int? CalculationOption { get; set; }
    public decimal? Value { get; set; }
    public int? Threshold { get; set; }
    public string? Notes { get; set; }
    public bool? AllowOverride { get; set; }
    public long? AddedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ContractDto? Contract { get; set; }
    public ExtraFeeTypeDto? ExtraFeeType { get; set; }
}
