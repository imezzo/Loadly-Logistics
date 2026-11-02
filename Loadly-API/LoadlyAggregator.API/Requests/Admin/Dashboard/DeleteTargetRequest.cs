using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Dashboard;

namespace LoadlyAggregator.API.Requests.Admin.Dashboard;

public class DeleteTargetRequest
{
    public const string Permission = "manage_dashboard_targets";

    public long Id { get; set; }

    public DeleteTargetDto FromRequest() => new() { Id = Id };
}

public class DeleteTargetRequestValidator : AbstractValidator<DeleteTargetRequest>
{
    public DeleteTargetRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Target ID");
    }
}
