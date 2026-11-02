using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Discounts.Requests;

public class CreateDiscountRequest : IRequiresPermission
{



    public const string Permission = "create_discount";

    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public DiscountType Type { get; set; }
    public decimal Value { get; set; }
    public DiscountStatus Status { get; set; } = DiscountStatus.ACTIVE;




    public string RequiredPermission => Permission;
}

