using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.DocumentStatuses.Requests;

public class GetDocumentStatusRequest : IRequiresPermission
{



    public const string Permission = "view_document_status";

    public long Id { get; set; }




    public string RequiredPermission => Permission;
}

