using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Contractor.DTOs.Documents;

namespace LoadlyAggregator.API.Requests.Contractor.Documents;

public class ContractorListDocumentsRequest : BasePagedRequest
{
    public const string Permission = "view_document";

    public DocumentEntityType? EntityId { get; set; } = DocumentEntityType.CONTRACTOR;
    public bool? Required { get; set; }
    public DocumentStatus? Status { get; set; }

    public ContractorListDocumentsDto FromRequest() => new()
    {
        Page = Page,
        PageSize = PageSize,
        Search = Search,
        EntityId = EntityId!.Value,
        Required = Required,
        Status = Status
    };
}
