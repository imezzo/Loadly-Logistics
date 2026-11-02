using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Common.Services.Validation;

public static class BankTransferAllocationValidator
{
    public static async Task ValidateAllocationAsync(
        Invoice invoice,
        BankTransfer bankTransfer,
        decimal amount,
        IInvoicePaymentRepository invoicePaymentRepository,
        InvoicePayment? existingPayment = null)
    {
        var invoicePaid = await invoicePaymentRepository.GetTotalPaidAmountByInvoiceIdAsync(invoice.Id);
        var transferAllocated = await invoicePaymentRepository.GetTotalAllocatedAmountByBankTransferIdAsync(bankTransfer.Id);
        ValidateAllocation(invoice, bankTransfer, amount, invoicePaid, transferAllocated, existingPayment);
    }

    public static void ValidateAllocation(
        Invoice invoice,
        BankTransfer bankTransfer,
        decimal amount,
        decimal invoicePaid,
        decimal transferAllocated,
        InvoicePayment? existingPayment = null)
    {
        if (amount <= 0m)
            throw new ValidationException("invoice_payments.allocation_amount_must_be_greater_than_zero");

        if (invoice.Status == InvoiceStatus.Cancelled)
            throw new ValidationException("invoice_payments.cannot_allocate_to_cancelled_invoice");

        if (invoice.Contract is null && !invoice.LeadId.HasValue)
            throw new ValidationException("invoice_payments.invoice_bill_to_required_for_allocation");

        if (!PayerMatchesInvoiceBillTo(invoice, bankTransfer))
            throw new ValidationException("invoice_payments.payer_must_match_invoice_bill_to");

        var previousForPair = existingPayment?.Amount ?? 0m;
        var newInvoicePaid = invoicePaid - previousForPair + amount;
        if (invoice.Status == InvoiceStatus.Paid && newInvoicePaid < invoice.TotalAmount)
            throw new ValidationException("invoice_payments.cannot_reduce_allocation_on_paid_invoice");

        if (newInvoicePaid > invoice.TotalAmount)
            throw new ValidationException("invoice_payments.would_exceed_invoice_total");

        var newTransferAllocated = transferAllocated - previousForPair + amount;
        if (newTransferAllocated > bankTransfer.Amount)
            throw new ValidationException("invoice_payments.would_exceed_bank_transfer_amount");
    }

    public static async Task ValidateCanUpdateAsync(
        BankTransfer bankTransfer,
        decimal newAmount,
        PaymentMethod newPaymentMethod,
        long newPayerId,
        string newPayerType,
        IInvoicePaymentRepository invoicePaymentRepository)
    {
        var allocated = await invoicePaymentRepository.GetTotalAllocatedAmountByBankTransferIdAsync(bankTransfer.Id);
        if (allocated <= 0m)
            return;

        if (newAmount < allocated)
            throw new ValidationException("bank_transfers.cannot_reduce_amount_below_allocated");

        if (newPaymentMethod != bankTransfer.PaymentMethod)
            throw new ValidationException("bank_transfers.cannot_change_payment_method_with_allocations");

        if (newPayerId != bankTransfer.PayerId || newPayerType != bankTransfer.PayerType)
            throw new ValidationException("bank_transfers.cannot_change_payer_with_allocations");
    }

    public static async Task ValidateCanDeleteAsync(
        BankTransfer bankTransfer,
        IInvoicePaymentRepository invoicePaymentRepository)
    {
        var allocated = await invoicePaymentRepository.GetTotalAllocatedAmountByBankTransferIdAsync(bankTransfer.Id);
        if (allocated > 0m)
            throw new ValidationException("bank_transfers.cannot_delete_with_allocations");
    }

    private static bool PayerMatchesInvoiceBillTo(Invoice invoice, BankTransfer bankTransfer)
    {
        if (!bankTransfer.PayerId.HasValue || string.IsNullOrWhiteSpace(bankTransfer.PayerType))
            return false;

        return bankTransfer.PayerType switch
        {
            BankTransferPayerTypes.Company =>
                invoice.Contract is not null
                && bankTransfer.PayerId.Value == invoice.Contract.CompanyId,
            BankTransferPayerTypes.Lead =>
                invoice.LeadId.HasValue
                && bankTransfer.PayerId.Value == invoice.LeadId.Value,
            _ => false
        };
    }

    public static InvoiceStatus ResolveInvoiceStatus(decimal paidAmount, decimal totalAmount)
    {
        if (paidAmount <= 0m)
            return InvoiceStatus.UnPaid;

        if (paidAmount >= totalAmount)
            return InvoiceStatus.Paid;

        return InvoiceStatus.PartiallyPaid;
    }

    public static void SyncInvoiceStatus(Invoice invoice, decimal paidAmount)
    {
        if (invoice.Status == InvoiceStatus.Cancelled)
            return;

        invoice.Status = ResolveInvoiceStatus(paidAmount, invoice.TotalAmount);
    }

}
