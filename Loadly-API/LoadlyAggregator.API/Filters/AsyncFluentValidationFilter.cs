using System.Collections.Concurrent;
using System.Reflection;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LoadlyAggregator.API.Filters;

/// <summary>
/// Runs FluentValidation validators asynchronously (ValidateAsync) so validators with async rules
/// (e.g. IsUnique, MustAsync) work without moving validation into actions.
/// Also applies route/action parameter values (id, roleId, permissionId, etc.) onto [FromBody]
/// request models before validation so uniqueness checks that exclude the current record work.
/// </summary>
public sealed class AsyncFluentValidationFilter : IAsyncActionFilter
{
    private static readonly ConcurrentDictionary<Type, MethodInfo?> ValidateAsyncMethodCache = new();
    private static readonly ConcurrentDictionary<(Type ModelType, string ParamName), PropertyInfo?> RoutePropertyCache = new();
    private static readonly BindingFlags PropertyLookupFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.FlattenHierarchy;

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(next);

        ApplyRouteIdsToRequestModels(context);

        foreach (var arg in context.ActionArguments.Values)
        {
            if (arg == null || !IsRequestModel(arg)) continue;

            var modelType = arg.GetType();
            var validatorType = typeof(IValidator<>).MakeGenericType(modelType);
            var validator = context.HttpContext.RequestServices.GetService(validatorType);
            if (validator == null) continue;

            var result = await InvokeValidateAsync(validator, validatorType, arg, context.HttpContext.RequestAborted);
            if (result == null || result.IsValid) continue;

            foreach (var err in result.Errors)
                context.ModelState.AddModelError(err.PropertyName, err.ErrorMessage);

            context.Result = new BadRequestObjectResult((object?)null);
            return;
        }

        await next();
    }

    /// <summary>
    /// Copies route/action parameter values (id, roleId, permissionId, modelId, modelType) onto
    /// [FromBody] request models before validation so uniqueness validators see the correct Id.
    /// </summary>
    private static void ApplyRouteIdsToRequestModels(ActionExecutingContext context)
    {
        var args = context.ActionArguments;
        if (args == null || args.Count == 0) return;

        foreach (var (paramName, value) in args)
        {
            if (value == null || !IsRequestModel(value)) continue;

            var modelType = value.GetType();
            foreach (var (routeParamName, routeValue) in args)
            {
                if (routeParamName == paramName || routeValue == null || !IsRouteParameter(routeValue)) continue;
                TrySetRouteValueOnModel(value, modelType, routeParamName, routeValue);
            }
        }
    }

    private static bool IsRequestModel(object value)
    {
        var t = value.GetType();
        return !t.IsPrimitive && !t.IsEnum && t != typeof(string) && t != typeof(decimal);
    }

    private static bool IsRouteParameter(object value)
    {
        var t = value.GetType();
        return t == typeof(long) || t == typeof(string);
    }

    /// <summary>
    /// Sets the model property matching the route parameter name if it is still at default value.
    /// Uses cached PropertyInfo lookups for robustness and performance.
    /// </summary>
    private static void TrySetRouteValueOnModel(object model, Type modelType, string paramName, object routeValue)
    {
        var prop = GetWritableProperty(modelType, paramName);
        if (prop == null) return;

        if (routeValue is long longValue)
        {
            if (prop.PropertyType != typeof(long)) return;
            var current = (long)(prop.GetValue(model) ?? 0L);
            if (current != 0) return;
            prop.SetValue(model, longValue);
            return;
        }

        if (routeValue is string stringValue)
        {
            if (prop.PropertyType != typeof(string)) return;
            var current = prop.GetValue(model) as string;
            if (!string.IsNullOrEmpty(current)) return;
            prop.SetValue(model, stringValue);
        }
    }

    private static PropertyInfo? GetWritableProperty(Type modelType, string paramName)
    {
        var key = (modelType, paramName);
        return RoutePropertyCache.GetOrAdd(key, _ =>
        {
            var prop = modelType.GetProperty(paramName, PropertyLookupFlags);
            return prop != null && prop.CanWrite ? prop : null;
        });
    }

    private static async Task<FluentValidation.Results.ValidationResult?> InvokeValidateAsync(
        object validator,
        Type validatorType,
        object model,
        CancellationToken cancellationToken)
    {
        var method = ValidateAsyncMethodCache.GetOrAdd(validatorType, t =>
            t.GetMethod("ValidateAsync", [t.GenericTypeArguments[0], typeof(CancellationToken)]));

        if (method == null) return null;

        var task = method.Invoke(validator, [model, cancellationToken]);
        if (task is not Task validationTask) return null;

        await validationTask.ConfigureAwait(false);
        return validationTask.GetType().GetProperty("Result")?.GetValue(validationTask) as ValidationResult;
    }
}
