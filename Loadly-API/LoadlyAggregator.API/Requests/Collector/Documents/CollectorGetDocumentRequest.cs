using LoadlyAggregator.Domain.Collector.DTOs.Documents;

namespace LoadlyAggregator.API.Requests.Collector.Documents;

public class CollectorGetDocumentRequest
{
    public const string Permission = "view_document";

    public long Id { get; set; }

    public CollectorGetDocumentDto FromRequest() => new() { Id = Id };
}
