using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class ContractorPayoutPaidValidation
{
    public static void EnsureCanMarkPaid(ContractorPayout payout)
    {
        if (payout.Status == ContractorPayoutStatus.Paid)
            throw new ValidationException("contractor_payouts.cannot_update_paid_payout");

        if (payout.Status == ContractorPayoutStatus.Cancelled)
            throw new ValidationException("contractor_payouts.payout_already_cancelled");
    }
}
