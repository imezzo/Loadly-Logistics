using System.Text.Json;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Entities.EventPayloads;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Export;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Api;
using LoadlyAggregator.Shared.Time;
using LoadlyAggregator.Shared.Utilities;

namespace LoadlyAggregator.Domain.Admin.Export;

public static class TripExportColumns
{
    private const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss";

    public static IReadOnlyList<ExportColumn<TripExportRow>> All { get; } =
    [
        new("tripNumber", "Trip Number", "رقم الرحلة", (row, _) => TripReferenceNumber.Format(row.Id)),
        new("createdAt", "Created At", "تاريخ الإنشاء", (row, _) => EgyptTime.Format(row.CreatedAt, DateTimeFormat)),
        new("startDate", "Start Date", "تاريخ البداية", (row, _) => EgyptTime.Format(row.StartDate, DateTimeFormat)),
        new("financialDate", "Financial Date", "اليوم المالي", (row, _) => row.FinancialDate?.ToString("yyyy-MM-dd") ?? string.Empty),
        new("endDate", "End Date", "تاريخ النهاية", (row, _) => EgyptTime.Format(row.EndDate, DateTimeFormat)),
        new("company", "Company", "الشركة", GetCompanyName),
        new("contract", "Contract", "العقد", (row, _) => row.ContractReferenceNumber ?? string.Empty),
        new("driver", "Driver", "السائق", GetDriverName),
        new("dispatcher", "Dispatcher", "مسؤول التشغيل", (row, _) => row.DispatcherName ?? string.Empty),
        new("vehicle", "Vehicle", "المركبة", (row, _) => DisplayPlate(row)),
        new("vehicleType", "Vehicle Type", "نوع المركبة", GetVehicleTypeName),
        new("consumptionRate", "Consumption Rate", "معدل الاستهلاك", (row, _) => row.ConsumptionRate),
        new("totalFuelAmount", "Total Fuel Amount", "إجمالي كمية الوقود", (row, _) => GetTotalFuelAmount(row)),
        new("fuelPriceCharge", "Fuel Price Charge", "رسوم سعر الوقود", (row, _) =>
            TripChargeTotals.SumForChargeTypes(row.TripCharges, TripChargeType.Fuel)),
        new("duration", "Duration", "المدة", (row, _) => FormatDuration(ResolveTripDuration(row))),
        new("fuelPricingCalculationOption", "Fuel Pricing Option", "خيار تسعير الوقود", GetFuelPricingCalculationOption),
        new("sector", "Sector", "القطاع", GetSectorName),
        new("tripStatus", "Trip Status", "حالة الرحلة", (row, _) => row.Status.ToLocalizedDisplay()),
        new("currentDriverStatus", "Current Driver Status", "حالة السائق الحالية", GetCurrentDriverStatus),
        new("lifecycleActorName", "Done By", "تم بواسطة", GetLifecycleActorName),
        new("tripDocumentStatus", "Trip Document Status", "حالة مستندات الرحلة", (row, _) => row.TripDocumentStatus.ToLocalizedDisplay()),
        new("numberOfDays", "Number Of Days", "عدد الأيام", (row, _) => row.NumberOfDays.HasValue && row.NumberOfDays.Value < 0
            ? 0
            : row.NumberOfDays),
        new("isExternalTrip", "Is External Trip", "رحلة خارجية", (row, _) => row.IsExternalTrip),
        new("isBackfill", "Is Backfill", "رحلة بأثر رجعي", (row, _) => row.IsBackfill),
        new("contractDestination", "Contract Destination", "وجهات العقد", GetContractDestinationName),
        new("manualDestinations", "Manual Destinations", "وجهات يدوية", (row, _) => FormatWaypoints(row.Waypoints), Multiline: true),
        new("contractor", "Contractor", "المقاول", (row, _) => row.ContractorName ?? string.Empty),
        new("lead", "Lead", "العميل", (row, _) => row.LeadName ?? string.Empty),
        new("contactPersonName", "Contact Person Name", "اسم جهة الاتصال", (row, _) => row.ContactPersonName ?? string.Empty),
        new("contactPersonMobile", "Contact Person Mobile", "جوال جهة الاتصال", (row, _) => row.ContactPersonMobile ?? string.Empty),
        new("helpersCount", "Helpers Count", "عدد المساعدين", (row, _) => row.HelpersCount),
        new("contractDistanceKm", "Contract Distance Km", "مسافة العقد (كم)", (row, _) => row.ContractDistanceKm),
        new("actualDistanceKm", "Actual Distance Km", "المسافة الفعلية (كم)", (row, _) => row.ActualDistanceKm),
        new("actualCargoWeightTons", "Actual Cargo Weight Tons", "وزن الشحنة الفعلي (طن)", (row, _) => row.ActualCargoWeightTons),
        new("cargoPrice", "Cargo Price", "سعر الشحنة", (row, _) => row.CargoPrice),
        new("spotPrice", "Spot Price", "السعر الفوري", (row, _) => row.SpotPrice),
        new("totalTripCost", "Total Trip Cost", "إجمالي تكلفة الرحلة", (row, _) => row.TotalTripCost),
        new("totalAddonsCost", "Total Addons Cost", "إجمالي تكلفة الإضافات", (row, _) => row.TotalAddonsCost),
        new("tripCostWithoutAddon", "Trip Cost Without Addon", "تكلفة الرحلة بدون إضافات", (row, _) => row.TotalTripCost.HasValue
            ? row.TotalTripCost.Value - (row.TotalAddonsCost ?? 0m)
            : null),
        new("overNightCostBearer", "Over Night Cost Bearer", "تحمل تكلفة المبيت", (row, _) => row.OverNightCostBearer.HasValue
            ? row.OverNightCostBearer.Value.ToLocalizedDisplay()
            : string.Empty),
        new("customsStatus", "Customs Status", "حالة الجمارك", (row, _) => row.CustomsStatus.ToLocalizedDisplay()),
        new("customsTotalAmount", "Customs Total Amount", "إجمالي مبلغ الجمارك", (row, _) => row.CustomsTotalAmount),
        new("dispatcherNotes", "Dispatcher Note", "ملاحظة مسؤول التشغيل", (row, _) => row.DispatcherNotes ?? string.Empty, Multiline: true),
        new("cancellationReason", "Cancellation Reason", "سبب الإلغاء", (row, _) => ExtractCancellationReason(row) ?? string.Empty)
    ];

    private static object? GetCurrentDriverStatus(TripExportRow row, ILanguageService languageService) =>
        OperationalDriverStatusResolver.Resolve(ForStatus(row))?.ToLocalizedDisplay() ?? string.Empty;

    private static object? GetLifecycleActorName(TripExportRow row, ILanguageService languageService) =>
        TripLifecycleActorResolver.Resolve(ForStatus(row))?.ToLocalizedDisplay() ?? string.Empty;

    private static Trip ForStatus(TripExportRow row) => new()
    {
        Status = row.Status,
        Recordings = row.Recordings,
        Waypoints = row.Waypoints
    };

    private static object? GetCompanyName(TripExportRow row, ILanguageService languageService) =>
        languageService.IsArabic()
            ? row.CompanyNameAr ?? string.Empty
            : row.CompanyNameEn ?? string.Empty;

    private static object? GetDriverName(TripExportRow row, ILanguageService languageService) =>
        row.DriverName == null && row.DriverNameAr == null
            ? string.Empty
            : languageService.IsArabic()
                ? row.DriverNameAr ?? string.Empty
                : row.DriverName ?? string.Empty;

    private static string DisplayPlate(TripExportRow row) =>
        !string.IsNullOrWhiteSpace(row.HeadPlateNo) || !string.IsNullOrWhiteSpace(row.HeadPlateLetters)
            ? TripPlateDisplayHelper.FormatPlate(row.HeadPlateLetters, row.HeadPlateNo)
            : TripPlateDisplayHelper.FormatPlate(row.VehiclePlateLetters, row.VehiclePlateNo);

    private static object? GetVehicleTypeName(TripExportRow row, ILanguageService languageService)
    {
        if (!row.HasContractVehicleType && row.VehicleTypeNameEn == null && row.VehicleTypeNameAr == null)
            return string.Empty;

        return languageService.IsArabic()
            ? row.HasContractVehicleType ? row.ContractVehicleTypeNameAr ?? string.Empty : row.VehicleTypeNameAr ?? string.Empty
            : row.HasContractVehicleType ? row.ContractVehicleTypeNameEn ?? string.Empty : row.VehicleTypeNameEn ?? string.Empty;
    }

    private static object? GetFuelPricingCalculationOption(TripExportRow row, ILanguageService languageService)
    {
        if (row.FuelCalculationOption is not int option || !Enum.IsDefined(typeof(FuelPricingCalculationOption), option))
            return string.Empty;

        return ((FuelPricingCalculationOption)option).ToLocalizedDisplay();
    }

    private static decimal? GetTotalFuelAmount(TripExportRow row)
    {
        decimal? consumptionRate = row.ConsumptionRate;
        if (consumptionRate is null or <= 0m)
            return null;

        decimal distanceKm = row.ContractDistanceKm ?? 0m;
        if (distanceKm <= 0m)
            return null;

        return distanceKm / consumptionRate.Value;
    }

    private static TimeSpan? ResolveTripDuration(TripExportRow row)
    {
        if (row.Recordings != null)
        {
            DateTime? startedAt = row.Recordings
                .Where(recording =>
                    recording.RecordableType == RecordableType.TRIP
                    && recording.RecordableEventType == RecordableEventType.TRIP_STARTED)
                .OrderBy(recording => recording.CreatedAt)
                .Select(recording => (DateTime?)recording.CreatedAt)
                .FirstOrDefault();

            DateTime? completedAt = row.Recordings
                .Where(recording =>
                    recording.RecordableType == RecordableType.TRIP
                    && recording.RecordableEventType == RecordableEventType.TRIP_COMPLETED)
                .OrderByDescending(recording => recording.CreatedAt)
                .Select(recording => (DateTime?)recording.CreatedAt)
                .FirstOrDefault();

            if (startedAt.HasValue && completedAt.HasValue && completedAt.Value > startedAt.Value)
                return completedAt.Value - startedAt.Value;
        }

        if (row.StartDate.HasValue && row.EndDate.HasValue && row.EndDate.Value > row.StartDate.Value)
            return row.EndDate.Value - row.StartDate.Value;

        return null;
    }

    private static string FormatDuration(TimeSpan? duration)
    {
        if (!duration.HasValue)
            return string.Empty;

        int totalHours = (int)duration.Value.TotalHours;
        return $"{totalHours:D2}:{duration.Value.Minutes:D2}:{duration.Value.Seconds:D2}";
    }

    private static object? GetSectorName(TripExportRow row, ILanguageService languageService) =>
        languageService.IsArabic()
            ? row.SectorNameAr ?? string.Empty
            : row.SectorNameEn ?? string.Empty;

    private static object? GetContractDestinationName(TripExportRow row, ILanguageService languageService) =>
        languageService.IsArabic()
            ? row.ContractDestinationNameAr ?? string.Empty
            : row.ContractDestinationNameEn ?? string.Empty;

    private static string FormatWaypoints(ICollection<TripWaypoint>? waypoints)
    {
        if (waypoints == null || waypoints.Count == 0)
            return string.Empty;

        return string.Join(
            Environment.NewLine,
            waypoints.OrderBy(waypoint => waypoint.Sequence).Select(waypoint =>
                $"{waypoint.StopType.ToLocalizedDisplay()}: {waypoint.AddressName ?? string.Empty}"));
    }

    private static string? ExtractCancellationReason(TripExportRow row)
    {
        if (row.Recordings == null)
            return null;

        Recording? tripCancelledRecording = row.Recordings
            .Where(recording => recording.RecordableType == RecordableType.TRIP && recording.RecordableEventType == RecordableEventType.TRIP_CANCELLED)
            .OrderByDescending(recording => recording.CreatedAt)
            .FirstOrDefault();

        if (tripCancelledRecording?.Data == null)
            return null;

        try
        {
            DriverCancelTripEventData? eventData = JsonSerializer.Deserialize<DriverCancelTripEventData>(
                tripCancelledRecording.Data.RootElement);
            return eventData?.CancellationReason;
        }
        catch
        {
            return null;
        }
    }
}
