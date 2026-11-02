using Microsoft.Extensions.Logging;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Translation;
using LoadlyAggregator.Infrastructure.Services.Localization;

namespace LoadlyAggregator.Infrastructure.Services.Translation;

public class TranslationService : ITranslationService
{
    private readonly ILanguageService _languageService;
    private readonly ILogger<TranslationService> _logger;
    private readonly ResourceCatalogProvider _catalogProvider;

    public TranslationService(
        ILanguageService languageService,
        ILogger<TranslationService> logger,
        ResourceCatalogProvider catalogProvider)
    {
        _languageService = languageService;
        _logger = logger;
        _catalogProvider = catalogProvider;
    }

    public string GetTranslation(string key, params object[] args)
    {
        var language = _languageService.GetCurrentLanguage();
        var catalog = _catalogProvider.GetCatalog();
        var resolver = new TranslationResolver(catalog, language);
        var translations = resolver.GetTranslations();

        if (translations == null)
        {
            _logger.LogWarning(
                "Translations not loaded for language: {Language}, falling back to English. Available languages: {Available}",
                language, catalog.Keys.Count > 0 ? string.Join(", ", catalog.Keys) : "none");
        }

        var translation = resolver.GetTranslation(key);

        if (translation == null && !string.Equals(language, "en", StringComparison.OrdinalIgnoreCase)
            && catalog.TryGetValue("en", out var enDict)
            && enDict.TryGetValue(key, out var enTranslation))
        {
            var enResolver = new TranslationResolver(catalog, "en");
            translation = enResolver.GetTranslation(key);
        }

        if (translation == null)
        {
            _logger.LogWarning(
                "Translation key not found: {Key} for language: {Language}. Available keys: {Available}",
                key, language, translations?.Keys != null ? string.Join(", ", translations.Keys.Take(10)) : "none");
            return key;
        }

        return FormatTranslation(translation, args);
    }

    private static string FormatTranslation(string translation, params object[]? args)
    {
        if (args == null || args.Length == 0)
            return translation;

        try
        {
            return string.Format(translation, args);
        }
        catch
        {
            return translation;
        }
    }

    public string GetTranslationOrDefault(string key, string defaultValue, params object[] args)
    {
        var translation = GetTranslation(key, args);
        return translation == key ? defaultValue : translation;
    }
}
