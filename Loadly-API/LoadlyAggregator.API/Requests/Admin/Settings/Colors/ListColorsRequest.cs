using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings.Colors;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.DTOs.Common;

namespace LoadlyAggregator.API.Requests.Admin.Settings.Colors;

public class ListColorsRequest : BasePagedRequest
{
    public ListColorsDto FromRequest() => new()
    {
        Page = Page,
        PageSize = PageSize,
        Search = Search
    };
}

public class ListColorsRequestValidator : AbstractValidator<ListColorsRequest>
{
    public ListColorsRequestValidator()
    {
        RuleFor(x => x.Page).IsValidPage();
        RuleFor(x => x.PageSize).IsValidPageSize();
    }
}
