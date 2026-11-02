using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Invoices;

namespace LoadlyAggregator.API.Requests.Admin.Invoices;

public class LinkInvoicePaymentsRequest
{
    public const string Permission = "edit_invoices";

    public List<LinkInvoicePaymentItemDto> Payments { get; set; } = new();

    public LinkInvoicePaymentsDto FromRequest(long id) => new()
    {
        Id = id,
        Payments = Payments
    };
}

public class LinkInvoicePaymentsRequestValidator : AbstractValidator<LinkInvoicePaymentsRequest>
{
    public LinkInvoicePaymentsRequestValidator()
    {
        RuleFor(x => x.Payments)
            .NotEmpty()
            .WithMessage("validation.collection_at_least_one|Payments");
        RuleForEach(x => x.Payments).ChildRules(p =>
        {
            p.RuleFor(a => a.InvoicePaymentId)
                .IsOptionalId("Invoice payment ID")
                .When(a => a.InvoicePaymentId.HasValue);
            p.RuleFor(a => a.BankTransferId).IsRequiredId("Bank transfer ID");
            p.RuleFor(a => a.Amount)
                .GreaterThan(0m)
                .WithMessage("invoice_payments.allocation_amount_must_be_greater_than_zero");
        });
        RuleFor(x => x.Payments)
            .Must(list => list.Select(p => p.BankTransferId).Distinct().Count() == list.Count)
            .WithMessage("invoice_payments.duplicate_bank_transfer");
        RuleFor(x => x.Payments)
            .Must(list => list
                .Where(p => p.InvoicePaymentId.HasValue)
                .GroupBy(p => p.InvoicePaymentId)
                .All(g => g.Count() == 1))
            .WithMessage("invoice_payments.duplicate_invoice_payment");
    }
}
