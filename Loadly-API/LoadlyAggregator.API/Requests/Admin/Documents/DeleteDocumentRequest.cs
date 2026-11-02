using LoadlyAggregator.Domain.Common.DTOs.Documents;

namespace LoadlyAggregator.API.Requests.Admin.Documents;

public class DeleteDocumentRequest
{
    public const string Permission = "delete_document";

    public long Id { get; set; }

    public DeleteDocumentDto FromRequest() => new() { Id = Id };
}
