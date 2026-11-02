using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.CommunicationMethods.Requests;

public class ListCommunicationMethodsRequest : BasePagedRequest, IRequiresPermission
{
    public const string Permission = "view_communication_method";

    public string RequiredPermission => Permission;
}

