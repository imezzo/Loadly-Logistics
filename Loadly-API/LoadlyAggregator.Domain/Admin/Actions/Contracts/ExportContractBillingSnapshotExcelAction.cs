using System.Globalization;
using ClosedXML.Excel;
using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Common.DTOs.Relations;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Api;
using LoadlyAggregator.Shared.Utilities;

namespace LoadlyAggregator.Domain.Admin.Actions.Contracts;

public class ExportContractBillingSnapshotExcelAction
{
    private const int IndexColumn = 1;
    private const int VehicleColumn = 2;
    private const int FirstDateColumn = 3;

    private static readonly XLColor HeaderBackground = XLColor.FromHtml("#2F5496");
    private static readonly XLColor HeaderFont = XLColor.White;
    private static readonly XLColor GridBorder = XLColor.FromHtml("#D9E2F3");
    private static readonly XLColor ExtraDayColor = XLColor.FromHtml("#5B9BD5");
    private static readonly XLColor WorkingColor = XLColor.FromHtml("#70AD47");
    private static readonly XLColor SkippedColor = XLColor.FromHtml("#E74C3C");
    private static readonly XLColor NotActiveColor = XLColor.FromHtml("#D9D9D9");
    private static readonly XLColor UnknownColor = XLColor.FromHtml("#FFC000");
    private static readonly XLColor HalfWorkingColor = XLColor.FromHtml("#A9D08E");
    private static readonly XLColor DarkFont = XLColor.FromHtml("#1F2937");

    private readonly GetContractBillingSnapshotAction _getContractBillingSnapshotAction;
    private readonly IExcelParseService _excelParseService;
    private readonly ILanguageService _languageService;

    public ExportContractBillingSnapshotExcelAction(
        GetContractBillingSnapshotAction getContractBillingSnapshotAction,
        IExcelParseService excelParseService,
        ILanguageService languageService)
    {
        _getContractBillingSnapshotAction = getContractBillingSnapshotAction;
        _excelParseService = excelParseService;
        _languageService = languageService;
    }

    public async Task<ContractBillingSnapshotExcelFileDto> ExecuteAsync(ExportContractBillingSnapshotExcelDto dto)
    {
        ContractBillingSnapshotDetailDto detail = await _getContractBillingSnapshotAction.GetByIdAsync(dto.SnapshotId);
        bool isArabic = _languageService.IsArabic();
        SnapshotGrid grid = SnapshotGrid.Build(detail, isArabic);

        using var workbook = new XLWorkbook();
        AddFleetStatusSheet(workbook, grid, isArabic);
        AddMetricSheet(workbook, "Working Days", grid, MetricKind.WorkingDayIndex, isArabic);
        AddMetricSheet(workbook, "Charges", grid, MetricKind.ChargeAmount, isArabic);
        AddMetricSheet(workbook, "Trips", grid, MetricKind.TripCount, isArabic);
        AddSummarySheet(workbook, detail, isArabic);

        if (detail.Calculation.Kms.Count > 0)
            AddKmSummarySheet(workbook, detail, grid, isArabic);

        if (detail.Calculation.KmPool is not null)
            AddKmPoolSheet(workbook, detail.Calculation.KmPool, isArabic);

        if (detail.Calculation.CommercialTermOverrides.Count > 0)
            AddCommercialTermsSheet(workbook, detail, grid, isArabic);

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        return new ContractBillingSnapshotExcelFileDto
        {
            Content = stream.ToArray(),
            FileName = BuildFileName(detail)
        };
    }

    private void AddFleetStatusSheet(XLWorkbook workbook, SnapshotGrid grid, bool isArabic)
    {
        IXLWorksheet sheet = workbook.Worksheets.Add("Fleet Status");
        sheet.RightToLeft = isArabic;

        int lastColumn = LastContentColumn(grid, includeTotal: true);
        var culture = isArabic ? new CultureInfo("ar") : CultureInfo.InvariantCulture;
        sheet.Range(1, 1, 1, Math.Min(lastColumn, 8)).Merge();
        sheet.Cell(1, 1).Value = BuildTitle(grid, culture, isArabic);
        sheet.Cell(1, 1).Style.Font.Bold = true;
        sheet.Cell(1, 1).Style.Font.FontSize = 16;
        sheet.Cell(1, 1).Style.Font.FontColor = XLColor.FromHtml("#1F4E79");
        sheet.Row(1).Height = 24;

        sheet.Range(2, 1, 2, Math.Min(lastColumn, 8)).Merge();
        sheet.Cell(2, 1).Value = BuildSubtitle(grid, isArabic);
        sheet.Cell(2, 1).Style.Font.FontColor = XLColor.FromHtml("#595959");
        sheet.Row(2).Height = 18;

        AddLegend(sheet, isArabic);
        sheet.Row(3).Height = 22;
        sheet.Row(4).Height = 8;

        const int headerRow = 5;
        WriteGridHeaders(sheet, headerRow, grid, isArabic, includeTotal: true);
        StyleHeaderRow(sheet, headerRow, lastColumn);

        int rowNumber = headerRow + 1;
        foreach (SlotRow slot in grid.Slots)
        {
            sheet.Cell(rowNumber, IndexColumn).Value = slot.Index;
            sheet.Cell(rowNumber, VehicleColumn).Value = slot.Label;
            sheet.Cell(rowNumber, IndexColumn).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            sheet.Cell(rowNumber, VehicleColumn).Style.Font.Bold = true;

            decimal rowTotal = 0;
            for (int dateIndex = 0; dateIndex < grid.Dates.Count; dateIndex++)
            {
                DateOnly date = grid.Dates[dateIndex];
                int column = FirstDateColumn + dateIndex;
                IXLCell cell = sheet.Cell(rowNumber, column);
                if (!grid.Cells.TryGetValue((slot.SlotId, date), out DayCharge? dayCharge))
                    continue;

                rowTotal += dayCharge.Amount;
                cell.Value = BuildFleetText(dayCharge, isArabic);
                PaintDayCell(cell, dayCharge.Day);
            }

            IXLCell totalCell = sheet.Cell(rowNumber, lastColumn);
            if (rowTotal > 0)
                totalCell.Value = FormatMoney(rowTotal, isArabic);

            totalCell.Style.Font.Bold = true;
            totalCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            totalCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            sheet.Row(rowNumber).Height = 78;
            rowNumber++;
        }

        int lastRow = Math.Max(headerRow, rowNumber - 1);
        IXLRange table = sheet.Range(headerRow, 1, lastRow, lastColumn);
        table.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        table.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        table.Style.Border.OutsideBorderColor = GridBorder;
        table.Style.Border.InsideBorderColor = GridBorder;

        sheet.Column(IndexColumn).Width = 8;
        sheet.Column(VehicleColumn).Width = 18;
        int widestColumn = Math.Max(lastColumn, 6);
        for (int column = FirstDateColumn; column <= widestColumn; column++)
            sheet.Column(column).Width = 16;

        sheet.SheetView.Freeze(headerRow, VehicleColumn);
        sheet.SheetView.ZoomScale = 110;
    }

    private void AddMetricSheet(
        XLWorkbook workbook,
        string sheetName,
        SnapshotGrid grid,
        MetricKind kind,
        bool isArabic)
    {
        bool includeTotal = kind is MetricKind.ChargeAmount or MetricKind.TripCount;
        IXLWorksheet sheet = workbook.Worksheets.Add(sheetName);
        const int headerRow = 1;
        WriteGridHeaders(sheet, headerRow, grid, isArabic, includeTotal);

        int rowNumber = headerRow + 1;
        foreach (SlotRow slot in grid.Slots)
        {
            sheet.Cell(rowNumber, IndexColumn).Value = slot.Index;
            sheet.Cell(rowNumber, VehicleColumn).Value = slot.Label;

            for (int dateIndex = 0; dateIndex < grid.Dates.Count; dateIndex++)
            {
                DateOnly date = grid.Dates[dateIndex];
                IXLCell cell = sheet.Cell(rowNumber, FirstDateColumn + dateIndex);
                if (!grid.Cells.TryGetValue((slot.SlotId, date), out DayCharge? dayCharge))
                    continue;

                if (kind == MetricKind.TripCount)
                {
                    if (dayCharge.Day is { } tripDay && ShowsTripCount(tripDay))
                        cell.Value = BuildTripMetricText(tripDay);
                }
                else
                {
                    object? value = ResolveMetric(kind, dayCharge);
                    if (value is decimal amount)
                        cell.Value = amount;
                    else if (value is int number)
                        cell.Value = number;
                }

                PaintDayCell(cell, dayCharge.Day);
            }

            if (includeTotal && grid.Dates.Count > 0)
            {
                if (kind == MetricKind.TripCount)
                {
                    int tripTotal = grid.Dates.Sum(date =>
                        grid.Cells.TryGetValue((slot.SlotId, date), out DayCharge? totalCharge)
                        && totalCharge.Day is { } totalDay
                        && ShowsTripCount(totalDay)
                            ? totalDay.TripCount
                            : 0);
                    sheet.Cell(rowNumber, LastContentColumn(grid, includeTotal: true)).Value = tripTotal;
                }
                else
                {
                    string start = sheet.Cell(rowNumber, FirstDateColumn).Address.ToString()!;
                    string end = sheet.Cell(rowNumber, FirstDateColumn + grid.Dates.Count - 1).Address.ToString()!;
                    sheet.Cell(rowNumber, LastContentColumn(grid, includeTotal: true)).FormulaA1 = $"SUM({start}:{end})";
                }
            }

            if (kind == MetricKind.TripCount)
                sheet.Row(rowNumber).Height = 64;

            rowNumber++;
        }

        _excelParseService.ApplyExportStyle(sheet);
        CenterMetricCells(sheet, headerRow + 1, Math.Max(headerRow, rowNumber - 1), LastContentColumn(grid, includeTotal));

        if (kind == MetricKind.ChargeAmount)
        {
            int lastColumn = LastContentColumn(grid, includeTotal: true);
            sheet.Range(headerRow + 1, FirstDateColumn, Math.Max(headerRow + 1, rowNumber - 1), lastColumn)
                .Style.NumberFormat.Format = "#,##0.00";
        }

        if (grid.Slots.Count > 0)
            sheet.SheetView.Freeze(headerRow, VehicleColumn);
    }

    private void AddSummarySheet(
        XLWorkbook workbook,
        ContractBillingSnapshotDetailDto detail,
        bool isArabic)
    {
        IXLWorksheet sheet = workbook.Worksheets.Add("Summary");
        sheet.Cell(1, 1).InsertData(new[] { new[] { Label("Field", "الحقل", isArabic), Label("Value", "القيمة", isArabic) } });

        CalculateIncludedContractSlotChargesResultDto result = detail.Calculation;
        object[][] rows =
        [
            Row(Label("Snapshot ID", "معرف اللقطة", isArabic), detail.SnapshotId),
            Row(Label("Name", "الاسم", isArabic), detail.Name),
            Row(Label("Contract ID", "معرف العقد", isArabic), detail.ContractId),
            Row(Label("Period Start", "بداية الفترة", isArabic), detail.PeriodStart.ToString("yyyy-MM-dd")),
            Row(Label("Period End", "نهاية الفترة", isArabic), detail.PeriodEnd.ToString("yyyy-MM-dd")),
            Row(Label("Status", "الحالة", isArabic), detail.Status.ToLocalizedDisplay()),
            Row(Label("Invoice ID", "معرف الفاتورة", isArabic), detail.InvoiceId?.ToString() ?? string.Empty),
            Row(Label("Finalized At", "تاريخ الاعتماد", isArabic), detail.FinalizedAt?.ToString("yyyy-MM-dd HH:mm") ?? string.Empty),
            Row(Label("Engine Version", "إصدار المحرك", isArabic), detail.EngineVersion),
            Row(Label("Total Amount", "المبلغ الإجمالي", isArabic), result.TotalAmount),
            Row(Label("Total Working Days Amount", "مبلغ أيام العمل", isArabic), result.TotalWorkingDaysAmount),
            Row(Label("Total Extra Working Days Amount", "مبلغ أيام العمل الإضافية", isArabic), result.TotalExtraWorkingDaysAmount),
            Row(Label("Total Extra KMs Amount", "مبلغ تجاوز الكيلومترات", isArabic), result.TotalExtraKmsAmount),
            Row(Label("Total Extra Trips Amount", "مبلغ الرحلات الإضافية", isArabic), result.TotalExtraTripsAmount),
            Row(Label("Total Trips Count", "عدد الرحلات", isArabic), result.TotalTripsCount),
            Row(Label("Total Days With Trips Count", "عدد أيام بها رحلات", isArabic), result.TotalDaysWithTripsCount),
            Row(Label("Total Working Days Count", "عدد أيام العمل", isArabic), result.TotalWorkingDaysCount),
            Row(Label("Total Extra Working Days Count", "عدد أيام العمل الإضافية", isArabic), result.TotalExtraWorkingDaysCount),
            Row(Label("Total Extra Trips Count", "عدد الرحلات الإضافية", isArabic), result.TotalExtraTripsCount),
            Row(Label("Total Extra KMs Count", "عدد الكيلومترات الإضافية", isArabic), result.TotalExtraKmsCount),
            Row(Label("Total Fuel Amount", "إجمالي تكلفة الوقود", isArabic), result.TotalFuelAmount),
            Row(Label("Total Fuel KMs", "كيلومترات حساب الوقود", isArabic), result.TotalFuelKms),
            Row(Label("Total Waiting For Loading Days Count", "عدد أيام انتظار التحميل", isArabic), result.TotalWaitingForLoadingDaysCount),
            Row(Label("Total Waiting For Loading Working Days Count", "عدد أيام انتظار التحميل ضمن أيام العمل", isArabic), result.TotalWaitingForLoadingWorkingDaysCount),
            Row(Label("Total Waiting For Loading Extra Days Count", "عدد أيام انتظار التحميل الإضافية", isArabic), result.TotalWaitingForLoadingExtraDaysCount),
            Row(Label("Total Waiting For Loading Amount", "مبلغ أيام انتظار التحميل", isArabic), result.TotalWaitingForLoadingAmount),
            Row(Label("Total Overnight Trips Count", "عدد رحلات المبيت", isArabic), result.TotalOvernightTripsCount),
            Row(Label("Total Overnight Customer Amount", "مبلغ مبيت العميل", isArabic), result.TotalOvernightCustomerAmount),
            Row(Label("Total Overnight Loadly Amount", "مبلغ مبيت صن واي", isArabic), result.TotalOvernightLoadlyAmount)
        ];

        sheet.Cell(2, 1).InsertData(rows);
        _excelParseService.ApplyExportStyle(sheet);
    }

    private void AddKmSummarySheet(
        XLWorkbook workbook,
        ContractBillingSnapshotDetailDto detail,
        SnapshotGrid grid,
        bool isArabic)
    {
        IXLWorksheet sheet = workbook.Worksheets.Add("Km Summary");
        sheet.Cell(1, 1).InsertData(new[]
        {
            new[]
            {
                Label("Slot ID", "معرف الخانة", isArabic),
                Label("Vehicle", "المركبة", isArabic),
                Label("Consumption", "الاستهلاك", isArabic),
                Label("Overage Km", "كيلومترات التجاوز", isArabic),
                Label("Over Consumption Rate", "سعر التجاوز", isArabic),
                Label("Suggested Overage Amount", "مبلغ التجاوز المقترح", isArabic)
            }
        });

        sheet.Cell(2, 1).InsertData(detail.Calculation.Kms.Select(summary =>
        {
            string vehicle = grid.Slots.FirstOrDefault(slot => slot.SlotId == summary.SlotId)?.Label
                ?? VehicleLabel(summary.SlotId, detail.Calculation.Vehicles, isArabic);
            return new object[]
            {
                summary.SlotId,
                vehicle,
                summary.Consumption,
                summary.OverageKm,
                summary.OverConsumptionRate,
                summary.SuggestedOverageAmount
            };
        }));

        _excelParseService.ApplyExportStyle(sheet);
    }

    private void AddKmPoolSheet(XLWorkbook workbook, IncludedContractKmPoolDto kmPool, bool isArabic)
    {
        IXLWorksheet sheet = workbook.Worksheets.Add("Km Pool");
        sheet.Cell(1, 1).InsertData(new[] { new[] { Label("Field", "الحقل", isArabic), Label("Value", "القيمة", isArabic) } });

        object[][] rows =
        [
            Row(Label("Total Consumption", "إجمالي الاستهلاك", isArabic), kmPool.TotalConsumption),
            Row(Label("Total Max KMs", "إجمالي الحد الأقصى للكيلومترات", isArabic), kmPool.TotalMaxKms),
            Row(Label("Overage Km", "كيلومترات التجاوز", isArabic), kmPool.OverageKm),
            Row(Label("Over Consumption Rate", "سعر التجاوز", isArabic), kmPool.OverConsumptionRate),
            Row(Label("Suggested Overage Amount", "مبلغ التجاوز المقترح", isArabic), kmPool.SuggestedOverageAmount)
        ];

        sheet.Cell(2, 1).InsertData(rows);
        _excelParseService.ApplyExportStyle(sheet);
    }

    private void AddCommercialTermsSheet(
        XLWorkbook workbook,
        ContractBillingSnapshotDetailDto detail,
        SnapshotGrid grid,
        bool isArabic)
    {
        IXLWorksheet sheet = workbook.Worksheets.Add("Commercial Terms");
        sheet.Cell(1, 1).InsertData(new[]
        {
            new[]
            {
                Label("Seat Root Slot ID", "معرف خانة المقعد", isArabic),
                Label("Vehicle", "المركبة", isArabic),
                Label("Effective Start", "بداية السريان", isArabic),
                Label("Effective End", "نهاية السريان", isArabic),
                Label("Daily Value", "القيمة اليومية", isArabic),
                Label("Extra Working Day Fees", "رسوم يوم العمل الإضافي", isArabic),
                Label("Over Consumption", "تجاوز الاستهلاك", isArabic),
                Label("Override Reason", "سبب التعديل", isArabic)
            }
        });

        sheet.Cell(2, 1).InsertData(detail.Calculation.CommercialTermOverrides.Select(row =>
        {
            string vehicle = grid.Slots.FirstOrDefault(slot => slot.SlotId == row.SeatRootSlotId)?.Label
                ?? VehicleLabel(row.SeatRootSlotId, detail.Calculation.Vehicles, isArabic);
            return new object[]
            {
                row.SeatRootSlotId,
                vehicle,
                row.EffectiveStart.ToString("yyyy-MM-dd"),
                row.EffectiveEnd.ToString("yyyy-MM-dd"),
                (object?)row.DailyValue ?? string.Empty,
                (object?)row.ExtraWorkingDayFees ?? string.Empty,
                (object?)row.OverConsumption ?? string.Empty,
                row.OverrideReason
            };
        }));

        _excelParseService.ApplyExportStyle(sheet);
    }

    private static void WriteGridHeaders(
        IXLWorksheet sheet,
        int headerRow,
        SnapshotGrid grid,
        bool isArabic,
        bool includeTotal)
    {
        var culture = isArabic ? new CultureInfo("ar") : CultureInfo.InvariantCulture;
        sheet.Cell(headerRow, IndexColumn).Value = Label("Index", "م", isArabic);
        sheet.Cell(headerRow, VehicleColumn).Value = Label("Vehicle", "المركبة", isArabic);

        for (int dateIndex = 0; dateIndex < grid.Dates.Count; dateIndex++)
        {
            DateOnly date = grid.Dates[dateIndex];
            string header = isArabic
                ? date.ToString("d MMM", culture)
                : date.ToString("MMM d", CultureInfo.InvariantCulture);
            sheet.Cell(headerRow, FirstDateColumn + dateIndex).Value = header;
        }

        if (includeTotal)
            sheet.Cell(headerRow, LastContentColumn(grid, includeTotal: true)).Value = Label("Total", "الإجمالي", isArabic);
    }

    private static void StyleHeaderRow(IXLWorksheet sheet, int headerRow, int lastColumn)
    {
        foreach (IXLCell cell in sheet.Row(headerRow).Cells(1, lastColumn))
        {
            cell.Style.Font.Bold = true;
            cell.Style.Font.FontColor = HeaderFont;
            cell.Style.Fill.BackgroundColor = HeaderBackground;
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            cell.Style.Alignment.WrapText = true;
        }
    }

    private static void AddLegend(IXLWorksheet sheet, bool isArabic)
    {
        (string English, string Arabic, XLColor Color, bool DarkText)[] items =
        [
            ("Extra Day", "يوم إضافي", ExtraDayColor, false),
            ("Working", "يوم عمل", WorkingColor, false),
            ("Half Working Day", "نصف يوم عمل", HalfWorkingColor, true),
            ("Out of Service", "خارج الخدمة", SkippedColor, false),
            ("Not Active", "غير نشط", NotActiveColor, true),
            ("Unknown", "غير معروف", UnknownColor, true)
        ];

        for (int index = 0; index < items.Length; index++)
        {
            (string english, string arabic, XLColor color, bool darkText) = items[index];
            IXLCell cell = sheet.Cell(3, index + 1);
            cell.Value = Label(english, arabic, isArabic);
            cell.Style.Fill.BackgroundColor = color;
            cell.Style.Font.Bold = true;
            cell.Style.Font.FontColor = darkText ? DarkFont : XLColor.White;
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        }
    }

    private static void PaintDayCell(IXLCell cell, IncludedContractSlotCalendarDayDto? day)
    {
        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        cell.Style.Alignment.WrapText = true;

        if (day is null)
            return;

        XLColor fill = ResolveFill(day);
        cell.Style.Fill.BackgroundColor = fill;
        cell.Style.Font.FontColor = UseDarkFont(day) ? DarkFont : XLColor.White;
        cell.Style.Font.Bold = true;
    }

    private static void CenterMetricCells(IXLWorksheet sheet, int firstRow, int lastRow, int lastColumn)
    {
        if (lastRow < firstRow)
            return;

        sheet.Range(firstRow, FirstDateColumn, lastRow, lastColumn).Style.Alignment.Horizontal =
            XLAlignmentHorizontalValues.Center;
        sheet.Range(firstRow, FirstDateColumn, lastRow, lastColumn).Style.Alignment.Vertical =
            XLAlignmentVerticalValues.Center;
    }

    private static string BuildFleetText(DayCharge dayCharge, bool isArabic)
    {
        IncludedContractSlotCalendarDayDto? day = dayCharge.Day;
        if (day?.Disposition == ContractSlotDayDisposition.Skipped && day.FacilityType is { } facility)
            return facility.ToLocalizedDisplay();

        var lines = new List<string>();
        if (day?.FacilityType == VehicleFacilityType.Parking)
            lines.Add(VehicleFacilityType.Parking.ToLocalizedDisplay());

        if (day is { } fleetDay && ShowsTripCount(fleetDay))
        {
            lines.Add(Label($"Trip ({fleetDay.TripCount})", $"رحلة ({fleetDay.TripCount})", isArabic));
            foreach (long tripId in fleetDay.TripIds)
                lines.Add(TripReferenceNumber.Format(tripId));
        }

        if (day?.WorkingDayIndex is { } index)
            lines.Add(Label($"Day {index}", $"يوم {index}", isArabic));

        if (dayCharge.Amount > 0)
            lines.Add(FormatMoney(dayCharge.Amount, isArabic));

        if (lines.Count > 0)
            return string.Join('\n', lines);

        if (day is null || day.Disposition == ContractSlotDayDisposition.NotActive)
            return string.Empty;

        return day.Disposition.ToLocalizedDisplay();
    }

    private static object? ResolveMetric(MetricKind kind, DayCharge dayCharge) =>
        kind switch
        {
            MetricKind.WorkingDayIndex => dayCharge.Day?.WorkingDayIndex,
            MetricKind.ChargeAmount => dayCharge.Amount > 0 ? dayCharge.Amount : null,
            MetricKind.TripCount => dayCharge.Day is { } day && ShowsTripCount(day) ? day.TripCount : null,
            _ => null
        };

    private static string BuildTripMetricText(IncludedContractSlotCalendarDayDto day)
    {
        var lines = new List<string> { day.TripCount.ToString(CultureInfo.InvariantCulture) };
        foreach (long tripId in day.TripIds)
            lines.Add(TripReferenceNumber.Format(tripId));
        return string.Join('\n', lines);
    }

    private static bool ShowsTripCount(IncludedContractSlotCalendarDayDto day) =>
        day.TripCount > 0
        || day.TripCountSource == ContractBillingSnapshotDispositionSource.Manual;

    private static XLColor ResolveFill(IncludedContractSlotCalendarDayDto day)
    {
        if (day.Disposition == ContractSlotDayDisposition.Skipped)
            return SkippedColor;

        if (day.IsExtra)
            return ExtraDayColor;

        return day.Disposition switch
        {
            ContractSlotDayDisposition.Working => WorkingColor,
            ContractSlotDayDisposition.HalfWorking => HalfWorkingColor,
            ContractSlotDayDisposition.NotActive => NotActiveColor,
            ContractSlotDayDisposition.Unknown => UnknownColor,
            _ => UnknownColor
        };
    }

    private static bool UseDarkFont(IncludedContractSlotCalendarDayDto day) =>
        day.Disposition != ContractSlotDayDisposition.Skipped
        && !day.IsExtra
        && day.Disposition is ContractSlotDayDisposition.NotActive
            or ContractSlotDayDisposition.Unknown
            or ContractSlotDayDisposition.HalfWorking;

    private static int LastContentColumn(SnapshotGrid grid, bool includeTotal) =>
        FirstDateColumn + Math.Max(grid.Dates.Count - 1, 0) + (includeTotal ? 1 : 0);

    private static string BuildTitle(SnapshotGrid grid, CultureInfo culture, bool isArabic)
    {
        DateOnly start = grid.Detail.PeriodStart;
        DateOnly end = grid.Detail.PeriodEnd;
        string period = start.Month == end.Month && start.Year == end.Year
            ? start.ToString("MMMM yyyy", culture)
            : $"{start:yyyy-MM-dd} – {end:yyyy-MM-dd}";

        return Label($"Vehicle Fleet Status – {period}", $"حالة الأسطول – {period}", isArabic);
    }

    private static string BuildSubtitle(SnapshotGrid grid, bool isArabic)
    {
        string period = $"{grid.Detail.PeriodStart:yyyy-MM-dd} – {grid.Detail.PeriodEnd:yyyy-MM-dd}";
        string status = grid.Detail.Status.ToLocalizedDisplay();
        return $"{grid.Detail.Name}  |  {period}  |  {status}";
    }

    private static string FormatMoney(decimal amount, bool isArabic)
    {
        string number = amount == decimal.Truncate(amount)
            ? amount.ToString("N0", CultureInfo.InvariantCulture)
            : amount.ToString("N2", CultureInfo.InvariantCulture);
        return Label($"{number} EGP", $"{number} ج.م", isArabic);
    }

    private static string VehicleLabel(
        long slotId,
        IReadOnlyDictionary<long, VehicleRelationDto> vehicles,
        bool isArabic)
    {
        if (!vehicles.TryGetValue(slotId, out VehicleRelationDto? vehicle))
            return Label($"Slot {slotId}", $"خانة {slotId}", isArabic);

        string plate = string.Join(' ', new[] { vehicle.PlateLetters, vehicle.PlateNo }
            .Where(part => !string.IsNullOrWhiteSpace(part)));
        if (!string.IsNullOrWhiteSpace(plate))
            return plate;

        string name = string.Join(' ', new[] { vehicle.BrandName, vehicle.ModelName }
            .Where(part => !string.IsNullOrWhiteSpace(part)));
        return string.IsNullOrWhiteSpace(name)
            ? Label($"Slot {slotId}", $"خانة {slotId}", isArabic)
            : name;
    }

    private static string BuildFileName(ContractBillingSnapshotDetailDto detail)
    {
        string name = SanitizeFileName(detail.Name);
        if (name.Length > 60)
            name = name[..60].Trim();

        string period = $"{detail.PeriodStart:yyyyMMdd}-{detail.PeriodEnd:yyyyMMdd}";
        return $"billing-snapshot-{detail.SnapshotId}-{name}-{period}.xlsx";
    }

    private static string SanitizeFileName(string name)
    {
        char[] invalid = Path.GetInvalidFileNameChars();
        string cleaned = new string(name.Select(ch => invalid.Contains(ch) ? '-' : ch).ToArray()).Trim();
        return string.IsNullOrWhiteSpace(cleaned) ? "snapshot" : cleaned;
    }

    private static string Label(string english, string arabic, bool isArabic) => isArabic ? arabic : english;

    private static object[] Row(string label, object value) => [label, value];

    private enum MetricKind
    {
        WorkingDayIndex,
        ChargeAmount,
        TripCount
    }

    private sealed record SlotRow(int Index, long SlotId, string Label);

    private sealed record DayCharge(IncludedContractSlotCalendarDayDto? Day, decimal Amount);

    private sealed record SnapshotGrid(
        ContractBillingSnapshotDetailDto Detail,
        IReadOnlyList<DateOnly> Dates,
        IReadOnlyList<SlotRow> Slots,
        IReadOnlyDictionary<(long SlotId, DateOnly Date), DayCharge> Cells)
    {
        public static SnapshotGrid Build(ContractBillingSnapshotDetailDto detail, bool isArabic)
        {
            var cells = new Dictionary<(long SlotId, DateOnly Date), DayCharge>();
            foreach (IncludedContractSlotCalendarDto calendar in detail.Calculation.Calendar)
            {
                foreach (IncludedContractSlotCalendarDayDto day in calendar.Days)
                    cells[(calendar.SlotId, day.Date)] = new DayCharge(day, 0);
            }

            foreach (IncludedContractSlotChargeLineDto line in detail.Calculation.Lines)
            {
                if (line.Date is not { } date || line.ContractSlotId <= 0)
                    continue;

                (long ContractSlotId, DateOnly) key = (line.ContractSlotId, date);
                if (cells.TryGetValue(key, out DayCharge? existing))
                    cells[key] = existing with { Amount = existing.Amount + line.Amount };
                else
                    cells[key] = new DayCharge(null, line.Amount);
            }

            DateOnly start = detail.PeriodStart;
            DateOnly end = detail.PeriodEnd;
            var dates = new SortedSet<DateOnly>();
            if (end >= start)
            {
                for (DateOnly date = start; date <= end; date = date.AddDays(1))
                    dates.Add(date);
            }

            foreach ((long _, DateOnly date) in cells.Keys)
                dates.Add(date);

            var slotIds = detail.Calculation.Calendar.Select(calendar => calendar.SlotId)
                .Concat(detail.Calculation.Vehicles.Keys)
                .Concat(detail.Calculation.Lines.Where(line => line.ContractSlotId > 0).Select(line => line.ContractSlotId))
                .Concat(detail.Calculation.Kms.Where(km => km.SlotId > 0).Select(km => km.SlotId))
                .Distinct()
                .OrderBy(slotId => slotId)
                .ToList();

            var slots = slotIds
                .Select((slotId, index) => new SlotRow(
                    index + 1,
                    slotId,
                    VehicleLabel(slotId, detail.Calculation.Vehicles, isArabic)))
                .ToList();

            return new SnapshotGrid(detail, dates.ToList(), slots, cells);
        }
    }
}
