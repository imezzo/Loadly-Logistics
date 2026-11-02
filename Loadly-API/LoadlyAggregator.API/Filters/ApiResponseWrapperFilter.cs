using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using LoadlyAggregator.API.Constants;
using LoadlyAggregator.API.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.API.Filters;

/// <summary>
/// Automatically wraps API responses in ApiResponse&lt;T&gt; structure.
/// - 200 responses: data = value, success = true, errors = null
/// - Non-200 responses: data = null, success = false, errors = extracted if available
/// </summary>
public sealed class ApiResponseWrapperFilter : IResultFilter
{
    private readonly ITranslationService? _translationService;

    #region Constants

    private static readonly BindingFlags PropertyFlags = BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance;

    // Status code constants
    private const int StatusCodeNoContent = 204;
    private const int StatusCodeBadRequest = 400;
    private const int StatusCodeUnauthorized = 401;
    private const int StatusCodeForbidden = 403;
    private const int StatusCodeNotFound = 404;
    private const int StatusCodeInternalServerError = 500;
    private const int StatusCodeSuccessMin = 200;
    private const int StatusCodeSuccessMax = 299;

    // Property names for reflection
    private const string PropertyNameMessage = "message";
    private const string PropertyNameErrors = "errors";
    private const string ControllerSuffix = "controller";
    private const int ControllerSuffixLength = 10;

    #endregion

    #region Static Caches and Lookups

    // Reflection caches for performance
    private static readonly ConcurrentDictionary<Type, PropertyInfo?> MessagePropertyCache = new();
    private static readonly ConcurrentDictionary<Type, PropertyInfo?> ErrorsPropertyCache = new();

    // Action name sets for efficient O(1) lookup
    private static readonly HashSet<string> AuthActions = new(StringComparer.OrdinalIgnoreCase)
    {
        "login", "refreshtoken", "refresh-token",
        "verifymfa", "verify-mfa",
        "forgetpassword", "forget-password",
        "resetpassword", "reset-password",
        "changepassword", "change-password",
        "enablemfa", "enable-mfa",
        "disablemfa", "disable-mfa",
        "sendmfacode", "send-mfa-code",
        "verifyemail", "verify-email",
        "verifyphone", "verify-phone",
        "resendverification", "resend-verification",
        "logout"
    };

    private static readonly HashSet<string> CrudActions = new(StringComparer.OrdinalIgnoreCase)
    {
        "create", "update", "delete", "list", "getall", "get", "activate", "deactivate"
    };

    // Auth action to message key mapping for O(1) lookup
    private static readonly Dictionary<string, string> AuthActionToMessageKey = new(StringComparer.OrdinalIgnoreCase)
    {
        { "login", TranslationKeys.Auth.LoginSuccess },
        { "refreshtoken", TranslationKeys.Auth.LoginSuccess },
        { "refresh-token", TranslationKeys.Auth.LoginSuccess },
        { "verifymfa", TranslationKeys.Auth.MfaVerifySuccess },
        { "verify-mfa", TranslationKeys.Auth.MfaVerifySuccess },
        { "forgetpassword", TranslationKeys.Auth.ForgetPasswordSent },
        { "forget-password", TranslationKeys.Auth.ForgetPasswordSent },
        { "resetpassword", TranslationKeys.Auth.ResetPasswordSuccess },
        { "reset-password", TranslationKeys.Auth.ResetPasswordSuccess },
        { "changepassword", TranslationKeys.Auth.ChangePasswordSuccess },
        { "change-password", TranslationKeys.Auth.ChangePasswordSuccess },
        { "enablemfa", TranslationKeys.Auth.MfaEnabled },
        { "enable-mfa", TranslationKeys.Auth.MfaEnabled },
        { "disablemfa", TranslationKeys.Auth.MfaDisabled },
        { "disable-mfa", TranslationKeys.Auth.MfaDisabled },
        { "sendmfacode", TranslationKeys.Auth.MfaCodeSent },
        { "send-mfa-code", TranslationKeys.Auth.MfaCodeSent },
        { "verifyemail", TranslationKeys.Auth.EmailVerified },
        { "verify-email", TranslationKeys.Auth.EmailVerified },
        { "verifyphone", TranslationKeys.Auth.PhoneVerified },
        { "verify-phone", TranslationKeys.Auth.PhoneVerified },
        { "resendverification", TranslationKeys.Auth.VerificationCodeSent },
        { "resend-verification", TranslationKeys.Auth.VerificationCodeSent },
        { "logout", TranslationKeys.Auth.LogoutSuccess }
    };

    #endregion

    /// <summary>
    /// Initializes a new instance of the ApiResponseWrapperFilter.
    /// </summary>
    /// <param name="translationService">Optional translation service for localized messages.</param>
    public ApiResponseWrapperFilter(ITranslationService? translationService = null)
    {
        _translationService = translationService;
    }

    /// <summary>
    /// Called before the action result is executed.
    /// Wraps the result in ApiResponse format if applicable.
    /// </summary>
    /// <param name="context">The result executing context.</param>
    public void OnResultExecuting(ResultExecutingContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (ShouldSkipProcessing(context))
        {
            return;
        }

        if (TryHandleValidationErrors(context))
        {
            return;
        }

        var objectResult = ConvertToObjectResult(context.Result);
        if (objectResult == null || IsAlreadyWrapped(objectResult.Value))
        {
            return;
        }

        var statusCode = objectResult.StatusCode ?? StatusCodeSuccessMin;
        if (statusCode == StatusCodeNoContent)
        {
            return;
        }

        // Special handling for ListingDto and LookupResponse - flat structure without nested data
        if (IsListingDto(objectResult.Value) || IsLookupResponse(objectResult.Value))
        {
            var flatResponse = CreateFlatListResponse(objectResult.Value!, context);
            context.Result = new ObjectResult(flatResponse) { StatusCode = statusCode };
            return;
        }

        var wrappedResponse = WrapResponse(objectResult.Value, statusCode, context);

        if (wrappedResponse != null)
        {
            context.Result = new ObjectResult(wrappedResponse) { StatusCode = statusCode };
        }
    }

    /// <summary>
    /// Called after the action result is executed.
    /// </summary>
    /// <param name="context">The result executed context.</param>
    public void OnResultExecuted(ResultExecutedContext context)
    {
        // No action needed
    }

    #region Core Logic Methods

    /// <summary>
    /// Determines if the current result should skip wrapper processing.
    /// </summary>
    private static bool ShouldSkipProcessing(ResultExecutingContext context)
    {
        return context.Result is NoContentResult;
    }

    /// <summary>
    /// Handles validation errors from ModelState and wraps them in ApiResponse format.
    /// </summary>
    /// <returns>True if validation errors were handled, false otherwise.</returns>
    private bool TryHandleValidationErrors(ResultExecutingContext context)
    {
        if (context.ModelState.IsValid)
        {
            return false;
        }

        if (context.Result is not BadRequestObjectResult)
        {
            return false;
        }

        var errors = FormatModelStateErrors(context.ModelState, _translationService);
        var message = TranslationHelper.TranslateMessage(
            TranslationKeys.Common.ValidationFailed,
            _translationService);

        var wrappedResponse = ApiResponse<object>.Error(message, errors);
        context.Result = new ObjectResult(wrappedResponse) { StatusCode = StatusCodeBadRequest };
        return true;
    }

    /// <summary>
    /// Converts various IActionResult types to ObjectResult for wrapping.
    /// </summary>
    private static ObjectResult? ConvertToObjectResult(IActionResult result)
    {
        switch (result)
        {
            case BadRequestObjectResult:
                // Already handled above if ModelState invalid
                return null;

            case NotFoundResult:
                return new ObjectResult(null) { StatusCode = StatusCodeNotFound };

            case StatusCodeResult statusResult:
                return new ObjectResult(null) { StatusCode = statusResult.StatusCode };

            case ObjectResult objectResult:
                return objectResult;

            default:
                // Skip FileResult, RedirectResult, etc.
                return null;
        }
    }

    /// <summary>
    /// Checks if a value is already wrapped in ApiResponse format.
    /// </summary>
    private static bool IsAlreadyWrapped(object? value)
    {
        var valueType = value?.GetType();
        if (valueType == null || !valueType.IsGenericType)
        {
            return false;
        }

        return valueType.GetGenericTypeDefinition() == typeof(ApiResponse<>);
    }

    /// <summary>
    /// Checks if a value is a ListingDto that needs special handling.
    /// </summary>
    private static bool IsListingDto(object? value)
    {
        var valueType = value?.GetType();
        if (valueType == null || !valueType.IsGenericType)
        {
            return false;
        }

        return valueType.GetGenericTypeDefinition() == typeof(ListingDto<>);
    }

    /// <summary>
    /// Checks if a value is a LookupResponse that needs the same flat handling as ListingDto.
    /// </summary>
    private static bool IsLookupResponse(object? value)
    {
        return value?.GetType() == typeof(LookupResponse);
    }

    /// <summary>
    /// Wraps a response value in ApiResponse format based on status code.
    /// </summary>
    private ApiResponse<object>? WrapResponse(
        object? value,
        int statusCode,
        ResultExecutingContext context)
    {
        if (IsSuccessStatusCode(statusCode))
        {
            return WrapSuccessResponse(value, context);
        }

        return WrapErrorResponse(value, statusCode);
    }

    /// <summary>
    /// Determines if the status code represents a successful HTTP response (2xx).
    /// </summary>
    private static bool IsSuccessStatusCode(int statusCode)
    {
        return statusCode >= StatusCodeSuccessMin && statusCode <= StatusCodeSuccessMax;
    }

    #endregion

    #region Success Response Methods

    /// <summary>
    /// Wraps a successful response value in ApiResponse format.
    /// </summary>
    private ApiResponse<object> WrapSuccessResponse(
        object? value,
        ResultExecutingContext context)
    {
        object? data = value;
        string responseMessage;

        if (value == null)
        {
            responseMessage = GetMessageFromAction(context);
            return ApiResponse<object>.Create(data!, responseMessage);
        }

        var messageFromValue = TryExtractMessageFromValue(value);
        if (!string.IsNullOrEmpty(messageFromValue))
        {
            responseMessage = TranslationHelper.TranslateMessage(messageFromValue, _translationService);

            // If value only has message property (anonymous object with just message), set data to null
            var valueType = value.GetType();
            var propertyCount = valueType.GetProperties(PropertyFlags).Length;
            if (propertyCount == 1)
            {
                data = null;
            }
        }
        else
        {
            responseMessage = GetMessageFromAction(context);
        }

        return ApiResponse<object>.Create(data!, responseMessage);
    }

    /// <summary>
    /// Creates a flat response for ListingDto or LookupResponse.
    /// Returns: { data: [...], pagination: {...}, additional: {...}, message: "...", success: true, errors: null }
    /// </summary>
    private object CreateFlatListResponse(object listResponse, ResultExecutingContext context)
    {
        var responseType = listResponse.GetType();

        var dataProperty = responseType.GetProperty("Data");
        var paginationProperty = responseType.GetProperty("Pagination");
        var additionalProperty = responseType.GetProperty("Additional");

        var data = dataProperty?.GetValue(listResponse);
        var pagination = paginationProperty?.GetValue(listResponse);
        var additional = additionalProperty?.GetValue(listResponse);

        var message = GetMessageFromAction(context);

        return new
        {
            data = data,
            pagination = pagination,
            additional = additional,
            message = message,
            success = true,
            errors = (Dictionary<string, string[]>?)null
        };
    }

    /// <summary>
    /// Attempts to extract a message property from an object value.
    /// </summary>
    private static string? TryExtractMessageFromValue(object value)
    {
        var valueType = value.GetType();
        var messageProperty = GetMessageProperty(valueType);
        var messageValue = messageProperty?.GetValue(value)?.ToString();

        return string.IsNullOrWhiteSpace(messageValue) ? null : messageValue;
    }

    /// <summary>
    /// Derives an appropriate success message based on action and controller names.
    /// </summary>
    private string GetMessageFromAction(ResultExecutingContext context)
    {
        var actionName = context.RouteData.Values["action"]?.ToString() ?? string.Empty;
        var controllerName = GetControllerName(context);
        var messageKey = GetSuccessMessageKey(actionName, controllerName);
        return TranslationHelper.TranslateMessage(messageKey, _translationService);
    }

    /// <summary>
    /// Gets the appropriate success message key for an action.
    /// </summary>
    private static string GetSuccessMessageKey(string actionName, string controllerName)
    {
        // Handle auth-specific actions
        if (AuthActions.Contains(actionName))
        {
            return AuthActionToMessageKey.TryGetValue(actionName, out var messageKey)
                ? messageKey
                : TranslationKeys.Common.Success;
        }

        // Handle CRUD operations - derive from controller name
        if (CrudActions.Contains(actionName))
        {
            return GetCrudMessageKey(actionName, controllerName);
        }

        return TranslationKeys.Common.Success;
    }

    /// <summary>
    /// Gets the CRUD message key based on action and controller names.
    /// </summary>
    private static string GetCrudMessageKey(string actionName, string controllerName)
    {
        switch (actionName)
        {
            case "create":
                return $"{controllerName}.created_successfully";

            case "update":
                return $"{controllerName}.updated_successfully";

            case "delete":
                return $"{controllerName}.deleted_successfully";

            case "list":
            case "getall":
            case "get":
                return $"{controllerName}.retrieved_successfully";

            case "activate":
                return $"{controllerName}.activated_successfully";

            case "deactivate":
                return $"{controllerName}.deactivated_successfully";

            default:
                return TranslationKeys.Common.Success;
        }
    }

    #endregion

    #region Error Response Methods

    /// <summary>
    /// Wraps an error response value in ApiResponse format.
    /// </summary>
    private ApiResponse<object> WrapErrorResponse(
        object? errorValue,
        int statusCode)
    {
        var (message, errors) = ExtractErrorInfo(errorValue, statusCode);
        return ApiResponse<object>.Error(message, errors);
    }

    /// <summary>
    /// Extracts error message and error dictionary from an error object.
    /// </summary>
    private (string message, Dictionary<string, string[]>? errors) ExtractErrorInfo(
        object? errorValue,
        int statusCode)
    {
        if (errorValue == null)
        {
            var defaultMessage = GetDefaultErrorMessage(statusCode);
            return (defaultMessage, null);
        }

        var errorMessage = ExtractMessageFromErrorObject(errorValue, errorValue.GetType(), statusCode);
        var errorDictionary = ExtractErrorsFromErrorObject(errorValue, errorValue.GetType());
        return (errorMessage, errorDictionary);
    }

    /// <summary>
    /// Extracts the message property from an error object.
    /// </summary>
    private string ExtractMessageFromErrorObject(
        object errorValue,
        Type errorType,
        int statusCode)
    {
        var messageProperty = GetMessageProperty(errorType);
        var messageValue = messageProperty?.GetValue(errorValue)?.ToString();

        if (!string.IsNullOrWhiteSpace(messageValue))
        {
            return TranslationHelper.TranslateMessage(messageValue, _translationService);
        }

        if (errorValue is string stringValue)
        {
            return TranslationHelper.TranslateMessage(stringValue, _translationService);
        }

        return GetDefaultErrorMessage(statusCode);
    }

    /// <summary>
    /// Extracts the errors dictionary from an error object.
    /// </summary>
    private static Dictionary<string, string[]>? ExtractErrorsFromErrorObject(object errorValue, Type errorType)
    {
        var errorsProperty = GetErrorsProperty(errorType);
        return errorsProperty?.GetValue(errorValue) as Dictionary<string, string[]>;
    }

    /// <summary>
    /// Gets the default error message for a status code.
    /// </summary>
    private string GetDefaultErrorMessage(int statusCode)
    {
        var messageKey = GetErrorMessageKey(statusCode);
        return TranslationHelper.TranslateMessage(messageKey, _translationService);
    }

    /// <summary>
    /// Maps status codes to translation keys.
    /// </summary>
    private static string GetErrorMessageKey(int statusCode)
    {
        return statusCode switch
        {
            StatusCodeBadRequest => TranslationKeys.Common.Error,
            StatusCodeForbidden => TranslationKeys.Common.Error,
            StatusCodeInternalServerError => TranslationKeys.Common.Error,
            StatusCodeUnauthorized => TranslationKeys.Common.Unauthorized,
            StatusCodeNotFound => TranslationKeys.Common.NotFound,
            _ => TranslationKeys.Common.Error
        };
    }

    #endregion

    #region Validation Error Formatting

    /// <summary>
    /// Formats ModelState validation errors into Dictionary&lt;string, string[]&gt; with camelCase keys.
    /// Filters out empty error messages and ensures each field has at least one error.
    /// </summary>
    private static Dictionary<string, string[]> FormatModelStateErrors(
        Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary modelState,
        ITranslationService? translationService)
    {
        ArgumentNullException.ThrowIfNull(modelState);

        var errors = new Dictionary<string, string[]>();

        foreach (var keyValuePair in modelState)
        {
            var fieldName = keyValuePair.Key;
            var errorMessages = ExtractErrorMessages(keyValuePair.Value);

            if (errorMessages.Length == 0)
            {
                continue;
            }

            var camelCaseFieldName = ToCamelCase(fieldName);
            errors[camelCaseFieldName] = errorMessages
                .Select(msg => TranslationHelper.TranslateModelStateMessage(msg, translationService))
                .ToArray();
        }

        return errors;
    }

    /// <summary>
    /// Extracts error messages from a ModelState entry.
    /// </summary>
    private static string[] ExtractErrorMessages(
        Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateEntry? modelStateEntry)
    {
        if (modelStateEntry == null)
        {
            return Array.Empty<string>();
        }

        return modelStateEntry.Errors
            .Select(e => e.ErrorMessage)
            .Where(msg => !string.IsNullOrWhiteSpace(msg))
            .ToArray();
    }

    /// <summary>
    /// Converts a field name to camelCase format.
    /// </summary>
    private static string ToCamelCase(string fieldName)
    {
        if (string.IsNullOrEmpty(fieldName))
        {
            return fieldName;
        }

        // Handle single character case
        if (fieldName.Length == 1)
        {
            return fieldName.ToLowerInvariant();
        }

        return char.ToLowerInvariant(fieldName[0]) + fieldName.Substring(1);
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Extracts and normalizes the controller name from route data.
    /// </summary>
    private static string GetControllerName(ResultExecutingContext context)
    {
        var controllerName = context.RouteData.Values["controller"]?.ToString() ?? string.Empty;

        if (string.IsNullOrEmpty(controllerName))
        {
            return string.Empty;
        }

        // Remove "Controller" suffix if present (case-insensitive)
        if (controllerName.EndsWith(ControllerSuffix, StringComparison.OrdinalIgnoreCase))
        {
            controllerName = controllerName[..^ControllerSuffixLength];
        }

        // Normalize hyphens to underscores to match JSON translation keys
        // e.g., "company-documents" → "company_documents"
        return controllerName.Replace("-", "_", StringComparison.Ordinal);
    }

    #endregion

    #region Property Reflection Methods

    /// <summary>
    /// Gets the "message" property from a type using reflection cache.
    /// </summary>
    private static PropertyInfo? GetMessageProperty(Type type)
    {
        // Caching is an implementation detail - improves performance by avoiding repeated reflection
        return MessagePropertyCache.GetOrAdd(type, t =>
            t.GetProperty(PropertyNameMessage, PropertyFlags));
    }

    /// <summary>
    /// Gets the "errors" property from a type using reflection cache.
    /// </summary>
    private static PropertyInfo? GetErrorsProperty(Type type)
    {
        // Caching is an implementation detail - improves performance by avoiding repeated reflection
        return ErrorsPropertyCache.GetOrAdd(type, t =>
            t.GetProperty(PropertyNameErrors, PropertyFlags));
    }

    #endregion
}
