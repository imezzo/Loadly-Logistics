using LoadlyAggregator.Domain.Admin.DTOs.Settings;

namespace LoadlyAggregator.API.Requests.Admin.Settings.DocumentStatuses;

public class GetDocumentStatusRequest
{
    public const string Permission = "view_document_status";

    public long Id { get; set; }

    public GetDocumentStatusDto FromRequest() => new()
    {
        Id = Id
    };
}
