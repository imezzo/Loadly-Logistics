using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Discounts.Requests;

public class GetDiscountRequest : IRequiresPermission
{



    public const string Permission = "view_discount";

    public long Id { get; set; }




    public string RequiredPermission => Permission;
}

