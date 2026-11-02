namespace LoadlyAggregator.Domain.Common.DTOs.Notifications;

public class BatchMarkNotificationsReadDto
{
    public IReadOnlyList<long> Ids { get; set; } = [];
}
