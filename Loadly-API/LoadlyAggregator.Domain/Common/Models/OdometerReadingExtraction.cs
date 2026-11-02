namespace LoadlyAggregator.Domain.Common.Models;

public sealed record OdometerReadingExtraction(
    int? Reading,
    string? Unit,
    bool? IsDigital,
    string? ReadingQuality,
    bool IsScreen,
    int? TotalTokens,
    string? RawResponse,
    string Provider,
    string? ProviderModel = null);
