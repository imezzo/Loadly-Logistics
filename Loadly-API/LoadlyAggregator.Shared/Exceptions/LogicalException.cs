namespace LoadlyAggregator.Shared.Exceptions;

public class LogicalException : Exception, ILocalizedException
{
    public string MessageKey => Message;
    public object[] Args { get; }

    public LogicalException(string message) : base(message)
    {
        Args = Array.Empty<object>();
    }
    public LogicalException(string messageKey, params object[] args) : base(messageKey)
    {
        Args = args ?? Array.Empty<object>();
    }
    public LogicalException(string message, Exception innerException) : base(message, innerException)
    {
        Args = Array.Empty<object>();
    }
}
