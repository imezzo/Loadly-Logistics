using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Common.Export;

public sealed record ExportColumn<T>(
    string Key,
    string LabelEn,
    string LabelAr,
    Func<T, ILanguageService, object?> GetValue,
    bool Multiline = false);
