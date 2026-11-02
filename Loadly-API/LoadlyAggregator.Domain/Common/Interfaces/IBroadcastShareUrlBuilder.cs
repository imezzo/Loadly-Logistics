namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IBroadcastShareUrlBuilder
{
    /// <summary>
    /// Builds the public landing URL for a broadcast token, or returns null when
    /// no template is configured. Callers decide whether to use it (e.g. only for SMS,
    /// only when attachments exist).
    /// </summary>
    string? Build(string? publicToken);
}
