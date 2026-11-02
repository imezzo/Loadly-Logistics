using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Contracts;

public class DeleteContractBillingSnapshotAction
{
    private readonly IContractBillingSnapshotRepository _snapshotRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteContractBillingSnapshotAction(
        IContractBillingSnapshotRepository snapshotRepository,
        IUnitOfWork unitOfWork)
    {
        _snapshotRepository = snapshotRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(DeleteContractBillingSnapshotDto dto)
    {
        var snapshot = await _snapshotRepository.GetByIdWithDetailsOrThrowAsync(dto.SnapshotId);
        if (ContractBillingSnapshotInvoicingProgress.IsBillingLocked(snapshot.Status))
            throw new ValidationException("validation.delete_contract_billing_snapshot_draft_only");

        ContractBillingSnapshotDeletion.SoftDelete(snapshot);
        await _unitOfWork.SaveChangesAsync();
    }
}
