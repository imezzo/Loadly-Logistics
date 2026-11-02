using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.DocumentStatuses.Requests;

public class UpdateDocumentStatusRequest : IRequiresPermission
{



    public const string Permission = "edit_document_status";

    public long Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;




    public string RequiredPermission => Permission;
}

