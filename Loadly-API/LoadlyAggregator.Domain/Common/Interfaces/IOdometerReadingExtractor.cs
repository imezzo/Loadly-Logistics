using LoadlyAggregator.Domain.Common.Models;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IOdometerReadingExtractor
{
    string Provider { get; }

    Task<OdometerReadingExtraction> ExtractAsync(
        byte[] imageBytes,
        string mimeType,
        CancellationToken cancellationToken = default);
}
