using System.Globalization;
using System.Reflection;
using LoadlyAggregator.Domain.Admin.DTOs.Invoices;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Api;
using LoadlyAggregator.Shared.Constants;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class BankTransferDisplayLabel
{
    private static readonly CultureInfo EnCulture = CultureInfo.GetCultureInfo("en-US");
    private static readonly CultureInfo ArCulture = CultureInfo.GetCultureInfo("ar-EG");

    public static (string DisplayLabelEn, string DisplayLabelAr) Format(BankTransferDto dto)
    {
        return (Build(dto, isArabic: false), Build(dto, isArabic: true));
    }

    private static string Build(BankTransferDto dto, bool isArabic)
    {
        var culture = isArabic ? ArCulture : EnCulture;
        var payer = ResolvePayerName(dto, isArabic);
        var methodRef = FormatMethodRef(dto.PaymentMethod, dto.ExternalPaymentReferenceNumber, isArabic);
        var amount = $"{dto.Amount.ToString("N0", culture)} {(isArabic ? "جنيه" : "EGP")}";
        var remaining = dto.RemainingAmount.ToString("N0", culture);
        var unallocated = isArabic ? "غير مخصص" : "unallocated";
        var date = dto.IssueDate?.ToString("yyyy-MM-dd") ?? "—";

        return $"#{dto.Id}  {payer} · {methodRef} · {amount} · {remaining} {unallocated} · {date}";
    }

    private static string ResolvePayerName(BankTransferDto dto, bool isArabic)
    {
        if (dto.PayerType == BankTransferPayerTypes.Company && dto.Company is not null)
        {
            return isArabic
                ? dto.Company.NameAr ?? dto.Company.NameEn ?? dto.Company.FullName ?? $"Company #{dto.PayerId}"
                : dto.Company.NameEn ?? dto.Company.NameAr ?? dto.Company.FullName ?? $"Company #{dto.PayerId}";
        }

        if (dto.PayerType == BankTransferPayerTypes.Lead && dto.Lead is not null)
        {
            return dto.Lead.FullName;
        }

        if (dto.PayerType == BankTransferPayerTypes.Lead)
            return isArabic ? $"عميل محتمل #{dto.PayerId}" : $"Lead #{dto.PayerId}";

        return isArabic ? $"شركة #{dto.PayerId}" : $"Company #{dto.PayerId}";
    }

    private static string FormatMethodRef(PaymentMethod paymentMethod, string? externalReference, bool isArabic)
    {
        var method = PaymentMethodDisplay(paymentMethod, isArabic);
        var reference = externalReference?.Trim();

        return string.IsNullOrWhiteSpace(reference) ? method : $"{method}/{reference}";
    }

    private static string PaymentMethodDisplay(PaymentMethod paymentMethod, bool isArabic)
    {
        var field = paymentMethod.GetType().GetField(paymentMethod.ToString());
        var displayAttribute = field?.GetCustomAttribute<DisplayAttribute>();

        if (displayAttribute is not null)
            return isArabic ? displayAttribute.Arabic : displayAttribute.English;

        return paymentMethod.ToString();
    }
}
