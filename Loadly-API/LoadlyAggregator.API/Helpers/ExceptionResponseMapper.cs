using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Npgsql;
using LoadlyAggregator.API.Constants;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.API.Helpers;

public sealed class ExceptionResponse
{
    public int StatusCode { get; init; }

    public string Message { get; init; } = string.Empty;

    public Dictionary<string, string[]>? Errors { get; init; }
}


public static class ExceptionResponseMapper
{
    private const string FallbackErrorMessage = "An unexpected error occurred";

    public static ExceptionResponse Map(
        Exception exception,
        ITranslationService translationService,
        IHostEnvironment? hostEnvironment = null)
    {
        ArgumentNullException.ThrowIfNull(exception);
        ArgumentNullException.ThrowIfNull(translationService);

        if (exception is DbUpdateException { InnerException: PostgresException postgresEx }
            && TryMapPostgresException(postgresEx, translationService, out var postgresResponse))
        {
            return postgresResponse;
        }

        return exception switch
        {
            NotFoundException ex => Create(
                StatusCodes.Status404NotFound,
                ExceptionMessageTranslator.Translate(ex, translationService, TranslationKeys.Common.NotFound)),

            UnauthorizedException ex => Create(
                StatusCodes.Status401Unauthorized,
                ExceptionMessageTranslator.Translate(ex, translationService, TranslationKeys.Common.Unauthorized)),

            ForbiddenException ex => Create(
                StatusCodes.Status403Forbidden,
                ExceptionMessageTranslator.Translate(ex, translationService, TranslationKeys.Common.Forbidden)),

            ValidationException ex => Create(
                StatusCodes.Status400BadRequest,
                ExceptionMessageTranslator.Translate(ex, translationService, TranslationKeys.Common.ValidationFailed),
                ex.Errors.Count > 0 ? ex.Errors : null),

            LogicalException ex => Create(
                StatusCodes.Status400BadRequest,
                ExceptionMessageTranslator.Translate(ex, translationService, TranslationKeys.Common.BadRequest)),

            TooManyRequestsException ex => Create(
                StatusCodes.Status429TooManyRequests,
                ExceptionMessageTranslator.Translate(ex, translationService, TranslationKeys.Common.Error)),

            _ => MapUnknownException(exception, translationService, hostEnvironment)
        };
    }

    public static bool ShouldReportToRollbar(Exception exception)
    {
        return exception is not NotFoundException
            and not UnauthorizedException
            and not ForbiddenException
            and not ValidationException
            and not LogicalException
            and not TooManyRequestsException
            && !IsKnownDatabaseException(exception);
    }

    private static ExceptionResponse MapUnknownException(
        Exception exception,
        ITranslationService translationService,
        IHostEnvironment? hostEnvironment)
    {
        var baseMessage = TranslationHelper.TranslateMessage(
            TranslationKeys.Common.Error,
            translationService,
            FallbackErrorMessage);

        if (hostEnvironment?.IsDevelopment() == true)
        {
            var detailedMessage = $"{baseMessage}: {exception.GetType().Name} - {exception.Message}";
            if (exception.InnerException != null)
            {
                detailedMessage +=
                    $" | Inner: {exception.InnerException.GetType().Name} - {exception.InnerException.Message}";
            }

            return Create(StatusCodes.Status500InternalServerError, detailedMessage);
        }

        return Create(StatusCodes.Status500InternalServerError, baseMessage);
    }

    private static bool TryMapPostgresException(
        PostgresException postgresEx,
        ITranslationService translationService,
        out ExceptionResponse response)
    {
        response = null!;

        var (statusCode, message) = postgresEx.SqlState switch
        {
            "23505" => (
                StatusCodes.Status422UnprocessableEntity,
                GetDuplicateKeyMessage(postgresEx, translationService)),
            "23503" => (
                StatusCodes.Status422UnprocessableEntity,
                GetForeignKeyViolationMessage(postgresEx, translationService)),
            "23502" => (
                StatusCodes.Status400BadRequest,
                GetNotNullViolationMessage(postgresEx, translationService)),
            "23514" => (
                StatusCodes.Status400BadRequest,
                GetCheckViolationMessage(postgresEx, translationService)),
            _ => (0, string.Empty)
        };

        if (statusCode == 0)
        {
            return false;
        }

        response = Create(statusCode, message);
        return true;
    }

    private static bool IsKnownDatabaseException(Exception exception)
    {
        return exception is DbUpdateException { InnerException: PostgresException postgresEx }
            && postgresEx.SqlState is "23505" or "23503" or "23502" or "23514";
    }

    private static string GetDuplicateKeyMessage(PostgresException postgresEx, ITranslationService translationService)
    {
        var fieldName = ExtractFieldName(postgresEx) ?? "record";
        return TranslationHelper.TranslateMessage(
            "common.duplicate_key",
            translationService,
            $"A record with this {fieldName} is archived. Kindly Contact Your Administrator.",
            fieldName);
    }

    private static string GetForeignKeyViolationMessage(
        PostgresException postgresEx,
        ITranslationService translationService)
    {
        var fallback = postgresEx.MessageText?.Contains("violates foreign key constraint", StringComparison.OrdinalIgnoreCase) == true
            ? "This operation violates referential integrity. The record is being used by other records or references a non-existent record."
            : "This operation cannot be completed because the record is being referenced by other records, or the referenced record does not exist.";

        return TranslationHelper.TranslateMessage("common.foreign_key_violation", translationService, fallback);
    }

    private static string GetNotNullViolationMessage(
        PostgresException postgresEx,
        ITranslationService translationService)
    {
        var columnName = postgresEx.ColumnName ?? "field";
        return TranslationHelper.TranslateMessage(
            "common.not_null_violation",
            translationService,
            $"The {columnName} field is required and cannot be null.",
            columnName);
    }

    private static string GetCheckViolationMessage(
        PostgresException postgresEx,
        ITranslationService translationService)
    {
        var fallback = !string.IsNullOrEmpty(postgresEx.ColumnName)
            ? $"The {postgresEx.ColumnName} field violates the validation rules."
            : $"The data provided violates the validation rules for {postgresEx.ConstraintName ?? "constraint"}.";

        return TranslationHelper.TranslateMessage("common.check_violation", translationService, fallback);
    }

    private static string? ExtractFieldName(PostgresException postgresEx)
    {
        if (!string.IsNullOrEmpty(postgresEx.Detail))
        {
            var match = Regex.Match(
                postgresEx.Detail,
                @"Key \(([^)]+)\)=\([^)]+\) already exists",
                RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return match.Groups[1].Value.Trim();
            }
        }

        if (string.IsNullOrEmpty(postgresEx.ConstraintName))
        {
            return null;
        }

        var parts = postgresEx.ConstraintName.Split('_');
        var ignoredSuffixes = new[] { "key", "idx", "ix", "uq" };

        for (var i = parts.Length - 1; i >= 0; i--)
        {
            var part = parts[i];
            if (!string.IsNullOrEmpty(part)
                && !ignoredSuffixes.Contains(part, StringComparer.OrdinalIgnoreCase))
            {
                return part;
            }
        }

        return null;
    }

    private static ExceptionResponse Create(
        int statusCode,
        string message,
        Dictionary<string, string[]>? errors = null)
    {
        return new ExceptionResponse
        {
            StatusCode = statusCode,
            Message = message,
            Errors = errors
        };
    }
}
