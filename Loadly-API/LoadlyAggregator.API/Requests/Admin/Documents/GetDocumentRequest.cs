using LoadlyAggregator.Domain.Common.DTOs.Documents;

namespace LoadlyAggregator.API.Requests.Admin.Documents;

public class GetDocumentRequest
{
    public const string Permission = "view_document";

    public long Id { get; set; }

    public GetDocumentDto FromRequest() => new() { Id = Id };
}
