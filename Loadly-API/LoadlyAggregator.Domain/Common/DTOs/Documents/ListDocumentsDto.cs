using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.DTOs.Documents;

public class ListDocumentsDto : BasePagedRequest
{
    public DocumentEntityType? EntityId { get; set; }
    public bool? Required { get; set; }
    public DocumentStatus? Status { get; set; }
}
