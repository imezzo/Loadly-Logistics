using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings.Districts;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.DTOs.Common;

namespace LoadlyAggregator.API.Requests.Admin.Settings.Districts;

public class ListDistrictsRequest : BasePagedRequest
{
    public const string Permission = "view_district";

    public long? CityId { get; set; }

    public ListDistrictsDto FromRequest() => new()
    {
        Page = Page,
        PageSize = PageSize,
        Search = Search,
        CityId = CityId
    };
}

public class ListDistrictsRequestValidator : AbstractValidator<ListDistrictsRequest>
{
    public ListDistrictsRequestValidator()
    {
        RuleFor(x => x.Page).IsValidPage();
        RuleFor(x => x.PageSize).IsValidPageSize();
        RuleFor(x => x.CityId)
            .IsOptionalId("City ID")
            .When(x => x.CityId.HasValue);
    }
}
