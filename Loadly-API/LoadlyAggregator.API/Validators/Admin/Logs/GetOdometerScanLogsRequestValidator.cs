using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Logs;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Constants;

namespace LoadlyAggregator.API.Validators.Admin.Logs;

public class GetOdometerScanLogsRequestValidator : AbstractValidator<GetOdometerScanLogsRequest>
{
    public GetOdometerScanLogsRequestValidator()
    {
        RuleFor(x => x.Page).IsValidPage();
        RuleFor(x => x.PageSize).IsValidPageSize();

        RuleFor(x => x.Status)
            .Must(value => !value.HasValue || Enum.IsDefined(typeof(OdometerScanStatus), value.Value))
            .WithMessage("validation.invalid_status_value");

        RuleFor(x => x.ScanContext)
            .Must(value => !value.HasValue || Enum.IsDefined(typeof(OdometerScanContext), value.Value))
            .WithMessage("trips.odometer_scan_context_invalid");

        RuleFor(x => x.CorrelationId)
            .MaximumLength(CorrelationIdConstants.MaxLength)
            .When(x => !string.IsNullOrWhiteSpace(x.CorrelationId));

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
            .WithMessage("validation.end_date_must_be_after_start_date");
    }
}
