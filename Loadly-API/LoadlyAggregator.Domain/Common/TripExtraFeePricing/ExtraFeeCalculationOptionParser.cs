using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.TripExtraFeePricing;

/// <summary>
/// Parses contract extra-fee <c>CalculationOption</c> / value / threshold with safe defaults for pricing strategies.
/// </summary>
internal static class ExtraFeeCalculationOptionParser
{
    internal static int GetCalculationOptionOrDefault<TEnum>(int? option, TEnum defaultOption)
        where TEnum : struct, Enum
    {
        var defaultRaw = Convert.ToInt32(defaultOption);
        if (!option.HasValue)
            return defaultRaw;
        return Enum.IsDefined(typeof(TEnum), option.Value) ? option.Value : defaultRaw;
    }

    internal static decimal GetValueOrZero(ContractExtraFee extraFee)
    {
        return extraFee.Value ?? 0m;
    }

    internal static decimal GetThresholdOrZero(ContractExtraFee extraFee)
    {
        return extraFee.Threshold ?? 0;
    }

    internal static TollGateCalculation ToTollGateOption(int? option)
    {
        var raw = GetCalculationOptionOrDefault(option, TollGateCalculation.Included);
        return (TollGateCalculation)raw;
    }

    internal static WeightFeeCalculation ToWeightFeeOption(int? option)
    {
        var raw = GetCalculationOptionOrDefault(option, WeightFeeCalculation.Prorated);
        return (WeightFeeCalculation)raw;
    }

    internal static HelpersFeeCalculation ToHelpersFeeOption(int? option)
    {
        var raw = GetCalculationOptionOrDefault(option, HelpersFeeCalculation.PerTrip);
        return (HelpersFeeCalculation)raw;
    }

    internal static WaitingTimeFeeCalculation ToWaitingTimeOption(int? option)
    {
        var raw = GetCalculationOptionOrDefault(option, WaitingTimeFeeCalculation.ProratedMinutes);
        return (WaitingTimeFeeCalculation)raw;
    }

    internal static LoadingUnloadingCalculation ToLoadingUnloadingOption(int? option)
    {
        var raw = GetCalculationOptionOrDefault(option, LoadingUnloadingCalculation.Included);
        return (LoadingUnloadingCalculation)raw;
    }

    internal static IncompleteReturnCalculation ToIncompleteReturnOption(int? option)
    {
        var raw = GetCalculationOptionOrDefault(option, IncompleteReturnCalculation.NormalTripCostOnly);
        return (IncompleteReturnCalculation)raw;
    }
}
