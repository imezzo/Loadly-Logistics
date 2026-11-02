using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.CommunicationMethods.Requests;

public class CreateCommunicationMethodRequest : IRequiresPermission
{



    public const string Permission = "create_communication_method";

    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }




    public string RequiredPermission => Permission;
}

