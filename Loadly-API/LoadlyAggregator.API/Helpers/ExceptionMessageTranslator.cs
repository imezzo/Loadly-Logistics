using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.API.Helpers;

public static class ExceptionMessageTranslator
{
    private const string FallbackErrorMessage = "An unexpected error occurred";

    public static string Translate(
        Exception exception,
        ITranslationService? translationService,
        string? defaultTranslationKey = null)
    {
        ArgumentNullException.ThrowIfNull(exception);

        if (exception is ILocalizedException localizedException)
        {
            var messageKey = localizedException.MessageKey;
            if (!string.IsNullOrWhiteSpace(messageKey))
            {
                var fallback = defaultTranslationKey ?? messageKey;
                return localizedException.Args is { Length: > 0 }
                    ? TranslationHelper.TranslateMessage(messageKey, translationService, fallback, localizedException.Args)
                    : TranslationHelper.TranslateMessage(messageKey, translationService, fallback);
            }
        }

        var exceptionMessage = exception.Message;
        if (string.IsNullOrWhiteSpace(exceptionMessage))
        {
            return defaultTranslationKey != null
                ? TranslationHelper.TranslateMessage(defaultTranslationKey, translationService, FallbackErrorMessage)
                : FallbackErrorMessage;
        }

        if (!exceptionMessage.Contains('.', StringComparison.Ordinal))
        {
            return exceptionMessage;
        }

        return TranslationHelper.TranslateMessage(exceptionMessage, translationService, exceptionMessage);
    }
}
