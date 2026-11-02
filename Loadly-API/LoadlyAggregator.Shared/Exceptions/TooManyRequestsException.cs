namespace LoadlyAggregator.Shared.Exceptions;




public class TooManyRequestsException : Exception, ILocalizedException
{
    public string MessageKey => Message;
    public object[] Args { get; }

    public TooManyRequestsException(string message) : base(message)
    {
        Args = Array.Empty<object>();
    }
    public TooManyRequestsException(string messageKey, params object[] args) : base(messageKey)
    {
        Args = args ?? Array.Empty<object>();
    }
    public TooManyRequestsException() : base("Too many requests. Please try again later.")
    {
        Args = Array.Empty<object>();
    }
}
