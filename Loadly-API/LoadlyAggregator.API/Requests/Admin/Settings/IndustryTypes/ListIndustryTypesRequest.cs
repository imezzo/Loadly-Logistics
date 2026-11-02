using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings.IndustryTypes;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.DTOs.Common;

namespace LoadlyAggregator.API.Requests.Admin.Settings.IndustryTypes;

public class ListIndustryTypesRequest : BasePagedRequest
{
    public const string Permission = "view_industry_type";


    public ListIndustryTypesDto FromRequest() => new()
    {
        Page = Page,
        PageSize = PageSize,
        Search = Search
    };
}

public class ListIndustryTypesRequestValidator : AbstractValidator<ListIndustryTypesRequest>
{
    public ListIndustryTypesRequestValidator()
    {
        RuleFor(x => x.Page).IsValidPage();
        RuleFor(x => x.PageSize).IsValidPageSize();
    }
}
