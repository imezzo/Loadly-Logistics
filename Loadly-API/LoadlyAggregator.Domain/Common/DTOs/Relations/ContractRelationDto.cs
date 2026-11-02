using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.DTOs.Relations;

/// <summary>Contract reference (e.g. in trip).</summary>
public class ContractRelationDto
{
    public long Id { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public ContractType ContractType { get; set; }
    public ContractConsumptionTypes ConsumptionType { get; set; }
    public CompanyRelationDto? Company { get; set; }
}
