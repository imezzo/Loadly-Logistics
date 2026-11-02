using Microsoft.Extensions.Logging;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Services.Localization;

namespace LoadlyAggregator.Infrastructure.Services.Notification;

public class NotificationTemplateService : INotificationTemplateService
{
    private const string SectionPrefix = "notifications";

    private readonly ILogger<NotificationTemplateService> _logger;
    private readonly ResourceCatalogProvider _catalogProvider;

    public NotificationTemplateService(
        ILogger<NotificationTemplateService> logger,
        ResourceCatalogProvider catalogProvider)
    {
        _logger = logger;
        _catalogProvider = catalogProvider;
    }

    public Task<string> ResolveAsync(string key, string lang, Dictionary<string, string>? placeholders = null)
    {
        var catalog = _catalogProvider.GetCatalog();
        var qualifiedKey = $"{SectionPrefix}.{key}";

        var normalizedLang = (lang ?? "en").ToLowerInvariant();
        if (normalizedLang != "en" && normalizedLang != "ar")
            normalizedLang = "en";

        string? template = null;
        if (catalog.TryGetValue(normalizedLang, out var langDict))
            langDict.TryGetValue(qualifiedKey, out template);

        if (template == null && catalog.TryGetValue("en", out var enDict))
            enDict.TryGetValue(qualifiedKey, out template);

        if (template == null)
        {
            _logger.LogWarning("Notification template key not found: {Key} for language: {Lang}", key, lang);
            return Task.FromResult(key);
        }

        if (placeholders != null)
        {
            foreach (var kvp in placeholders)
            {
                template = template.Replace("{" + kvp.Key + "}", kvp.Value ?? "", StringComparison.OrdinalIgnoreCase);
            }
        }

        return Task.FromResult(template);
    }
}
