namespace LoadlyAggregator.Domain.Common.Interfaces;

/// <summary>
/// Resolves user (model) IDs, contacts, and display names for broadcast delivery by audience role (admin, company, driver).
/// </summary>
public interface IBroadcastNotificationTargetResolver
{
    Task<List<long>> GetUserIdsAsync(string targetRole);
    Task<IReadOnlyList<long>> GetUserIdsPageAsync(string targetRole, int skip, int take);
    Task<IReadOnlyList<BroadcastContact>> GetUserContactsPageAsync(string targetRole, int skip, int take);
    Task<IReadOnlyList<BroadcastContact>> GetUserContactsByIdsAsync(string targetRole, IReadOnlyList<long> userIds);
    Task<IReadOnlyDictionary<long, string>> GetUserNamesByIdsAsync(string targetRole, IReadOnlyList<long> userIds);
}
