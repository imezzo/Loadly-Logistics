using Microsoft.Extensions.Options;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Options;

namespace LoadlyAggregator.Infrastructure.Services.Notification;

public class BroadcastShareUrlBuilder : IBroadcastShareUrlBuilder
{
    private readonly string _baseUrl;

    public BroadcastShareUrlBuilder(IOptions<BroadcastShareOptions> options)
    {
        _baseUrl = (options.Value.PublicBaseUrl ?? string.Empty).TrimEnd('/');
    }

    public string? Build(string? publicToken)
    {
        if (string.IsNullOrWhiteSpace(publicToken) || string.IsNullOrWhiteSpace(_baseUrl))
        {
            return null;
        }

        return $"{_baseUrl}/{publicToken}";
    }
}
