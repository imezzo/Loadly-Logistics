using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.DocumentStatuses.Requests;

public class DeleteDocumentStatusRequest : IRequiresPermission
{



    public const string Permission = "delete_document_status";

    public long Id { get; set; }




    public string RequiredPermission => Permission;
}

