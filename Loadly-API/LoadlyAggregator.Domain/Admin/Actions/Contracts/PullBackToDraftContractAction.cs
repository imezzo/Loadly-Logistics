using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Admin.Services;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;
using LoadlyAggregator.Shared.Time;
namespace LoadlyAggregator.Domain.Admin.Actions.Contracts;

public class PullBackToDraftContractAction
{
    private readonly IContractRepository _contractRepository;
    private readonly IContractApprovalRepository _approvalRepository;
    private readonly IContractStatusHistoryRepository _historyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IContractStateMachine _stateMachine;
    private readonly TimeProvider _timeProvider;

    public PullBackToDraftContractAction(
        IContractRepository contractRepository,
        IContractApprovalRepository approvalRepository,
        IContractStatusHistoryRepository historyRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IContractStateMachine stateMachine,
        TimeProvider timeProvider)
    {
        _contractRepository = contractRepository;
        _approvalRepository = approvalRepository;
        _historyRepository = historyRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _stateMachine = stateMachine;
        _timeProvider = timeProvider;
    }

    public async Task ExecuteAsync(PullBackToDraftContractDto dto)
    {
        var contract = await _contractRepository.GetByIdOrThrowAsync(dto.ContractId);
        _stateMachine.ValidateTransition(contract.Status, ContractStatus.DRAFT);

        var currentUserId = _currentUserService.GetCurrentUserId()
            ?? throw new UnauthorizedException("auth.user_not_authenticated");

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var pendingApprovals = await _approvalRepository.GetPendingApprovalsByContractIdAsync(dto.ContractId);
            foreach (var approval in pendingApprovals)
            {
                approval.Status = ApprovalStatus.REJECTED;
                approval.ApproverId = currentUserId;
                approval.ActionDate = AppUtcTime.UtcNow(_timeProvider);
                approval.Comments = "Submission cancelled by creator";
            }

            var previousStatus = contract.Status;
            var approvalStage = contract.CurrentApprovalStage;
            contract.Status = ContractStatus.DRAFT;
            contract.CurrentApprovalStage = null;

            await _historyRepository.AddHistoryAsync(
                contract.Id,
                previousStatus,
                ContractStatus.DRAFT,
                currentUserId,
                additionalNotes: null,
                approvalStage);
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
