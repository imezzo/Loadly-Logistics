using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class MonthlySlotInvoicePolicy
{
    public static bool IsMonthlyContract(Contract? contract, bool isLeadBilling) =>
        !isLeadBilling
        && contract is not null
        && ContractCalculationHelper.IsMonthlyContract(contract.ContractType);

    public static bool AppliesTo(Contract? contract, InvoiceType invoiceType, bool isLeadBilling) =>
        IsMonthlyContract(contract, isLeadBilling)
        && invoiceType == InvoiceType.Trips;

    public static bool UsesBillingSnapshot(Contract? contract, bool isLeadBilling) =>
        IsMonthlyContract(contract, isLeadBilling);
}
