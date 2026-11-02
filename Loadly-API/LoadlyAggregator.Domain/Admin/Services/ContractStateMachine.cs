using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Services;

/// <summary>
/// Central definition of allowed contract status transitions. All contract actions should use this
/// instead of ad-hoc status checks so business rules stay in one place.
/// </summary>
public sealed class ContractStateMachine : IContractStateMachine
{
    private static readonly HashSet<(ContractStatus From, ContractStatus To)> AllowedTransitions = new()
    {
        (ContractStatus.DRAFT, ContractStatus.DRAFT),
        (ContractStatus.DRAFT, ContractStatus.PENDING_APPROVAL),
        (ContractStatus.PENDING_APPROVAL, ContractStatus.DRAFT),
        (ContractStatus.PENDING_APPROVAL, ContractStatus.ACTIVE),
        (ContractStatus.ACTIVE, ContractStatus.INACTIVE),
        (ContractStatus.ACTIVE, ContractStatus.TERMINATED),
        (ContractStatus.ACTIVE, ContractStatus.RENEWED),
        (ContractStatus.EXPIRED, ContractStatus.RENEWED),
        (ContractStatus.INACTIVE, ContractStatus.ACTIVE),
    };

    public bool CanTransition(ContractStatus from, ContractStatus to) =>
        AllowedTransitions.Contains((from, to));

    public void ValidateTransition(ContractStatus from, ContractStatus to)
    {
        if (!CanTransition(from, to))
            throw new ValidationException("contracts.invalid_status_transition", from, to);
    }
}
