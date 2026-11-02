using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum ContractBillingCalendarTripRole
{
    [Display("Main", "اساسية")]
    Main = 1,

    [Display("Extra", "اضافية")]
    Extra = 2
}
