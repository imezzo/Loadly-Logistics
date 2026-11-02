using LoadlyAggregator.Domain.Admin.DTOs.Dashboard;
using LoadlyAggregator.Domain.Common.Export;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Export;

public static class DriverLifecycleTripsExportColumns
{
    public static IReadOnlyList<ExportColumn<DriverCompletedTripsByLifecycleActorDto>> All { get; } =
    [
        new("id", "ID", "الرقم التعريفي", (row, _) => row.Id),
        new("name", "Name", "الاسم", GetName),
        new("phone", "Phone number", "رقم الهاتف", (row, _) => row.Phone),
        new("driver", "Driver trips", "رحلة تمت عن طريق السائق", (row, _) => row.Driver),
        new("dispatcher", "Dispatcher trips", "رحلة تمت عن طريق الموزع", (row, _) => row.Dispatcher),
        new("assisted", "Assisted trips", "رحلة تمت بمساعدة", (row, _) => row.Assisted),
        new("total", "Total", "الإجمالي", (row, _) => row.Total)
    ];

    private static object? GetName(DriverCompletedTripsByLifecycleActorDto row, ILanguageService languageService) =>
        languageService.IsArabic()
            ? row.NameAr ?? string.Empty
            : row.NameEn ?? string.Empty;
}
