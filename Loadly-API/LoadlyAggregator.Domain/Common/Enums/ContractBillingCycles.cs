using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum ContractBillingCycles
{
    [Display("Upfront", "مقدم")]
    Upfront = 1,
    [Display("Monthly", "شهري")]
    Monthly = 2,
    [Display("Quarterly", "ربع سنوي")]
    Quarterly = 3,
    [Display("Weekly", "أسبوعي")]
    Weekly = 4
}
