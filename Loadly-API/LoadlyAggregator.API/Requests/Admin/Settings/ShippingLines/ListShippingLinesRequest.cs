using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.API.Requests.Admin.Settings.ShippingLines;

public class ListShippingLinesRequest : BasePagedRequest
{
    public const string Permission = "view_shipping_line";

    public ShippingLineStatus? Status { get; set; }

    public ListShippingLinesDto FromRequest() => new()
    {
        Page = Page,
        PageSize = PageSize,
        Search = Search,
        Status = Status
    };
}

public class ListShippingLinesRequestValidator : AbstractValidator<ListShippingLinesRequest>
{
    public ListShippingLinesRequestValidator()
    {
        RuleFor(x => x.Page).IsValidPage();
        RuleFor(x => x.PageSize).IsValidPageSize();
        RuleFor(x => x.Status)
            .IsValidEnum<ListShippingLinesRequest, ShippingLineStatus>("Status")
            .When(x => x.Status.HasValue);
    }
}
