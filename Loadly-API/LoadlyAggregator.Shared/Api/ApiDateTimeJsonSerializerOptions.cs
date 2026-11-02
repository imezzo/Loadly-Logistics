using System.Text.Json;

namespace LoadlyAggregator.Shared.Api;

/// <summary>
/// Shared <see cref="JsonSerializerOptions"/> aligned with <see cref="ApiDateTimeContract"/> (UTC on the wire, <c>Z</c> in JSON for instants).
/// </summary>
public static class ApiDateTimeJsonSerializerOptions
{
    /// <summary>
    /// Registers the UTC <see cref="DateTime"/> / <see cref="DateTime"/>? converters. Safe to call more than once (skips if the factory is already present).
    /// </summary>
    public static void ApplyTo(JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        for (int i = 0; i < options.Converters.Count; i++)
        {
            if (options.Converters[i] is UtcDateTimeJsonConverterFactory)
            {
                return;
            }
        }

        options.Converters.Insert(0, new UtcDateTimeJsonConverterFactory());
    }

    /// <summary>
    /// Thread-safe shared options for queue and Redis (same as <see cref="CreateForQueueAndRedis"/>; <see cref="ApplyTo"/> runs inside the factory; do not mutate at runtime).
    /// </summary>
    public static JsonSerializerOptions SharedQueueAndRedis { get; } = CreateForQueueAndRedis();

    /// <summary>
    /// Thread-safe shared options for ad-hoc API JSON (same as <see cref="CreateForErrorResponse"/>; <see cref="ApplyTo"/> runs inside the factory).
    /// </summary>
    public static JsonSerializerOptions SharedErrorResponse { get; } = CreateForErrorResponse();

    /// <summary>
    /// Options for queue and Redis message (de)serialization. Calls <see cref="ApplyTo"/> for UTC <see cref="DateTime"/> rules.
    /// </summary>
    public static JsonSerializerOptions CreateForQueueAndRedis()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            WriteIndented = false
        };
        ApplyTo(options);
        return options;
    }

    /// <summary>
    /// Options for ad-hoc JSON (e.g. error middleware, small log payloads). Calls <see cref="ApplyTo"/> for UTC <see cref="DateTime"/> rules.
    /// </summary>
    public static JsonSerializerOptions CreateForErrorResponse()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
        ApplyTo(options);
        return options;
    }
}
