using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rollbar;
using LoadlyAggregator.API.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Api;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var user = context.User?.Identity?.Name ?? "Anonymous";

            if (ExceptionResponseMapper.ShouldReportToRollbar(ex))
            {
                _logger.LogError(
                    ex,
                    "Unhandled exception occurred. Path: {Path}, Method: {Method}, User: {User}",
                    context.Request.Path,
                    context.Request.Method,
                    user);

                RollbarLocator.RollbarInstance.AsBlockingLogger(TimeSpan.FromSeconds(1)).Error(ex);
            }
            else
            {
                // Expected business exceptions (validation, not found, logical, etc.) are part of
                // normal request flow; log without a stack trace to avoid polluting error logs.
                _logger.LogInformation(
                    "Handled business exception {ExceptionType}: {Message}. Path: {Path}, Method: {Method}, User: {User}",
                    ex.GetType().Name,
                    ex.Message,
                    context.Request.Path,
                    context.Request.Method,
                    user);
            }

            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var translationService = context.RequestServices.GetRequiredService<ITranslationService>();
        var hostEnvironment = context.RequestServices.GetService<IHostEnvironment>();
        var mapped = ExceptionResponseMapper.Map(exception, translationService, hostEnvironment);
        var response = ApiResponse<object>.Error(mapped.Message, mapped.Errors);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = mapped.StatusCode;

        return context.Response.WriteAsync(
            JsonSerializer.Serialize(response, ApiDateTimeJsonSerializerOptions.SharedErrorResponse));
    }
}
