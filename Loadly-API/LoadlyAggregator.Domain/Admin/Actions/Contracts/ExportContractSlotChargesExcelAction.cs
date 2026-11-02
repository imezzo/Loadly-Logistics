using System.Globalization;
using ClosedXML.Excel;
using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Common.DTOs.Relations;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Api;
using LoadlyAggregator.Shared.Time;
using LoadlyAggregator.Shared.Utilities;

namespace LoadlyAggregator.Domain.Admin.Actions.Contracts;

public class ExportContractSlotChargesExcelAction
{
    private const int SlotColumn = 1;
    private const int PlateNoColumn = 2;
    private const int PlateLettersColumn = 3;
    private const int FirstDateColumn = 4;
    private const int HeaderRow = 2;

    private static readonly XLColor HeaderBackground = XLColor.FromHtml("#2F5496");
    private static readonly XLColor HeaderFont = XLColor.White;
    private static readonly XLColor GridBorder = XLColor.FromHtml("#D9E2F3");
    private static readonly XLColor DarkFont = XLColor.FromHtml("#1F2937");
    private static readonly XLColor ExtraDayColor = XLColor.FromHtml("#5B9BD5");
    private static readonly XLColor WorkingColor = XLColor.FromHtml("#70AD47");
    private static readonly XLColor SkippedColor = XLColor.FromHtml("#E74C3C");
    private static readonly XLColor NotActiveColor = XLColor.FromHtml("#D9D9D9");
    private static readonly XLColor UnknownColor = XLColor.FromHtml("#FFC000");
    private static readonly XLColor HalfWorkingColor = XLColor.FromHtml("#A9D08E");
    private static readonly XLColor ExtraTripColor = XLColor.FromHtml("#ED7D31");
    private static readonly XLColor ExtraKmColor = XLColor.FromHtml("#7030A0");

    private readonly CalculateContractSlotChargesAction _calculateContractSlotChargesAction;
    private readonly IContractRepository _contractRepository;
    private readonly IExcelParseService _excelParseService;
    private readonly ILanguageService _languageService;

    public ExportContractSlotChargesExcelAction(
        CalculateContractSlotChargesAction calculateContractSlotChargesAction,
        IContractRepository contractRepository,
        IExcelParseService excelParseService,
        ILanguageService languageService)
    {
        _calculateContractSlotChargesAction = calculateContractSlotChargesAction;
        _contractRepository = contractRepository;
        _excelParseService = excelParseService;
        _languageService = languageService;
    }

    public async Task<ContractSlotChargesExcelFileDto> ExecuteAsync(ExportContractSlotChargesExcelDto dto)
    {
        var result = await _calculateContractSlotChargesAction.ExecuteAsync(new CalculateContractSlotChargesDto
        {
            ContractId = dto.ContractId,
            ContractSlotId = dto.ContractSlotId,
            PeriodStart = dto.PeriodStart,
            PeriodEnd = dto.PeriodEnd
        });

        var contract = await _contractRepository.GetByIdOrThrowForSlotBillingAsync(dto.ContractId);
        var slotVehicles = result.Vehicles;
        bool isArabic = _languageService.IsArabic();
        using var workbook = new XLWorkbook();

        AddSummarySheet(workbook, result, isArabic);
        AddChargesSheet(workbook, result, slotVehicles, dto.PeriodStart, dto.PeriodEnd, isArabic);
        AddCalendarSheet(workbook, result, slotVehicles, dto.PeriodStart, dto.PeriodEnd, isArabic);

        if (result.Kms.Count > 0)
            AddKmSummarySheet(workbook, result, slotVehicles, isArabic);

        if (result.KmPool is not null)
            AddKmPoolSheet(workbook, result.KmPool, isArabic);

        foreach (IXLWorksheet worksheet in workbook.Worksheets)
        {
            if (worksheet.Name is "Charges" or "Calendar")
                continue;

            _excelParseService.ApplyExportStyle(worksheet);
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        return new ContractSlotChargesExcelFileDto
        {
            Content = stream.ToArray(),
            FileName = $"contract-{contract.ReferenceNumber}-slot-charges-{dto.PeriodStart:yyyyMMdd}-{dto.PeriodEnd:yyyyMMdd}.xlsx"
        };
    }

    private static SlotVehiclePlateInfo ResolveSlotVehicle(
        long slotId,
        IReadOnlyDictionary<long, VehicleRelationDto> slotVehicles) =>
        slotVehicles.TryGetValue(slotId, out var vehicle)
            ? new SlotVehiclePlateInfo(vehicle.PlateNo, vehicle.PlateLetters)
            : SlotVehiclePlateInfo.Empty;

    private static void AddSummarySheet(
        XLWorkbook workbook,
        CalculateIncludedContractSlotChargesResultDto result,
        bool isArabic)
    {
        IXLWorksheet sheet = workbook.Worksheets.Add("Summary");
        sheet.Cell(1, 1).InsertData(new[] { new[] { Label("Field", "الحقل", isArabic), Label("Value", "القيمة", isArabic) } });

        object[][] rows =
        [
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
    }

    private static void AddChargesSheet(
        XLWorkbook workbook,
        CalculateIncludedContractSlotChargesResultDto result,
        IReadOnlyDictionary<long, VehicleRelationDto> slotVehicles,
        DateOnly periodStart,
        DateOnly periodEnd,
        bool isArabic)
    {
        IXLWorksheet sheet = workbook.Worksheets.Add("Charges");
        List<DateOnly> dates = BuildDates(
            periodStart,
            periodEnd,
            result.Lines.Where(line => line.Date.HasValue).Select(line => line.Date!.Value));
        List<long> slotIds = BuildSlotIds(result);
        bool hasUndated = result.Lines.Any(line => line.Date is null);
        int lastDateColumn = FirstDateColumn + Math.Max(dates.Count - 1, 0);
        int undatedColumn = hasUndated ? lastDateColumn + (dates.Count > 0 ? 1 : 0) : 0;
        int totalColumn = (dates.Count > 0 ? lastDateColumn : FirstDateColumn - 1) + (hasUndated ? 1 : 0) + 1;

        Dictionary<(long SlotId, DateOnly Date), List<IncludedContractSlotChargeLineDto>> linesByDay = result.Lines
            .Where(line => line.Date.HasValue && line.ContractSlotId > 0)
            .GroupBy(line => (line.ContractSlotId, line.Date!.Value))
            .ToDictionary(group => group.Key, group => group.ToList());
        Dictionary<long, decimal> undatedBySlot = result.Lines
            .Where(line => line.Date is null && line.ContractSlotId > 0)
            .GroupBy(line => line.ContractSlotId)
            .ToDictionary(group => group.Key, group => group.Sum(line => line.Amount));

        AddChargeLegend(sheet, isArabic);
        WriteIdentityHeaders(sheet, isArabic);
        WriteDateHeaders(sheet, dates, isArabic);
        if (hasUndated)
            sheet.Cell(HeaderRow, undatedColumn).Value = Label("No date", "بدون تاريخ", isArabic);

        sheet.Cell(HeaderRow, totalColumn).Value = Label("Total", "الإجمالي", isArabic);
        StyleHeaderRow(sheet, totalColumn);

        int rowNumber = HeaderRow + 1;
        foreach (long slotId in slotIds)
        {
            WriteIdentityCells(sheet, rowNumber, slotId, slotVehicles);
            for (int dateIndex = 0; dateIndex < dates.Count; dateIndex++)
            {
                if (!linesByDay.TryGetValue((slotId, dates[dateIndex]), out List<IncludedContractSlotChargeLineDto>? lines))
                    continue;

                IXLCell cell = sheet.Cell(rowNumber, FirstDateColumn + dateIndex);
                cell.Value = lines.Sum(line => line.Amount);
                PaintChargeCell(cell, lines);
            }

            if (hasUndated && undatedBySlot.TryGetValue(slotId, out decimal undatedAmount) && undatedAmount != 0)
                sheet.Cell(rowNumber, undatedColumn).Value = undatedAmount;

            if (dates.Count > 0 || hasUndated)
            {
                int sumStart = dates.Count > 0 ? FirstDateColumn : undatedColumn;
                int sumEnd = hasUndated ? undatedColumn : lastDateColumn;
                string start = sheet.Cell(rowNumber, sumStart).Address.ToString()!;
                string end = sheet.Cell(rowNumber, sumEnd).Address.ToString()!;
                sheet.Cell(rowNumber, totalColumn).FormulaA1 = $"SUM({start}:{end})";
            }

            rowNumber++;
        }

        FinishGridSheet(sheet, rowNumber - 1, totalColumn, isArabic, calendarRows: false);
    }

    private static void AddCalendarSheet(
        XLWorkbook workbook,
        CalculateIncludedContractSlotChargesResultDto result,
        IReadOnlyDictionary<long, VehicleRelationDto> slotVehicles,
        DateOnly periodStart,
        DateOnly periodEnd,
        bool isArabic)
    {
        IXLWorksheet sheet = workbook.Worksheets.Add("Calendar");
        List<DateOnly> dates = BuildDates(
            periodStart,
            periodEnd,
            result.Calendar.SelectMany(calendar => calendar.Days.Select(day => day.Date)));
        List<long> slotIds = BuildSlotIds(result);
        int lastColumn = dates.Count > 0
            ? FirstDateColumn + dates.Count - 1
            : PlateLettersColumn;

        Dictionary<(long SlotId, DateOnly Date), IncludedContractSlotCalendarDayDto> daysBySlot = result.Calendar
            .SelectMany(calendar => calendar.Days.Select(day => (calendar.SlotId, Day: day)))
            .GroupBy(item => (item.SlotId, item.Day.Date))
            .ToDictionary(group => group.Key, group => group.First().Day);

        AddCalendarLegend(sheet, isArabic);
        WriteIdentityHeaders(sheet, isArabic);
        WriteDateHeaders(sheet, dates, isArabic);
        StyleHeaderRow(sheet, lastColumn);

        int rowNumber = HeaderRow + 1;
        foreach (long slotId in slotIds)
        {
            WriteIdentityCells(sheet, rowNumber, slotId, slotVehicles);
            for (int dateIndex = 0; dateIndex < dates.Count; dateIndex++)
            {
                if (!daysBySlot.TryGetValue((slotId, dates[dateIndex]), out IncludedContractSlotCalendarDayDto? day))
                    continue;

                IXLCell cell = sheet.Cell(rowNumber, FirstDateColumn + dateIndex);
                cell.Value = BuildCalendarCell(day, isArabic);
                PaintCalendarCell(cell, day);
            }

            sheet.Row(rowNumber).Height = 78;
            rowNumber++;
        }

        FinishGridSheet(sheet, rowNumber - 1, lastColumn, isArabic, calendarRows: true);
    }

    private static void AddChargeLegend(IXLWorksheet sheet, bool isArabic)
    {
        (string English, string Arabic, XLColor Color)[] items =
        [
            ("Base Working Day", "يوم عمل أساسي", WorkingColor),
            ("Extra Working Day", "يوم عمل إضافي", ExtraDayColor),
            ("Extra Trip", "رحلة إضافية", ExtraTripColor),
            ("Extra KM Overage", "تجاوز الكيلومترات", ExtraKmColor)
        ];

        for (int index = 0; index < items.Length; index++)
        {
            (string english, string arabic, XLColor color) = items[index];
            IXLCell cell = sheet.Cell(1, index + 1);
            cell.Value = Label(english, arabic, isArabic);
            cell.Style.Fill.BackgroundColor = color;
            cell.Style.Font.Bold = true;
            cell.Style.Font.FontColor = XLColor.White;
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            cell.Style.Alignment.WrapText = true;
        }
    }

    private static void AddCalendarLegend(IXLWorksheet sheet, bool isArabic)
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
            IXLCell cell = sheet.Cell(1, index + 1);
            cell.Value = Label(english, arabic, isArabic);
            cell.Style.Fill.BackgroundColor = color;
            cell.Style.Font.Bold = true;
            cell.Style.Font.FontColor = darkText ? DarkFont : XLColor.White;
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            cell.Style.Alignment.WrapText = true;
        }
    }

    private static void WriteIdentityHeaders(IXLWorksheet sheet, bool isArabic)
    {
        sheet.Cell(HeaderRow, SlotColumn).Value = Label("Slot ID", "معرف الخانة", isArabic);
        sheet.Cell(HeaderRow, PlateNoColumn).Value = Label("Plate Number", "رقم اللوحة", isArabic);
        sheet.Cell(HeaderRow, PlateLettersColumn).Value = Label("Plate Letters", "حروف اللوحة", isArabic);
    }

    private static void WriteIdentityCells(
        IXLWorksheet sheet,
        int rowNumber,
        long slotId,
        IReadOnlyDictionary<long, VehicleRelationDto> slotVehicles)
    {
        SlotVehiclePlateInfo vehicle = ResolveSlotVehicle(slotId, slotVehicles);
        sheet.Cell(rowNumber, SlotColumn).Value = slotId;
        sheet.Cell(rowNumber, PlateNoColumn).Value = vehicle.PlateNo;
        sheet.Cell(rowNumber, PlateLettersColumn).Value = vehicle.PlateLetters;
        sheet.Cell(rowNumber, PlateNoColumn).Style.Font.Bold = true;
    }

    private static void WriteDateHeaders(IXLWorksheet sheet, IReadOnlyList<DateOnly> dates, bool isArabic)
    {
        CultureInfo culture = isArabic ? new CultureInfo("ar") : CultureInfo.InvariantCulture;
        for (int dateIndex = 0; dateIndex < dates.Count; dateIndex++)
        {
            DateOnly date = dates[dateIndex];
            sheet.Cell(HeaderRow, FirstDateColumn + dateIndex).Value = isArabic
                ? date.ToString("d MMM", culture)
                : date.ToString("MMM d", CultureInfo.InvariantCulture);
        }
    }

    private static void StyleHeaderRow(IXLWorksheet sheet, int lastColumn)
    {
        sheet.Row(1).Height = 22;
        foreach (IXLCell cell in sheet.Row(HeaderRow).Cells(1, lastColumn))
        {
            cell.Style.Font.Bold = true;
            cell.Style.Font.FontColor = HeaderFont;
            cell.Style.Fill.BackgroundColor = HeaderBackground;
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            cell.Style.Alignment.WrapText = true;
        }

        sheet.Row(HeaderRow).Height = 22;
    }

    private static void FinishGridSheet(
        IXLWorksheet sheet,
        int lastRow,
        int lastColumn,
        bool isArabic,
        bool calendarRows)
    {
        sheet.RightToLeft = isArabic;
        int tableLastRow = Math.Max(HeaderRow, lastRow);
        IXLRange table = sheet.Range(HeaderRow, 1, tableLastRow, lastColumn);
        table.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        table.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        table.Style.Border.OutsideBorderColor = GridBorder;
        table.Style.Border.InsideBorderColor = GridBorder;

        if (lastRow >= HeaderRow + 1)
        {
            sheet.Range(HeaderRow + 1, FirstDateColumn, lastRow, lastColumn).Style.Alignment.Horizontal =
                XLAlignmentHorizontalValues.Center;
            sheet.Range(HeaderRow + 1, FirstDateColumn, lastRow, lastColumn).Style.Alignment.Vertical =
                XLAlignmentVerticalValues.Center;
            sheet.Range(HeaderRow + 1, FirstDateColumn, lastRow, lastColumn).Style.Alignment.WrapText = true;
            if (!calendarRows)
                sheet.Range(HeaderRow + 1, FirstDateColumn, lastRow, lastColumn).Style.NumberFormat.Format = "#,##0.00";
        }

        sheet.Column(SlotColumn).Width = 14;
        sheet.Column(PlateNoColumn).Width = 14;
        sheet.Column(PlateLettersColumn).Width = 14;
        int widestColumn = Math.Max(lastColumn, 6);
        for (int column = FirstDateColumn; column <= widestColumn; column++)
            sheet.Column(column).Width = calendarRows ? 16 : 12;

        if (lastRow >= HeaderRow)
            sheet.SheetView.Freeze(HeaderRow, PlateLettersColumn);
    }

    private static void PaintChargeCell(IXLCell cell, IReadOnlyList<IncludedContractSlotChargeLineDto> lines)
    {
        ContractSlotChargeKind kind = (ContractSlotChargeKind)lines
            .OrderByDescending(line => line.Amount)
            .First()
            .ChargeKind;
        XLColor fill = kind switch
        {
            ContractSlotChargeKind.ExtraWorkingDay => ExtraDayColor,
            ContractSlotChargeKind.ExtraTrip => ExtraTripColor,
            ContractSlotChargeKind.ExtraKmOverage => ExtraKmColor,
            _ => WorkingColor
        };

        cell.Style.Fill.BackgroundColor = fill;
        cell.Style.Font.FontColor = XLColor.White;
        cell.Style.Font.Bold = true;
    }

    private static void PaintCalendarCell(IXLCell cell, IncludedContractSlotCalendarDayDto day)
    {
        XLColor fill = ResolveCalendarFill(day);
        cell.Style.Fill.BackgroundColor = fill;
        cell.Style.Font.FontColor = UseDarkCalendarFont(day) ? DarkFont : XLColor.White;
        cell.Style.Font.Bold = true;
        cell.Style.Alignment.WrapText = true;
    }

    private static string BuildCalendarCell(IncludedContractSlotCalendarDayDto day, bool isArabic)
    {
        if (day.Disposition == ContractSlotDayDisposition.Skipped && day.FacilityType is { } facility)
            return facility.ToLocalizedDisplay();

        if (day.Disposition == ContractSlotDayDisposition.NotActive
            && day.TripCount == 0
            && day.WorkingDayIndex is null)
        {
            return string.Empty;
        }

        var lines = new List<string>();
        if (day.FacilityType == VehicleFacilityType.Parking)
            lines.Add(VehicleFacilityType.Parking.ToLocalizedDisplay());

        if (day.TripCount > 0)
        {
            lines.Add(Label($"Trips ({day.TripCount})", $"رحلات ({day.TripCount})", isArabic));
            foreach (long tripId in day.TripIds)
                lines.Add(TripReferenceNumber.Format(tripId));
        }

        if (day.WorkingDayIndex is { } index)
            lines.Add(Label($"Day {index}", $"يوم {index}", isArabic));

        if (lines.Count == 0)
            return day.Disposition.ToLocalizedDisplay();

        return string.Join('\n', lines);
    }

    private static XLColor ResolveCalendarFill(IncludedContractSlotCalendarDayDto day)
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

    private static bool UseDarkCalendarFont(IncludedContractSlotCalendarDayDto day) =>
        day.Disposition != ContractSlotDayDisposition.Skipped
        && !day.IsExtra
        && day.Disposition is ContractSlotDayDisposition.NotActive
            or ContractSlotDayDisposition.Unknown
            or ContractSlotDayDisposition.HalfWorking;

    private static List<DateOnly> BuildDates(
        DateOnly periodStart,
        DateOnly periodEnd,
        IEnumerable<DateOnly> extraDates)
    {
        var dates = new SortedSet<DateOnly>();
        DateOnly start = periodStart;
        DateOnly end = periodEnd;
        if (end >= start)
        {
            for (DateOnly date = start; date <= end; date = date.AddDays(1))
                dates.Add(date);
        }

        foreach (DateOnly date in extraDates)
            dates.Add(date);

        return dates.ToList();
    }

    private static List<long> BuildSlotIds(CalculateIncludedContractSlotChargesResultDto result) =>
        result.Calendar.Select(calendar => calendar.SlotId)
            .Concat(result.Lines.Where(line => line.ContractSlotId > 0).Select(line => line.ContractSlotId))
            .Concat(result.Vehicles.Keys)
            .Distinct()
            .OrderBy(slotId => slotId)
            .ToList();

    private static void AddKmSummarySheet(
        XLWorkbook workbook,
        CalculateIncludedContractSlotChargesResultDto result,
        IReadOnlyDictionary<long, VehicleRelationDto> slotVehicles,
        bool isArabic)
    {
        IXLWorksheet sheet = workbook.Worksheets.Add("Km Summary");
        sheet.Cell(1, 1).InsertData(new[]
        {
            new[]
            {
                Label("Slot ID", "معرف الخانة", isArabic),
                Label("Plate Number", "رقم اللوحة", isArabic),
                Label("Plate Letters", "حروف اللوحة", isArabic),
                Label("Consumption", "الاستهلاك", isArabic),
                Label("Overage Km", "كيلومترات التجاوز", isArabic),
                Label("Over Consumption Rate", "سعر التجاوز", isArabic),
                Label("Suggested Overage Amount", "مبلغ التجاوز المقترح", isArabic)
            }
        });

        sheet.Cell(2, 1).InsertData(result.Kms.Select(summary =>
        {
            var vehicle = ResolveSlotVehicle(summary.SlotId, slotVehicles);
            return new object[]
            {
                summary.SlotId,
                vehicle.PlateNo,
                vehicle.PlateLetters,
                summary.Consumption,
                summary.OverageKm,
                summary.OverConsumptionRate,
                summary.SuggestedOverageAmount
            };
        }));
    }

    private static void AddKmPoolSheet(
        XLWorkbook workbook,
        IncludedContractKmPoolDto kmPool,
        bool isArabic)
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
    }

    private static string Label(string en, string ar, bool isArabic) => isArabic ? ar : en;

    private static object[] Row(string label, object value) => [label, value];

    private readonly record struct SlotVehiclePlateInfo(string PlateNo, string PlateLetters)
    {
        public static SlotVehiclePlateInfo Empty => new(string.Empty, string.Empty);
    }
}
