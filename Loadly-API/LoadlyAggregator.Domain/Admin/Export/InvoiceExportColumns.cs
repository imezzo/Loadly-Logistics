using System.Globalization;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Export;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Admin.Export;

public static class InvoiceExportColumns
{
    public static IReadOnlyList<ExportColumn<Invoice>> All { get; } =
    [
        new("invoiceNumber", "Invoice Number", "رقم الفاتورة", (invoice, _) => invoice.InvoiceNumber),
        new("companyName", "Company Name", "اسم الشركة", GetCompanyName),
        new("contractRef", "Contract Ref", "العقد", (invoice, _) => GetContractReferenceNumber(invoice)),
        new("invoiceType", "Invoice Type", "نوع الفاتورة", (invoice, _) => invoice.InvoiceType.ToLocalizedDisplay()),
        new("issueDate", "Issue Date", "تاريخ الإصدار", (invoice, _) => invoice.IssueDate.HasValue
            ? invoice.IssueDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
            : string.Empty),
        new("dueDate", "Due Date", "تاريخ الاستحقاق", (invoice, _) => invoice.DueDate.HasValue
            ? invoice.DueDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
            : string.Empty),
        new("amount", "Amount", "المبلغ", (invoice, _) => invoice.TotalAmount),
        new("tax", "Tax", "الضريبة", (invoice, _) => invoice.TaxType.HasValue && invoice.TaxValue.HasValue
            ? invoice.TaxType == TaxType.Percentage
                ? $"{invoice.TaxValue.Value}%"
                : $"{invoice.TaxValue.Value} egp"
            : string.Empty),
        new("taxAmount", "Tax Amount", "مبلغ الضريبة", (invoice, _) => invoice.TaxAmount),
        new("discount", "Discount", "الخصم", (invoice, _) => invoice.DiscountType.HasValue && invoice.DiscountValue.HasValue
            ? invoice.DiscountType == DiscountType.Percentage
                ? $"{invoice.DiscountValue.Value}%"
                : $"{invoice.DiscountValue.Value} egp"
            : string.Empty),
        new("discountAmount", "Discount Amount", "مبلغ الخصم", (invoice, _) => invoice.DiscountAmount),
        new("statusName", "Status Name", "الحالة", (invoice, _) => invoice.Status.ToLocalizedDisplay())
    ];

    private static object? GetCompanyName(Invoice invoice, ILanguageService languageService) =>
        invoice.Contract?.Company?.CompanyDetail is CompanyDetail companyDetail
            ? languageService.IsArabic()
                ? companyDetail.NameAr ?? string.Empty
                : companyDetail.NameEn ?? string.Empty
            : string.Empty;

    private static string GetContractReferenceNumber(Invoice invoice) =>
        invoice.Contract?.ReferenceNumber
        ?? (invoice.InvoiceType == InvoiceType.Claims
            ? invoice.ClaimInvoiceTrips?.FirstOrDefault()?.Contract?.ReferenceNumber
            : invoice.Trips?.FirstOrDefault()?.Contract?.ReferenceNumber)
        ?? string.Empty;
}
