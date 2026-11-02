using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Logs;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Constants;

namespace LoadlyAggregator.API.Validators.Admin.Logs;

public class GetActivityLogsRequestValidator : AbstractValidator<GetActivityLogsRequest>
{
    public GetActivityLogsRequestValidator()
    {
        RuleFor(x => x.Page).IsValidPage();
        RuleFor(x => x.PageSize).IsValidPageSize();

        RuleFor(x => x.EventType)
            .Must(value => string.IsNullOrWhiteSpace(value) || Enum.TryParse<AuditEventType>(value, true, out _))
            .WithMessage("validation.invalid_event_type");

        RuleFor(x => x.CorrelationId)
            .MaximumLength(CorrelationIdConstants.MaxLength)
            .When(x => !string.IsNullOrWhiteSpace(x.CorrelationId));

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
            .WithMessage("validation.end_date_must_be_after_start_date");
    }
}
