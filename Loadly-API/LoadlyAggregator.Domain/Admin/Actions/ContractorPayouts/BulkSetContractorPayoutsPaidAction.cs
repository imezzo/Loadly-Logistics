using LoadlyAggregator.Domain.Admin.DTOs.ContractorPayouts;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.ContractorPayouts;

public class BulkSetContractorPayoutsPaidAction
{
    private readonly IContractorPayoutRepository _contractorPayoutRepository;
    private readonly IUnitOfWork _unitOfWork;

    public BulkSetContractorPayoutsPaidAction(
        IContractorPayoutRepository contractorPayoutRepository,
        IUnitOfWork unitOfWork)
    {
        _contractorPayoutRepository = contractorPayoutRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(BulkSetContractorPayoutsPaidDto dto)
    {
        var payoutIds = dto.PayoutIds.Distinct().ToList();
        var payouts = (await _contractorPayoutRepository.FindAsync(p => payoutIds.Contains(p.Id))).ToList();

        if (payouts.Count != payoutIds.Count)
        {
            var foundIds = payouts.Select(p => p.Id).ToHashSet();
            var missingId = payoutIds.First(id => !foundIds.Contains(id));
            throw new NotFoundException("contractor_payouts.not_found_with_id", missingId);
        }

        foreach (var payout in payouts)
            ContractorPayoutPaidValidation.EnsureCanMarkPaid(payout);

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            foreach (var payout in payouts)
                payout.Status = ContractorPayoutStatus.Paid;

            await _contractorPayoutRepository.UpdateRangeAsync(payouts);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
