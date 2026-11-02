using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Admin.DTOs.Invoices;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class CalendarDayFacilityNameHelper
{
    public static Task ApplyAsync(
        CalculateIncludedContractSlotChargesResultDto calculation,
        IVehicleFacilityRepository facilityRepository) =>
        ApplyMissingNamesAsync(
            calculation.Calendar.SelectMany(calendar => calendar.Days),
            day => day.FacilityId,
            day => day.FacilityName,
            (day, name) => day.FacilityName = name,
            facilityRepository);

    public static Task ApplyAsync(
        InvoiceSlotChargesDto charges,
        IVehicleFacilityRepository facilityRepository) =>
        ApplyMissingNamesAsync(
            charges.Calendar.SelectMany(calendar => calendar.Days),
            day => day.FacilityId,
            day => day.FacilityName,
            (day, name) => day.FacilityName = name,
            facilityRepository);

    private static async Task ApplyMissingNamesAsync<TDay>(
        IEnumerable<TDay> days,
        Func<TDay, long?> facilityId,
        Func<TDay, string?> facilityName,
        Action<TDay, string> setFacilityName,
        IVehicleFacilityRepository facilityRepository)
    {
        var missing = days
            .Where(day => facilityId(day).HasValue && string.IsNullOrWhiteSpace(facilityName(day)))
            .ToList();
        if (missing.Count == 0)
            return;

        var names = await facilityRepository.GetNamesByIdsAsync(
            missing.Select(day => facilityId(day)!.Value).Distinct().ToList());
        foreach (var day in missing)
        {
            if (names.TryGetValue(facilityId(day)!.Value, out var name))
                setFacilityName(day, name);
        }
    }
}
