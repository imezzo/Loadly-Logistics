using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Services;

public class CollectorNotificationService : ICollectorNotificationService
{
    private readonly INotificationTransport _notificationTransport;

    public CollectorNotificationService(INotificationTransport notificationTransport)
    {
        _notificationTransport = notificationTransport;
    }

    public async Task SendCollectorWelcomeSmsAsync(Common.Entities.Collector collector, string setPasswordUrl)
    {
        var message = $"Welcome to Loadly!\n{setPasswordUrl}";

        await _notificationTransport.SendSmsAsync(
            collector.Phone,
            message,
            NotificationDomain.Collector,
            collector.Id,
            "Collector");
    }
}
