using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Admin.Services;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Contracts;

public class ReactivateContractAction
{
    private readonly IContractRepository _contractRepository;
    private readonly IContractStatusHistoryRepository _historyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IContractStateMachine _stateMachine;

    public ReactivateContractAction(
        IContractRepository contractRepository,
        IContractStatusHistoryRepository historyRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IContractStateMachine stateMachine)
    {
        _contractRepository = contractRepository;
        _historyRepository = historyRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _stateMachine = stateMachine;
    }

    public async Task ExecuteAsync(ReactivateContractDto dto)
    {
        var contract = await _contractRepository.GetByIdOrThrowAsync(dto.ContractId);
        _stateMachine.ValidateTransition(contract.Status, ContractStatus.ACTIVE);

        var currentUserId = _currentUserService.GetCurrentUserId()
            ?? throw new UnauthorizedException("auth.user_not_authenticated");

        var previousStatus = contract.Status;
        contract.Status = ContractStatus.ACTIVE;
        contract.DeactivationDate = null;

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            await _historyRepository.AddHistoryAsync(
                contract.Id,
                previousStatus,
                ContractStatus.ACTIVE,
                currentUserId,
                dto.Notes);
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
