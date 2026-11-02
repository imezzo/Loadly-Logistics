namespace LoadlyAggregator.Shared.DTOs.Common;

/// <summary>
/// Generic API response wrapper for consistent response format across all endpoints.
/// </summary>
/// <typeparam name="T">The type of data being returned.</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// The response data payload.
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Human-readable message describing the result.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether the operation was successful.
    /// </summary>
    public bool Success { get; set; } = true;

    /// <summary>
    /// Validation or error details, keyed by field name.
    /// </summary>
    public Dictionary<string, string[]>? Errors { get; set; }

    /// <summary>
    /// Creates a successful response with data and optional message.
    /// </summary>
    public static ApiResponse<T> Create(T data, string message = "Success")
    {
        return new ApiResponse<T>
        {
            Data = data,
            Message = message,
            Success = true,
            Errors = null
        };
    }

    /// <summary>
    /// Creates an error response with message and optional validation errors.
    /// </summary>
    public static ApiResponse<T> Error(string message, Dictionary<string, string[]>? errors = null)
    {
        return new ApiResponse<T>
        {
            Data = default,
            Success = false,
            Message = message,
            Errors = errors
        };
    }
}
