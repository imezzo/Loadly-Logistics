using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class InvoiceTotalsCalculator
{
    public static (decimal DiscountAmount, decimal TaxAmount, decimal TotalAmount) CalculateAll(
        decimal subTotal,
        TaxType? taxType,
        decimal? taxValue,
        DiscountType? discountType,
        decimal? discountValue)
    {
        var discountAmount = discountType.HasValue && discountValue.HasValue
            ? CalculateDiscountAmount(discountType.Value, discountValue.Value, subTotal)
            : 0m;

        var taxableBase = RoundMoney(subTotal - discountAmount);
        if (taxableBase < 0m)
            taxableBase = 0m;

        var taxAmount = taxType.HasValue && taxValue.HasValue
            ? CalculateTaxAmount(taxType.Value, taxValue.Value, taxableBase)
            : 0m;

        var totalAmount = CalculateTotalAmount(subTotal, taxAmount, discountAmount);
        return (discountAmount, taxAmount, totalAmount);
    }

    public static decimal CalculateTaxAmount(TaxType type, decimal value, decimal taxableBase) =>
        RoundMoney(type == TaxType.Fixed ? value : taxableBase * value / 100m);

    public static decimal CalculateDiscountAmount(DiscountType type, decimal value, decimal subTotal) =>
        RoundMoney(type == DiscountType.Fixed ? value : subTotal * value / 100m);

    public static decimal CalculateTotalAmount(decimal subTotal, decimal taxAmount, decimal discountAmount)
    {
        var amount = subTotal + taxAmount - discountAmount;
        return RoundMoney(amount < 0m ? 0m : amount);
    }

    private static decimal RoundMoney(decimal value) =>
        Math.Round(value, 2, MidpointRounding.AwayFromZero);

}
