using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings.Cities;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;

namespace LoadlyAggregator.API.Requests.Admin.Settings.Cities;

public class GetCityRequest
{
    public const string Permission = "view_city";
    public long Id { get; set; }
    public GetCityDto FromRequest() => new() { Id = Id };
}

public class GetCityRequestValidator : AbstractValidator<GetCityRequest>
{
    public GetCityRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("City ID");
    }
}
