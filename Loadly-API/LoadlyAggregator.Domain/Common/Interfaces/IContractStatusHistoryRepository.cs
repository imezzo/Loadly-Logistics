using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IContractStatusHistoryRepository : IRepository<ContractStatusHistory>
{
    Task<List<ContractStatusHistory>> GetHistoryByContractIdAsync(long contractId);
    Task AddHistoryAsync(long contractId, ContractStatus previousStatus, ContractStatus newStatus, long? changedBy, string? additionalNotes = null, ApprovalStage? approvalStage = null);
}
