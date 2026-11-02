using LoadlyAggregator.Domain.Collector.DTOs.Documents;
using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.API.Requests.Collector.Documents;

public class CollectorListDocumentsRequest : BasePagedRequest
{
    public const string Permission = "view_document";

    public DocumentEntityType? EntityId { get; set; }
    public bool? Required { get; set; }
    public DocumentStatus? Status { get; set; }

    public CollectorListDocumentsDto FromRequest() => new()
    {
        Page = Page,
        PageSize = PageSize,
        Search = Search,
        EntityId = EntityId,
        Required = Required,
        Status = Status
    };
}
