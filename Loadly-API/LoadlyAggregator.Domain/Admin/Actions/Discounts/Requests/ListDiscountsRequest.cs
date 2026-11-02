using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Discounts.Requests;

public class ListDiscountsRequest : BasePagedRequest, IRequiresPermission
{
    public const string Permission = "view_discount";

    public DiscountType? Type { get; set; }
    public DiscountStatus? Status { get; set; }

    public string RequiredPermission => Permission;
}

