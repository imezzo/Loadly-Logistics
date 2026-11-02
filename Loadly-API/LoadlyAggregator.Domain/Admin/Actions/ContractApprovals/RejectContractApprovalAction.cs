using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Admin.Services;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Time;

namespace LoadlyAggregator.Domain.Admin.Actions.ContractApprovals;

public class RejectContractApprovalAction
{
    private readonly IContractApprovalRepository _approvalRepository;
    private readonly IContractStatusHistoryRepository _historyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IContractStateMachine _stateMachine;
    private readonly TimeProvider _timeProvider;

    public RejectContractApprovalAction(
        IContractApprovalRepository approvalRepository,
        IContractStatusHistoryRepository historyRepository,
        IUnitOfWork unitOfWork,
        IContractStateMachine stateMachine,
        TimeProvider timeProvider)
    {
        _approvalRepository = approvalRepository;
        _historyRepository = historyRepository;
        _unitOfWork = unitOfWork;
        _stateMachine = stateMachine;
        _timeProvider = timeProvider;
    }

    public async Task ExecuteAsync(Contract contract, ContractApproval currentApproval, long currentUserId, ContractApprovalDecisionDto dto)
    {
        _stateMachine.ValidateTransition(contract.Status, ContractStatus.DRAFT);
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            currentApproval.Status = ApprovalStatus.REJECTED;
            currentApproval.ApproverId = currentUserId;
            currentApproval.Comments = dto.Comments;
            currentApproval.ActionDate = AppUtcTime.UtcNow(_timeProvider);

            var otherPendingApprovals = await _approvalRepository.GetOtherPendingApprovalsAsync(dto.ContractId, currentApproval.Id);
            foreach (var approval in otherPendingApprovals)
                approval.Status = ApprovalStatus.REJECTED;

            var previousStatus = contract.Status;
            contract.Status = ContractStatus.DRAFT;
            contract.CurrentApprovalStage = null;

            await _historyRepository.AddHistoryAsync(
                contract.Id,
                previousStatus,
                ContractStatus.DRAFT,
                currentUserId,
                dto.Comments,
                currentApproval.Stage);

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
