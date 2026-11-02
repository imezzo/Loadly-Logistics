using LoadlyAggregator.Domain.Admin.DTOs.Drivers;

namespace LoadlyAggregator.API.Requests.Admin.DriverDocuments;

public class DownloadDriverDocumentRequest
{
    public const string Permission = "view_driver_document";

    public long Id { get; set; }

    public DownloadDriverDocumentDto FromRequest() => new() { Id = Id };
}
