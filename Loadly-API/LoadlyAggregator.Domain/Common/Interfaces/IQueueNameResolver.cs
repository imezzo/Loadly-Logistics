namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IQueueNameResolver
{
    string Resolve(string domain);
}
