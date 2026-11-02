using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Helpers;

/// <summary>
/// Provides centralized translation functionality with fallback support.
/// Used by filters to translate message keys into localized strings.
/// </summary>
public static class TranslationHelper
{
    private const string TranslationKeyCommonError = "common.error";
    private const string TranslationKeyCommonSuccess = "common.success";
    private const string TranslationKeyCommonBadRequest = "common.bad_request";
    private const string TranslationKeyCommonForbidden = "common.forbidden";
    private const string TranslationKeyCommonUnauthorized = "common.unauthorized";
    private const string TranslationKeyCommonNotFound = "common.not_found";
    private const string TranslationKeyCommonValidationFailed = "common.validation_failed";

    /// <summary>
    /// Translates a message key using the translation service.
    /// - If key contains '.', treats as translation key and attempts translation
    /// - If translation not found or service unavailable, returns fallback
    /// - If key does not contain '.', returns the key as-is (plain text message)
    /// </summary>
    /// <param name="messageKey">The message key or plain text to translate.</param>
    /// <param name="translationService">The translation service (optional).</param>
    /// <param name="customFallback">Custom fallback message if translation not found (optional).</param>
    /// <returns>Translated message, fallback, or original key.</returns>
    /// <summary>
    /// Translates a ModelState / FluentValidation error string.
    /// Supports optional format arguments after the first <c>|</c>:
    /// <c>validation.must_be_between|1|10</c> → key <c>validation.must_be_between</c> with args 1 and 10.
    /// </summary>
    public static string TranslateModelStateMessage(string message, ITranslationService? translationService)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return message;
        }

        if (translationService == null)
        {
            return message;
        }

        var pipeIndex = message.IndexOf('|');
        if (pipeIndex < 0)
        {
            return TranslateMessage(message, translationService, message);
        }

        var key = message[..pipeIndex];
        if (!key.Contains('.', StringComparison.Ordinal))
        {
            return message;
        }

        var argsPart = message[(pipeIndex + 1)..];
        if (string.IsNullOrEmpty(argsPart))
        {
            return TranslateMessage(key, translationService, message);
        }

        var args = argsPart.Split('|');
        return TranslateMessage(key, translationService, message, args);
    }

    public static string TranslateMessage(
        string messageKey,
        ITranslationService? translationService,
        string? customFallback = null,
        params object[] formatArgs)
    {
        if (string.IsNullOrWhiteSpace(messageKey))
        {
            return customFallback ?? string.Empty;
        }

        if (translationService == null)
        {
            return customFallback ?? messageKey;
        }

        // If message doesn't contain '.', it's plain text, not a translation key
        if (!messageKey.Contains('.', StringComparison.Ordinal))
        {
            return messageKey;
        }

        if (formatArgs is { Length: > 0 })
        {
            return translationService.GetTranslationOrDefault(
                messageKey,
                customFallback ?? messageKey,
                formatArgs);
        }

        var translated = translationService.GetTranslation(messageKey);

        // If translation service returns the key itself, translation was not found
        if (string.Equals(translated, messageKey, StringComparison.Ordinal))
        {
            return customFallback ?? GetDefaultFallback(messageKey);
        }

        return translated;
    }

    /// <summary>
    /// Provides default fallback messages for known translation keys.
    /// </summary>
    /// <param name="translationKey">The translation key.</param>
    /// <returns>User-friendly fallback message.</returns>
    private static string GetDefaultFallback(string translationKey)
    {
        if (IsKnownTranslationKey(translationKey, out var fallback))
        {
            return fallback;
        }

        // Handle auth.* keys
        if (translationKey.StartsWith("auth.", StringComparison.Ordinal))
        {
            return "Authentication operation completed";
        }

        // Handle common.* keys
        if (translationKey.StartsWith("common.", StringComparison.Ordinal))
        {
            return "Operation completed";
        }

        // Generic fallback: convert key to readable text
        // e.g., "users.created_successfully" -> "created successfully"
        if (translationKey.Contains('.', StringComparison.Ordinal))
        {
            var parts = translationKey.Split('.');
            var lastPart = parts[parts.Length - 1];
            return lastPart.Replace("_", " ", StringComparison.Ordinal);
        }

        return translationKey;
    }

    /// <summary>
    /// Checks if a translation key is known and provides its fallback message.
    /// </summary>
    private static bool IsKnownTranslationKey(string translationKey, out string fallback)
    {
        switch (translationKey)
        {
            case TranslationKeyCommonBadRequest:
                fallback = "Bad request";
                return true;

            case TranslationKeyCommonForbidden:
                fallback = "Forbidden";
                return true;

            case TranslationKeyCommonError:
                fallback = "An error occurred";
                return true;

            case TranslationKeyCommonSuccess:
                fallback = "Success";
                return true;

            case TranslationKeyCommonNotFound:
                fallback = "Resource not found";
                return true;

            case TranslationKeyCommonUnauthorized:
                fallback = "Unauthorized access";
                return true;

            case TranslationKeyCommonValidationFailed:
                fallback = "One or more validation errors occurred";
                return true;

            default:
                fallback = string.Empty;
                return false;
        }
    }
}
