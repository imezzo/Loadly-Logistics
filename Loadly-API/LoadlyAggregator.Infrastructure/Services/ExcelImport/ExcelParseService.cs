using ClosedXML.Excel;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Infrastructure.Services.ExcelImport;

public class ExcelParseService : IExcelParseService
{
    private static readonly XLColor HeaderBackgroundColor = XLColor.FromHtml("#2F5496");
    private static readonly XLColor HeaderFontColor = XLColor.White;

    private readonly ILanguageService _languageService;

    public ExcelParseService(ILanguageService languageService)
    {
        _languageService = languageService;
    }

    public Task<(IReadOnlyList<string> ActualHeaders, List<Dictionary<string, string>> Rows)> ParseAsync(Stream excelStream, IReadOnlyList<string> columnHeaders, CancellationToken cancellationToken = default)
    {
        using var workbook = new XLWorkbook(excelStream);
        var worksheet = workbook.Worksheet(1);
        var firstRow = worksheet.FirstRowUsed();
        var headerMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var firstCol = firstRow.FirstCellUsed().Address.ColumnNumber;
        var lastCol = firstRow.LastCellUsed().Address.ColumnNumber;
        var actualHeaders = new List<string>();
        for (var c = firstCol; c <= lastCol; c++)
        {
            var value = NormalizeHeader(firstRow.Cell(c).GetString());
            actualHeaders.Add(value);
            if (!string.IsNullOrEmpty(value))
                headerMap[value] = c;
        }

        var rows = new List<Dictionary<string, string>>();
        foreach (var row in worksheet.RowsUsed().Skip(1))
        {
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var header in columnHeaders)
            {
                var value = headerMap.TryGetValue(header, out var col) ? row.Cell(col).GetString()?.Trim() : null;
                dict[header] = value ?? string.Empty;
            }
            rows.Add(dict);
        }

        return Task.FromResult<(IReadOnlyList<string>, List<Dictionary<string, string>>)>((actualHeaders, rows));
    }

    public void ApplyExportStyle(IXLWorksheet sheet, IReadOnlyList<string>? multilineCenteredColumnHeaders = null)
    {
        multilineCenteredColumnHeaders ??= [];

        IXLRange? usedRange = sheet.RangeUsed();
        if (usedRange == null)
            return;

        int firstRow = usedRange.RangeAddress.FirstAddress.RowNumber;
        int lastRow = usedRange.RangeAddress.LastAddress.RowNumber;
        int firstCol = usedRange.RangeAddress.FirstAddress.ColumnNumber;
        int lastCol = usedRange.RangeAddress.LastAddress.ColumnNumber;
        IReadOnlyDictionary<string, int> headerColumns = BuildHeaderColumnMap(sheet, firstCol, lastCol);
        HashSet<int> multilineCenteredColumns = headerColumns
            .Where(pair => multilineCenteredColumnHeaders.Contains(pair.Key, StringComparer.OrdinalIgnoreCase))
            .Select(pair => pair.Value)
            .ToHashSet();
        bool isArabic = _languageService.IsArabic();
        sheet.RightToLeft = isArabic;

        IXLRange tableRange = sheet.Range(firstRow, firstCol, lastRow, lastCol);
        tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        tableRange.Style.Border.OutsideBorderColor = XLColor.FromHtml("#B4C6E7");
        tableRange.Style.Border.InsideBorderColor = XLColor.FromHtml("#D9E2F3");

        IXLRow headerRow = sheet.Row(firstRow);
        foreach (IXLCell cell in headerRow.Cells(firstCol, lastCol))
        {
            IXLStyle style = cell.Style;
            style.Font.Bold = true;
            style.Font.FontColor = HeaderFontColor;
            style.Fill.BackgroundColor = HeaderBackgroundColor;
            style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            style.Alignment.WrapText = true;
        }

        for (int rowNumber = firstRow + 1; rowNumber <= lastRow; rowNumber++)
        {
            IXLRow row = sheet.Row(rowNumber);
            foreach (IXLCell cell in row.Cells(firstCol, lastCol))
            {
                IXLStyle style = cell.Style;
                style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                if (multilineCenteredColumns.Contains(cell.Address.ColumnNumber) || ContainsMultipleLines(cell))
                    style.Alignment.WrapText = true;

                style.Alignment.Horizontal = ResolveHorizontalAlignment(cell, isArabic);

                if (cell.DataType == XLDataType.Boolean)
                {
                    bool isTrue = cell.GetBoolean();
                    cell.Value = isTrue ? "✓" : "✗";
                    cell.Style.Font.FontColor = isTrue ? XLColor.Green : XLColor.Red;
                }
            }
        }

        sheet.Columns(firstCol, lastCol).AdjustToContents();
        for (int columnNumber = firstCol; columnNumber <= lastCol; columnNumber++)
        {
            IXLColumn column = sheet.Column(columnNumber);
            if (column.Width < 12)
                column.Width = 12;
            if (column.Width > 48)
                column.Width = 48;
        }

        sheet.Rows(firstRow, lastRow).AdjustToContents();
        if (headerRow.Height < 20)
            headerRow.Height = 20;
    }

    private static IReadOnlyDictionary<string, int> BuildHeaderColumnMap(IXLWorksheet sheet, int firstCol, int lastCol)
    {
        Dictionary<string, int> headerColumns = new(StringComparer.OrdinalIgnoreCase);
        foreach (IXLCell cell in sheet.Row(1).Cells(firstCol, lastCol))
        {
            string header = NormalizeHeader(cell.GetString());
            if (!string.IsNullOrEmpty(header))
                headerColumns[header] = cell.Address.ColumnNumber;
        }

        return headerColumns;
    }

    private static bool ContainsMultipleLines(IXLCell cell)
    {
        if (cell.DataType == XLDataType.Text)
            return cell.GetString().Contains('\n', StringComparison.Ordinal);

        return cell.GetFormattedString().Contains('\n', StringComparison.Ordinal);
    }

    private static XLAlignmentHorizontalValues ResolveHorizontalAlignment(IXLCell cell, bool isArabic) =>
        cell.DataType switch
        {
            XLDataType.Number => XLAlignmentHorizontalValues.Right,
            XLDataType.Boolean => XLAlignmentHorizontalValues.Center,
            XLDataType.DateTime => XLAlignmentHorizontalValues.Center,
            XLDataType.TimeSpan => XLAlignmentHorizontalValues.Center,
            _ => isArabic ? XLAlignmentHorizontalValues.Right : XLAlignmentHorizontalValues.Left
        };

    private static string NormalizeHeader(string? value)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        return value.Trim().Replace('\u00A0', ' ');
    }
}
