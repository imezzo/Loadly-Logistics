using LoadlyAggregator.Domain.Admin.DTOs.Leads;
using LoadlyAggregator.Domain.Common.Entities;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface ILeadRepository : IRepository<Lead>
{
    Task<(List<Lead> Leads, int TotalCount)> ListAsync(
        string? search,
        int page,
        int pageSize);

    Task<Lead> GetByIdOrThrowAsync(long id);

    Task<Lead?> GetByPhoneAsync(string phone);

    Task<bool> ExistsByPhoneAsync(string phone, long excludeId);
    /// <summary>Insert or update by phone (conflict key). Restores soft-deleted rows. Returns lead id.</summary>
    Task<long> UpsertByPhoneAsync(string fullName, string phone, string? email, long? addedBy);

    Task<LeadAggregateCounts> GetAggregateCountsAsync(long leadId);

    Task<Dictionary<long, LeadAggregateCounts>> GetAggregateCountsByIdsAsync(IReadOnlyList<long> leadIds);
    /// <summary>Returns the active (non-deleted) lead with the given phone, or null if none exists.</summary>
    Task<Lead?> GetActiveByPhoneAsync(string phone);
}
