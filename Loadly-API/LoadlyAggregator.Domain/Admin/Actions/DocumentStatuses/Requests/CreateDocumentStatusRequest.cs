using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.DocumentStatuses.Requests;

public class CreateDocumentStatusRequest : IRequiresPermission
{



    public const string Permission = "create_document_status";

    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;




    public string RequiredPermission => Permission;
}

