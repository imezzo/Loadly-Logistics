using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Shared.Time;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class TripFinancialDateResolver
{
    /// <summary>
    /// Monthly included and excluded contracts keep the submitted financial date.
    /// Every other contract uses the Egypt calendar date of the trip start.
    /// </summary>
    public static DateOnly? Resolve(Contract? contract, DateTime startDate, DateOnly? submittedFinancialDate) =>
        contract is not null && ContractCalculationHelper.IsMonthlyContract(contract.ContractType)
            ? submittedFinancialDate
            : EgyptTime.ToCalendarDate(startDate);
}
