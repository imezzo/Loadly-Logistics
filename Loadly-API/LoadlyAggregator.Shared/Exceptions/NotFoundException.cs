namespace LoadlyAggregator.Shared.Exceptions;

public class NotFoundException : Exception, ILocalizedException
{
    public string MessageKey => Message;
    public object[] Args { get; }

    public NotFoundException(string message) : base(message)
    {
        Args = Array.Empty<object>();
    }
    public NotFoundException(string messageKey, params object[] args) : base(messageKey)
    {
        Args = args ?? Array.Empty<object>();
    }
}
