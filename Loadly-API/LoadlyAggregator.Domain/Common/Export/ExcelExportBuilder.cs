using ClosedXML.Excel;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Common.Export;

public static class ExcelExportBuilder
{
    public static byte[] Build<T>(
        string sheetName,
        IReadOnlyList<T> rows,
        IReadOnlyList<ExportColumn<T>> columns,
        ILanguageService languageService,
        IExcelParseService excelParseService)
    {
        using var workbook = new XLWorkbook();
        IXLWorksheet sheet = workbook.Worksheets.Add(sheetName);
        bool isArabic = languageService.IsArabic();

        sheet.Cell(1, 1).InsertData(
            new[] { columns.Select(c => isArabic ? c.LabelAr : c.LabelEn) });

        if (rows.Count > 0)
        {
            sheet.Cell(2, 1).InsertData(
                rows.Select(row => columns.Select(c => ToCellValue(c.GetValue(row, languageService)))));
        }

        excelParseService.ApplyExportStyle(
            sheet,
            columns.Where(c => c.Multiline).Select(c => isArabic ? c.LabelAr : c.LabelEn).ToList());

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public static void WriteHeader<T>(
        IXLWorksheet sheet,
        IReadOnlyList<ExportColumn<T>> columns,
        ILanguageService languageService)
    {
        bool isArabic = languageService.IsArabic();
        sheet.Cell(1, 1).InsertData(
            new[] { columns.Select(column => isArabic ? column.LabelAr : column.LabelEn) });
    }

    public static int AppendRows<T>(
        IXLWorksheet sheet,
        int startRow,
        IReadOnlyList<T> rows,
        IReadOnlyList<ExportColumn<T>> columns,
        ILanguageService languageService)
    {
        if (rows.Count == 0)
            return startRow;

        sheet.Cell(startRow, 1).InsertData(
            rows.Select(row => columns.Select(column => ToCellValue(column.GetValue(row, languageService)))));
        return startRow + rows.Count;
    }

    public static IReadOnlySet<string> Keys<T>(IReadOnlyList<ExportColumn<T>> columns) =>
        columns.Select(column => column.Key).ToHashSet(StringComparer.OrdinalIgnoreCase);

    public static Dictionary<string, string> ToExportKeyLabels<T>(
        IReadOnlyList<ExportColumn<T>> columns,
        ILanguageService languageService)
    {
        bool isArabic = languageService.IsArabic();
        return columns.ToDictionary(
            column => column.Key,
            column => isArabic ? column.LabelAr : column.LabelEn,
            StringComparer.OrdinalIgnoreCase);
    }

    public static IReadOnlyList<ExportColumn<T>> ResolveColumns<T>(
        IReadOnlyList<ExportColumn<T>> allColumns,
        IReadOnlyList<string>? requestedKeys)
    {
        if (requestedKeys is not { Count: > 0 })
            return allColumns;

        Dictionary<string, ExportColumn<T>> byKey = allColumns.ToDictionary(
            column => column.Key,
            StringComparer.OrdinalIgnoreCase);

        return requestedKeys.Select(key =>
        {
            if (!byKey.TryGetValue(key, out ExportColumn<T>? column))
                throw new ValidationException("validation.export_column_invalid", key);

            return column;
        }).ToList();
    }

    private static object ToCellValue(object? value)
    {
        switch (value)
        {
            case null:
                return string.Empty;
            case string:
            case int:
            case long:
            case decimal:
            case double:
            case float:
            case bool:
            case DateTime:
                return value;
            default:
                return value.ToString() ?? string.Empty;
        }
    }
}
