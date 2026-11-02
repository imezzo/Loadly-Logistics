using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings.Governerates;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.DTOs.Common;

namespace LoadlyAggregator.API.Requests.Admin.Settings.Governerates;

public class ListGoverneratesRequest : BasePagedRequest
{
    public const string Permission = "view_governerate";


    public ListGoverneratesDto FromRequest() => new()
    {
        Page = Page,
        PageSize = PageSize,
        Search = Search
    };
}

public class ListGoverneratesRequestValidator : AbstractValidator<ListGoverneratesRequest>
{
    public ListGoverneratesRequestValidator()
    {
        RuleFor(x => x.Page).IsValidPage();
        RuleFor(x => x.PageSize).IsValidPageSize();
    }
}
