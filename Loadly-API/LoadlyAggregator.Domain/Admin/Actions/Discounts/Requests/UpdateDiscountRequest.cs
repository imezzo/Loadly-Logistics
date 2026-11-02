using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Discounts.Requests;

public class UpdateDiscountRequest : IRequiresPermission
{



    public const string Permission = "edit_discount";

    public long Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public DiscountType Type { get; set; }
    public decimal Value { get; set; }
    public DiscountStatus Status { get; set; }




    public string RequiredPermission => Permission;
}

