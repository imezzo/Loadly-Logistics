using System.Linq.Expressions;
using FluentValidation;
using LoadlyAggregator.Domain.Admin.DTOs.Invoices;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.API.Validators.Extensions;

public static class InvoicePaymentValidationRules
{
    /// <summary>
    /// Validates an optional list of bank-transfer allocations: each entry needs a valid
    /// transfer id and a positive amount, and no transfer may appear twice (the allocation
    /// is upserted per (invoice, transfer), so a duplicate would overwrite rather than add).
    /// Also enforces that status and payments are mutually exclusive.
    /// </summary>
    public static void ApplyInvoicePaymentRules<T>(
        this AbstractValidator<T> validator,
        Expression<Func<T, IEnumerable<InvoicePaymentAllocationDto>>> paymentsSelector,
        Expression<Func<T, InvoiceStatus?>> statusSelector)
    {
        validator.RuleForEach(paymentsSelector).ChildRules(p =>
        {
            p.RuleFor(a => a.BankTransferId).IsRequiredId("Bank transfer ID");
            p.RuleFor(a => a.Amount)
                .GreaterThan(0m)
                .WithMessage("invoice_payments.allocation_amount_must_be_greater_than_zero");
        });

        // null = omitted (use status path); non-null empty [] = client sent Payments — reject
        validator.RuleFor(paymentsSelector)
            .Must(list => list is null || list.Any())
            .WithMessage("validation.collection_at_least_one|Payments");

        validator.RuleFor(paymentsSelector)
            .Must(list => list is null
                || list.Select(p => p.BankTransferId).Distinct().Count() == list.Count())
            .WithMessage("invoice_payments.duplicate_bank_transfer");

        validator.ApplyInvoiceStatusPaymentExclusivityRules(statusSelector, paymentsSelector);
    }

    /// <summary>
    /// Status may be supplied only when no bank-transfer allocations are present, and
    /// vice versa. When payments are supplied, status is derived from allocations.
    /// </summary>
    public static void ApplyInvoiceStatusPaymentExclusivityRules<T>(
        this AbstractValidator<T> validator,
        Expression<Func<T, InvoiceStatus?>> statusSelector,
        Expression<Func<T, IEnumerable<InvoicePaymentAllocationDto>>> paymentsSelector)
    {
        validator.RuleFor(statusSelector)
            .Null()
            .When(x => HasPayments(paymentsSelector, x))
            .WithMessage("invoices.status_not_allowed_with_payments");

        validator.RuleFor(paymentsSelector)
            .Must(list => list is null || !list.Any())
            .When(x => statusSelector.Compile()(x).HasValue)
            .WithMessage("invoices.payments_not_allowed_with_status");

        // Without bank transfers, the client may only declare UnPaid or Paid.
        // PartiallyPaid is derived when allocations cover part of the total.
        validator.RuleFor(statusSelector)
            .Must(s => s is InvoiceStatus.UnPaid or InvoiceStatus.Paid)
            .When(x => statusSelector.Compile()(x).HasValue && !HasPayments(paymentsSelector, x))
            .WithMessage("invoices.status_without_payments_must_be_unpaid_or_paid");
    }

    private static bool HasPayments<T>(
        Expression<Func<T, IEnumerable<InvoicePaymentAllocationDto>>> paymentsSelector,
        T instance)
    {
        var payments = paymentsSelector.Compile()(instance);
        return payments is not null && payments.Any();
    }
}
