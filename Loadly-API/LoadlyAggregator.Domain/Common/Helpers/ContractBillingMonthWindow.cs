namespace LoadlyAggregator.Domain.Common.Helpers;

public static class ContractBillingMonthWindow
{
    public static (DateTime periodStartUtc, DateTime periodEndUtc) GetPeriodContaining(
        DateTime contractStartUtc,
        DateTime instantUtc)
    {
        var periodStart = contractStartUtc;

        if (instantUtc < contractStartUtc)
        {
            while (periodStart > instantUtc)
                periodStart = periodStart.AddMonths(-1);
        }
        else
        {
            while (periodStart.AddMonths(1) <= instantUtc)
                periodStart = periodStart.AddMonths(1);
        }

        var periodEnd = periodStart.AddMonths(1);
        return (periodStart, periodEnd);
    }
}
