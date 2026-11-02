using FluentValidation;
using LoadlyAggregator.Domain.Admin.DTOs.Invoices;

namespace LoadlyAggregator.API.Requests.Admin.Invoices;

public class CancelInvoiceRequest
{
    public const string Permission = "cancel_invoice";

    public long Id { get; set; }
    public string? CancellationReason { get; set; }

    public CancelInvoiceDto FromRequest() => new()
    {
        Id = Id,
        CancellationReason = CancellationReason
    };
}

public class CancelInvoiceRequestValidator : AbstractValidator<CancelInvoiceRequest>
{
    public CancelInvoiceRequestValidator()
    {
        When(x => !string.IsNullOrWhiteSpace(x.CancellationReason), () =>
            RuleFor(x => x.CancellationReason).MaximumLength(500).WithMessage("validation.cancellation_reason_max_length|500"));
    }
}
