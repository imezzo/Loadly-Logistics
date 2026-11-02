using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Contractor.DTOs.Documents;

public class ContractorListDocumentsDto : BasePagedRequest
{
    public DocumentEntityType EntityId { get; set; }
    public bool? Required { get; set; }
    public DocumentStatus? Status { get; set; }
}
