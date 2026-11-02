using ClosedXML.Excel;

namespace LoadlyAggregator.Domain.Common.Interfaces;

/// <summary>
/// Excel workbook helpers for import parsing and export styling.
/// </summary>
public interface IExcelParseService
{
    /// <summary>
    /// Reads the first sheet; first row = actual headers (returned). Matches columns to <paramref name="columnHeaders"/> (case-insensitive).
    /// Returns the file's first row as ActualHeaders and one dictionary per data row as Rows.
    /// </summary>
    Task<(IReadOnlyList<string> ActualHeaders, List<Dictionary<string, string>> Rows)> ParseAsync(Stream excelStream, IReadOnlyList<string> columnHeaders, CancellationToken cancellationToken = default);

    /// <summary>
    /// Applies shared professional styling to an export worksheet.
    /// </summary>
    void ApplyExportStyle(IXLWorksheet sheet, IReadOnlyList<string>? multilineCenteredColumnHeaders = null);
}
