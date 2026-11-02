using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings.Districts;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;

namespace LoadlyAggregator.API.Requests.Admin.Settings.Districts;

public class GetDistrictRequest
{
    public const string Permission = "view_district";
    public long Id { get; set; }
    public GetDistrictDto FromRequest() => new() { Id = Id };
}

public class GetDistrictRequestValidator : AbstractValidator<GetDistrictRequest>
{
    public GetDistrictRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("District ID");
    }
}
