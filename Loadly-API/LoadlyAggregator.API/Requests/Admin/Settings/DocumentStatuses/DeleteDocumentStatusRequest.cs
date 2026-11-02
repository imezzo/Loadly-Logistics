using LoadlyAggregator.Domain.Admin.DTOs.Settings;

namespace LoadlyAggregator.API.Requests.Admin.Settings.DocumentStatuses;

public class DeleteDocumentStatusRequest
{
    public const string Permission = "delete_document_status";

    public long Id { get; set; }

    public DeleteDocumentStatusDto FromRequest() => new()
    {
        Id = Id
    };
}
