using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Company.DTOs.Contracts;

public class CompanyListContractsDto : BasePagedRequest
{
    public long CompanyId { get; set; }
    public long? ContractId { get; set; }
    public ContractType? ContractType { get; set; }
    public string? ReferenceNumber { get; set; }
}
