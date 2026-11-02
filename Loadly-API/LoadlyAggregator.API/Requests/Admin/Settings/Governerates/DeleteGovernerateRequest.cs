using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings.Governerates;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;

namespace LoadlyAggregator.API.Requests.Admin.Settings.Governerates;

public class DeleteGovernerateRequest
{
    public const string Permission = "delete_governerate";
    public long Id { get; set; }
    public DeleteGovernerateDto FromRequest() => new() { Id = Id };
}

public class DeleteGovernerateRequestValidator : AbstractValidator<DeleteGovernerateRequest>
{
    public DeleteGovernerateRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Governerate ID");
    }
}
