namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface ILanguageService
{
    string GetCurrentLanguage();
    bool IsArabic();
    bool IsEnglish();
}
