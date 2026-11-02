namespace LoadlyAggregator.Domain.Admin.Services;

public interface ICollectorNotificationService
{
    Task SendCollectorWelcomeSmsAsync(Common.Entities.Collector collector, string setPasswordUrl);
}
