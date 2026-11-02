using FluentValidation;
using LoadlyAggregator.Domain.Common.DTOs.Notifications;

namespace LoadlyAggregator.API.Requests.Admin.Notifications;

public class BatchMarkNotificationsReadRequest
{
    public List<long> Ids { get; set; } = new();

    public BatchMarkNotificationsReadDto FromRequest() => new()
    {
        Ids = Ids
    };
}

public class BatchMarkNotificationsReadRequestValidator : AbstractValidator<BatchMarkNotificationsReadRequest>
{
    public BatchMarkNotificationsReadRequestValidator()
    {
        RuleFor(x => x.Ids)
            .NotEmpty().WithMessage("At least one notification ID is required")
            .Must(ids => ids.Count <= 100).WithMessage("Cannot mark more than 100 notifications at once");
    }
}
