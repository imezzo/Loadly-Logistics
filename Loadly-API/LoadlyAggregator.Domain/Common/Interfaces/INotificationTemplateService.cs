namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface INotificationTemplateService
{
    Task<string> ResolveAsync(string key, string lang, Dictionary<string, string>? placeholders = null);
}
