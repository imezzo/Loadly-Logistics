using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.DTOs.Common;

namespace LoadlyAggregator.API.Requests.Admin.Settings.Tags;

public class ListTagsRequest : BasePagedRequest
{
    public const string Permission = "view_tag";

    public bool? IsPricing { get; set; }

    public ListTagsDto FromRequest() => new()
    {
        Page = Page,
        PageSize = PageSize,
        Search = Search,
        IsPricing = IsPricing
    };
}

public class ListTagsRequestValidator : AbstractValidator<ListTagsRequest>
{
    public ListTagsRequestValidator()
    {
        RuleFor(x => x.Page).IsValidPage();
        RuleFor(x => x.PageSize).IsValidPageSize();
    }
}
