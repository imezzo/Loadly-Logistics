using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Admin.Services;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;
using LoadlyAggregator.Shared.Time;

namespace LoadlyAggregator.Domain.Admin.Actions.ContractApprovals;

public class ApproveContractApprovalAction
{
    private readonly IContractApprovalRepository _approvalRepository;
    private readonly IContractStatusHistoryRepository _historyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IContractStateMachine _stateMachine;
    private readonly TimeProvider _timeProvider;

    public ApproveContractApprovalAction(
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
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            // Prevent approving contracts whose date range has already fully expired at ANY stage
            ValidateContractNotExpiredForApproval(contract);

            var hasAlreadyApproved = await _approvalRepository.HasUserAlreadyApprovedAtOtherStageAsync(dto.ContractId, currentUserId, currentApproval.Id);
            if (hasAlreadyApproved)
                throw new ValidationException("contracts.already_approved_at_previous_stage");

            currentApproval.Status = ApprovalStatus.APPROVED;
            currentApproval.ApproverId = currentUserId;
            currentApproval.Comments = dto.Comments;
            currentApproval.ActionDate = AppUtcTime.UtcNow(_timeProvider);

            var nextApproval = await _approvalRepository.GetNextPendingApprovalAsync(dto.ContractId, currentApproval.StageOrder);
            var isFullyApproved = nextApproval is null;

            if (isFullyApproved)
            {
                _stateMachine.ValidateTransition(contract.Status, ContractStatus.ACTIVE);
                ValidateContractDatesForActivation(contract);
                await ApplyFullyApprovedAsync(contract, currentUserId, currentApproval.Comments, currentApproval.Stage);
            }
            else
            {
                await ApplyStageApprovedAsync(contract, currentApproval, nextApproval!, currentUserId, currentApproval.Comments);
            }

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    private void ValidateContractNotExpiredForApproval(Contract contract)
    {
        DateTime todayUtcMidnight = AppUtcTime.UtcTodayAtMidnight(_timeProvider);
        if (contract.EndDate < todayUtcMidnight)
            throw new ValidationException("contracts.cannot_approve_expired");
    }

    private static void ValidateContractDatesForActivation(Contract contract)
    {
        if (contract.EndDate <= contract.StartDate)
            throw new ValidationException("contracts.end_date_must_be_after_start");
    }

    private async Task ApplyFullyApprovedAsync(Contract contract, long currentUserId, string? comments, ApprovalStage stageJustApproved)
    {
        var previousStatus = contract.Status;
        contract.Status = ContractStatus.ACTIVE;
        contract.CurrentApprovalStage = null;
        await _historyRepository.AddHistoryAsync(
            contract.Id,
            previousStatus,
            ContractStatus.ACTIVE,
            currentUserId,
            comments,
            stageJustApproved);
    }

    private async Task ApplyStageApprovedAsync(Contract contract, ContractApproval currentApproval, ContractApproval nextApproval, long currentUserId, string? comments)
    {
        contract.CurrentApprovalStage = nextApproval.Stage;
        await _historyRepository.AddHistoryAsync(
            contract.Id,
            contract.Status,
            contract.Status,
            currentUserId,
            comments,
            currentApproval.Stage);
    }
}
