using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings.Governerates;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;

namespace LoadlyAggregator.API.Requests.Admin.Settings.Governerates;

public class GetGovernerateRequest
{
    public const string Permission = "view_governerate";
    public long Id { get; set; }
    public GetGovernerateDto FromRequest() => new() { Id = Id };
}

public class GetGovernerateRequestValidator : AbstractValidator<GetGovernerateRequest>
{
    public GetGovernerateRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Governerate ID");
    }
}
