using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Discounts.Requests;

public class DeleteDiscountRequest : IRequiresPermission
{



    public const string Permission = "delete_discount";

    public long Id { get; set; }




    public string RequiredPermission => Permission;
}

