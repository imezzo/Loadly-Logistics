using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.CommunicationMethods.Requests;

public class DeleteCommunicationMethodRequest : IRequiresPermission
{



    public const string Permission = "delete_communication_method";

    public long Id { get; set; }




    public string RequiredPermission => Permission;
}

