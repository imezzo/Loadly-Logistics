using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Collector.DTOs.Documents;

public class CollectorListDocumentsDto : BasePagedRequest
{
    public DocumentEntityType? EntityId { get; set; }
    public bool? Required { get; set; }
    public DocumentStatus? Status { get; set; }
}
