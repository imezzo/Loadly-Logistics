using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Common.Export;

public static class ExportListingMetadata
{
    public static AdditionalMetadata From<T>(
        IReadOnlyList<ExportColumn<T>> columns,
        ILanguageService languageService) =>
        new()
        {
            ExportKeys = ExcelExportBuilder.ToExportKeyLabels(columns, languageService)
                .Select(pair => new ExportKey { Key = pair.Key, Label = pair.Value })
                .ToList()
        };
}
