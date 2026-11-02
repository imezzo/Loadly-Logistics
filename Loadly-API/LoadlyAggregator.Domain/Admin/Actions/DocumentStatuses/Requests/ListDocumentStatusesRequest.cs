using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.DocumentStatuses.Requests;

public class ListDocumentStatusesRequest : BasePagedRequest, IRequiresPermission
{
    public const string Permission = "view_document_status";

    public string RequiredPermission => Permission;
}

