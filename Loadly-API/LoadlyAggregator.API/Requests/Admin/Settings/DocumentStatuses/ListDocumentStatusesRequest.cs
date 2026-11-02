using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.DTOs.Common;

namespace LoadlyAggregator.API.Requests.Admin.Settings.DocumentStatuses;

public class ListDocumentStatusesRequest : BasePagedRequest
{
    public const string Permission = "view_document_status";


    public BasePagedRequest FromRequest() => new()
    {
        Page = Page,
        PageSize = PageSize,
        Search = Search
    };
}
