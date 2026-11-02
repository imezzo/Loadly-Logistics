using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings.Cities;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.DTOs.Common;

namespace LoadlyAggregator.API.Requests.Admin.Settings.Cities;

public class ListCitiesRequest : BasePagedRequest
{
    public const string Permission = "view_city";

    public long? GovernerateId { get; set; }

    public ListCitiesDto FromRequest() => new()
    {
        Page = Page,
        PageSize = PageSize,
        Search = Search,
        GovernerateId = GovernerateId
    };
}

public class ListCitiesRequestValidator : AbstractValidator<ListCitiesRequest>
{
    public ListCitiesRequestValidator()
    {
        RuleFor(x => x.Page).IsValidPage();
        RuleFor(x => x.PageSize).IsValidPageSize();
        RuleFor(x => x.GovernerateId)
            .IsOptionalId("Governerate ID")
            .When(x => x.GovernerateId.HasValue);
    }
}
