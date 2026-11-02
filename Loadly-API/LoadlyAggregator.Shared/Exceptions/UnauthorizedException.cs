namespace LoadlyAggregator.Shared.Exceptions;

public class UnauthorizedException : Exception, ILocalizedException
{
    public string MessageKey => Message;
    public object[] Args { get; }

    public UnauthorizedException(string message) : base(message)
    {
        Args = Array.Empty<object>();
    }
    public UnauthorizedException(string messageKey, params object[] args) : base(messageKey)
    {
        Args = args ?? Array.Empty<object>();
    }
    public UnauthorizedException() : base("Unauthorized access")
    {
        Args = Array.Empty<object>();
    }
}
