namespace LoadlyAggregator.Domain.Admin.Services;

public interface IAdminNotificationService
{
    Task SendAdminWelcomeSmsAsync(Common.Entities.Admin admin, string setPasswordUrl);
}
