using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.DTOs.Documents;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.API.Requests.Admin.Documents;

public class ListDocumentsRequest : BasePagedRequest
{
    public const string Permission = "view_document";

    public DocumentEntityType? EntityId { get; set; }
    public bool? Required { get; set; }
    public DocumentStatus? Status { get; set; }

    public ListDocumentsDto FromRequest() => new()
    {
        Page = Page,
        PageSize = PageSize,
        Search = Search,
        EntityId = EntityId,
        Required = Required,
        Status = Status
    };
}
