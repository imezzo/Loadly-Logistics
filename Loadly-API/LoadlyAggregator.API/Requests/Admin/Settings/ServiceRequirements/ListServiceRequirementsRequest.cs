using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings.ServiceRequirements;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.DTOs.Common;

namespace LoadlyAggregator.API.Requests.Admin.Settings.ServiceRequirements;

public class ListServiceRequirementsRequest : BasePagedRequest
{
    public const string Permission = "view_service_requirement";


    public ListServiceRequirementsDto FromRequest() => new()
    {
        Page = Page,
        PageSize = PageSize,
        Search = Search
    };
}

public class ListServiceRequirementsRequestValidator : AbstractValidator<ListServiceRequirementsRequest>
{
    public ListServiceRequirementsRequestValidator()
    {
        RuleFor(x => x.Page).IsValidPage();
        RuleFor(x => x.PageSize).IsValidPageSize();
    }
}
