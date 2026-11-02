using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Services;

public class AdminNotificationService : IAdminNotificationService
{
    private readonly INotificationTransport _notificationTransport;

    public AdminNotificationService(INotificationTransport notificationTransport)
    {
        _notificationTransport = notificationTransport;
    }

    public async Task SendAdminWelcomeSmsAsync(Common.Entities.Admin admin, string setPasswordUrl)
    {
        var message = $"Welcome to Loadly!\n{setPasswordUrl}";

        await _notificationTransport.SendSmsAsync(
            admin.Phone,
            message,
            NotificationDomain.Admin,
            admin.Id,
            "Admin");
    }
}
