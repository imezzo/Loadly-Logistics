using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.ContractorPayouts;

public class SetContractorPayoutPaidAction
{
    private readonly IContractorPayoutRepository _contractorPayoutRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SetContractorPayoutPaidAction(
        IContractorPayoutRepository contractorPayoutRepository,
        IUnitOfWork unitOfWork)
    {
        _contractorPayoutRepository = contractorPayoutRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(long id)
    {
        var payout = await _contractorPayoutRepository.GetByIdOrThrowAsync(id);

        ContractorPayoutPaidValidation.EnsureCanMarkPaid(payout);

        payout.Status = ContractorPayoutStatus.Paid;
        _contractorPayoutRepository.Update(payout);
        await _unitOfWork.SaveChangesAsync();
    }
}
