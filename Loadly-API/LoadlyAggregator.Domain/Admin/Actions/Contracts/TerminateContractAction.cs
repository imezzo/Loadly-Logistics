using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Admin.Helpers;
using LoadlyAggregator.Domain.Admin.Services;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Contracts;

/// <summary>
/// Terminates a contract. Valid only when the contract is in <see cref="ContractStatus.ACTIVE"/> status.
/// Archives active contract slots (preserving <c>vehicle_id</c> for history) and clears
/// <c>Vehicle.ContractId</c> so vehicles can be assigned to another contract.
/// </summary>
public class TerminateContractAction
{
    private readonly IContractRepository _contractRepository;
    private readonly IContractSlotRepository _contractSlotRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IContractStatusHistoryRepository _historyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IContractStateMachine _stateMachine;

    public TerminateContractAction(
        IContractRepository contractRepository,
        IContractSlotRepository contractSlotRepository,
        IVehicleRepository vehicleRepository,
        IContractStatusHistoryRepository historyRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IContractStateMachine stateMachine)
    {
        _contractRepository = contractRepository;
        _contractSlotRepository = contractSlotRepository;
        _vehicleRepository = vehicleRepository;
        _historyRepository = historyRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _stateMachine = stateMachine;
    }

    public async Task ExecuteAsync(TerminateContractDto dto)
    {
        var contract = await _contractRepository.GetByIdOrThrowAsync(dto.ContractId);

        _stateMachine.ValidateTransition(contract.Status, ContractStatus.TERMINATED);

        var currentUserId = _currentUserService.GetCurrentUserId()
            ?? throw new UnauthorizedException("auth.user_not_authenticated");

        var previousStatus = contract.Status;
        contract.Status = ContractStatus.TERMINATED;

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            await ArchiveActiveSlotsAsync(contract.Id);
            await _vehicleRepository.ClearContractAssignmentByContractIdAsync(contract.Id);
            await _historyRepository.AddHistoryAsync(
                contract.Id,
                previousStatus,
                ContractStatus.TERMINATED,
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

    private async Task ArchiveActiveSlotsAsync(long contractId)
    {
        var slots = await _contractSlotRepository.GetByContractIdAsync(contractId);
        foreach (var slot in slots.Where(s => s.Status == ContractSlotStatus.ACTIVE))
        {
            ContractSlotPersistenceHelper.MarkArchived(slot);
            _contractSlotRepository.Update(slot);
        }
    }
}
