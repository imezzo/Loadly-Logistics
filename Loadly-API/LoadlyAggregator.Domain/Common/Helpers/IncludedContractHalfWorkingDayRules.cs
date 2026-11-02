using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class IncludedContractHalfWorkingDayRules
{
    public const decimal AmountFactor = 0.5m;

    public static bool ParticipatesInWorkingDayIndex(ContractSlotDayDisposition disposition) =>
        disposition is ContractSlotDayDisposition.Working or ContractSlotDayDisposition.HalfWorking;

    public static bool IsHalfWorking(ContractSlotDayDisposition disposition) =>
        disposition == ContractSlotDayDisposition.HalfWorking;

    public static decimal ApplyAmountFactor(decimal fullDayAmount, ContractSlotDayDisposition disposition) =>
        IsHalfWorking(disposition)
            ? RoundMoney(fullDayAmount * AmountFactor)
            : fullDayAmount;

    private static decimal RoundMoney(decimal value) =>
        Math.Round(value, 2, MidpointRounding.AwayFromZero);
}
