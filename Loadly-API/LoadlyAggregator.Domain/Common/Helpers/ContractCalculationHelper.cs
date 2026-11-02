using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class ContractCalculationHelper
{
    public const int DefaultMonthlyWorkingDays = 26;

    public static bool IsMonthlyContract(ContractType contractType) =>
        contractType is ContractType.Included or ContractType.Excluded;

    public static int? ResolveMonthlyWorkingDays(int? monthlyWorkingDays, ContractType contractType) =>
        monthlyWorkingDays ?? (IsMonthlyContract(contractType) ? DefaultMonthlyWorkingDays : null);

    public static int? CalculateTripsPerDay(int? tripsPerMonth, int? monthlyWorkingDays)
    {
        if (!tripsPerMonth.HasValue || monthlyWorkingDays is not > 0)
            return null;

        return (int)Math.Ceiling(tripsPerMonth.Value / (double)monthlyWorkingDays.Value);
    }

    public static ContractKmOverageMode? NormalizeKmOverageModeForStorage(
        ContractType contractType,
        ContractKmOverageMode? kmOverageMode) =>
        contractType == ContractType.Included ? kmOverageMode : null;

    public static ContractKmOverageMode ResolveIncludedKmOverageMode(
        ContractKmOverageMode? kmOverageMode) =>
        kmOverageMode ?? ContractKmOverageMode.PooledAcrossSlots;

    public static decimal? CalculateDailyValue(
        decimal? monthlyRentalValue,
        int? monthlyWorkingDays,
        ContractType contractType)
    {
        if (!monthlyRentalValue.HasValue || monthlyWorkingDays is not > 0)
            return null;

        return IsMonthlyContract(contractType)
            ? monthlyRentalValue.Value / monthlyWorkingDays.Value
            : null;
    }
}
