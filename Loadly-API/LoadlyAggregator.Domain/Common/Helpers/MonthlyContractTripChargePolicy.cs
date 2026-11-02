using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Helpers;

/// <summary>
/// Trip completion rules for monthly contracts (Included and Excluded): addons only on the trip;
/// structural/slot charges are not persisted at trip completion.
/// </summary>
public static class MonthlyContractTripChargePolicy
{
    public static bool AppliesTo(ContractType contractType) =>
        ContractCalculationHelper.IsMonthlyContract(contractType);

    public static InvoiceType DefaultInvoiceType => InvoiceType.Claims;

    public static void ApplyDefaultInvoiceType(IEnumerable<TripCharge> charges)
    {
        foreach (var charge in charges)
            charge.InvoiceType = DefaultInvoiceType;
    }
}
