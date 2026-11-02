using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Dashboard;

namespace LoadlyAggregator.API.Requests.Admin.Dashboard;

public class GetTargetRequest
{
    public const string Permission = "view_dashboard";

    public long Id { get; set; }

    public GetTargetDto FromRequest() => new() { Id = Id };
}

public class GetTargetRequestValidator : AbstractValidator<GetTargetRequest>
{
    public GetTargetRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Target ID");
    }
}
