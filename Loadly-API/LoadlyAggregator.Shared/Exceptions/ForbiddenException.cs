namespace LoadlyAggregator.Shared.Exceptions;

public class ForbiddenException : Exception, ILocalizedException
{
    public string MessageKey => Message;
    public object[] Args { get; }

    public ForbiddenException(string message) : base(message)
    {
        Args = Array.Empty<object>();
    }
    public ForbiddenException(string messageKey, params object[] args) : base(messageKey)
    {
        Args = args ?? Array.Empty<object>();
    }
    public ForbiddenException() : base("Access forbidden")
    {
        Args = Array.Empty<object>();
    }
}
