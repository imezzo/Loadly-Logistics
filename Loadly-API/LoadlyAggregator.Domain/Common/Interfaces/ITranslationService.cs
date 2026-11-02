namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface ITranslationService
{
    string GetTranslation(string key, params object[] args);
    string GetTranslationOrDefault(string key, string defaultValue, params object[] args);
}
