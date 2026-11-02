namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IBroadcastNotificationRepository : IRepository<Common.Entities.BroadcastNotification>
{
    Task<(List<Common.Entities.BroadcastNotification> Items, int TotalCount)> ListAsync(
        string? search,
        int page,
        int pageSize);

    Task<Common.Entities.BroadcastNotification?> GetByPublicTokenAsync(string publicToken);
}
