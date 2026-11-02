using Microsoft.Extensions.Options;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Extensions;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Infrastructure.Services.Queue;

public class QueueNameResolver : IQueueNameResolver
{
    private readonly IReadOnlyDictionary<string, string> _domainToQueue;
    private readonly string _defaultQueue;

    public QueueNameResolver(IOptions<QueueOptions> options)
    {
        var opts = options.Value;
        _domainToQueue = opts.DomainToQueueMapping ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        _defaultQueue = opts.DefaultQueue ?? NotificationDomain.Admin.ToStringValue();
    }

    public string Resolve(string domain)
    {
        var normalized = (domain ?? "default").ToLowerInvariant();
        return _domainToQueue.TryGetValue(normalized, out var queue)
            ? queue
            : _domainToQueue.TryGetValue(NotificationDomain.Default.ToStringValue(), out var defaultQueue)
                ? defaultQueue
                : _defaultQueue;
    }
}
