using FluentValidation;
using LoadlyAggregator.Domain.Admin.DTOs.Collectors;

namespace LoadlyAggregator.API.Requests.Admin.Collectors;

public class BulkUpdateCollectorStatusIdsRequest
{
    public List<long> CollectorIds { get; set; } = new();

    public BulkUpdateCollectorStatusDto FromRequest(int status) => new()
    {
        CollectorIds = CollectorIds,
        Status = status
    };
}

public class BulkUpdateCollectorStatusIdsRequestValidator : AbstractValidator<BulkUpdateCollectorStatusIdsRequest>
{
    public BulkUpdateCollectorStatusIdsRequestValidator()
    {
        RuleFor(x => x.CollectorIds)
            .NotEmpty()
            .WithMessage("validation.at_least_one_collector_id");
    }
}
