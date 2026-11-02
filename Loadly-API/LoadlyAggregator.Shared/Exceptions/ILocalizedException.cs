namespace LoadlyAggregator.Shared.Exceptions;

public interface ILocalizedException
{
    string MessageKey { get; }
    object[] Args { get; }
}
