using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Admin.Services;

public interface IContractStateMachine
{
    /// <summary>
    /// Returns true if transitioning from <paramref name="from"/> to <paramref name="to"/> is allowed.
    /// </summary>
    bool CanTransition(ContractStatus from, ContractStatus to);

    /// <summary>
    /// Throws if the transition is not allowed. Use before applying a status change.
    /// </summary>
    void ValidateTransition(ContractStatus from, ContractStatus to);
}
