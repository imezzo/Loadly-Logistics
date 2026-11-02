using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.CommunicationMethods.Requests;

public class UpdateCommunicationMethodRequest : IRequiresPermission
{



    public const string Permission = "edit_communication_method";

    public long Id { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }




    public string RequiredPermission => Permission;
}

