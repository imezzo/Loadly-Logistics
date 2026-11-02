namespace LoadlyAggregator.Shared.Exceptions;

public class ValidationException : Exception, ILocalizedException
{
    public Dictionary<string, string[]> Errors { get; }
    public string MessageKey => Message;
    public object[] Args { get; }

    public ValidationException() : base("Validation failed")
    {
        Errors = new Dictionary<string, string[]>();
        Args = Array.Empty<object>();
    }
    public ValidationException(Dictionary<string, string[]> errors) : base("Validation failed")
    {
        Errors = errors;
        Args = Array.Empty<object>();
    }
    public ValidationException(string message) : base(message)
    {
        Errors = new Dictionary<string, string[]>();
        Args = Array.Empty<object>();
    }
    public ValidationException(string messageKey, params object[] args) : base(messageKey)
    {
        Errors = new Dictionary<string, string[]>();
        Args = args ?? Array.Empty<object>();
    }
}
