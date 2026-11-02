namespace LoadlyAggregator.Domain.Common.Services.Translation;




public class TranslationResolver
{
    private readonly Dictionary<string, Dictionary<string, string>>? _translations;
    private readonly string _language;
    private readonly string _fallbackLanguage;

    public TranslationResolver(
        Dictionary<string, Dictionary<string, string>>? translations,
        string language,
        string fallbackLanguage = "en")
    {
        _translations = translations;
        _language = language;
        _fallbackLanguage = fallbackLanguage;
    }

    public Dictionary<string, string>? GetTranslations()
    {

        var translations = _translations?.GetValueOrDefault(_language);


        return translations ?? _translations?.GetValueOrDefault(_fallbackLanguage);
    }

    public string? GetTranslation(string key)
    {
        var translations = GetTranslations();
        return translations?.TryGetValue(key, out var translation) == true
            ? translation
            : null;
    }
}

