using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Export;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Admin.Export;

public static class VehicleExportColumns
{
    public static IReadOnlyList<ExportColumn<Vehicle>> All { get; } =
    [
        new("vehicleUnitType", "Vehicle Unit Type", "نوع وحدة المركبة", (vehicle, _) => vehicle.VehicleType.VehicleUnitType.HasValue
            ? vehicle.VehicleType.VehicleUnitType.Value.ToLocalizedDisplay()
            : string.Empty),
        new("vehicleCategory", "Vehicle Category", "فئة المركبة", (vehicle, _) => vehicle.VehicleType.Category.ToLocalizedDisplay()),
        new("vehicleType", "Vehicle Type", "نوع المركبة", (vehicle, lang) => lang.IsArabic()
            ? vehicle.VehicleType.NameAr ?? string.Empty
            : vehicle.VehicleType.NameEn ?? string.Empty),
        new("fuelType", "Fuel Type", "نوع الوقود", (vehicle, lang) => vehicle.FuelType == null
            ? string.Empty
            : lang.IsArabic()
                ? vehicle.FuelType.NameAr ?? string.Empty
                : vehicle.FuelType.NameEn ?? string.Empty),
        new("plate", "Plate Letters And Plate Numbers", "حروف وأرقام اللوحة", (vehicle, _) => $"{vehicle.PlateLetters} {vehicle.PlateNo}".Trim()),
        new("vehicleBrand", "Vehicle Brand", "ماركة المركبة", (vehicle, lang) => vehicle.VehicleBrandModel?.VehicleBrand == null
            ? string.Empty
            : lang.IsArabic()
                ? vehicle.VehicleBrandModel.VehicleBrand.NameAr ?? string.Empty
                : vehicle.VehicleBrandModel.VehicleBrand.NameEn ?? string.Empty),
        new("vehicleModel", "Vehicle Model", "موديل المركبة", (vehicle, lang) => vehicle.VehicleBrandModel == null
            ? string.Empty
            : lang.IsArabic()
                ? vehicle.VehicleBrandModel.NameAr ?? string.Empty
                : vehicle.VehicleBrandModel.NameEn ?? string.Empty),
        new("modelYear", "Model Year", "سنة الصنع", (vehicle, _) => vehicle.MakeYear),
        new("capacityUnit", "Capacity Unit", "وحدة السعة", (vehicle, lang) => vehicle.CapacityUnit == null
            ? string.Empty
            : lang.IsArabic()
                ? vehicle.CapacityUnit.CapacityTypeAr ?? string.Empty
                : vehicle.CapacityUnit.CapacityTypeEn ?? string.Empty),
        new("capacity", "Capacity", "السعة", (vehicle, _) => vehicle.Capacity),
        new("vehicleColor", "Vehicle Color", "لون المركبة", (vehicle, lang) => vehicle.Color == null
            ? string.Empty
            : lang.IsArabic()
                ? vehicle.Color.NameAr ?? string.Empty
                : vehicle.Color.NameEn ?? string.Empty),
        new("sector", "Sector", "القطاع", GetSectorName),
        new("driver", "Driver", "السائق", GetDriverName),
        new("contract", "Contract", "العقد", GetContractNameAndReference),
        new("contractCompany", "Contract Company", "شركة العقد", GetContractCompanyName),
        new("status", "Status", "الحالة", (vehicle, _) => vehicle.Status.ToLocalizedDisplay()),
        new("currentLocation", "Current Location", "الموقع الحالي", GetCurrentLocation),
        new("doneBy", "Done By", "تم بواسطة", GetDoneBy)
    ];

    private static object? GetSectorName(Vehicle vehicle, ILanguageService languageService) =>
        vehicle.Sector == null
            ? string.Empty
            : languageService.IsArabic()
                ? vehicle.Sector.NameAr ?? string.Empty
                : vehicle.Sector.NameEn ?? string.Empty;

    private static object? GetDriverName(Vehicle vehicle, ILanguageService languageService) =>
        vehicle.Driver == null
            ? string.Empty
            : languageService.IsArabic()
                ? vehicle.Driver.FullNameAr ?? string.Empty
                : vehicle.Driver.FullName ?? string.Empty;

    private static object? GetContractNameAndReference(Vehicle vehicle, ILanguageService languageService)
    {
        if (vehicle.Contract == null)
            return string.Empty;

        string name = languageService.IsArabic()
            ? vehicle.Contract.NameAr ?? vehicle.Contract.NameEn
            : vehicle.Contract.NameEn;

        return string.IsNullOrWhiteSpace(name)
            ? vehicle.Contract.ReferenceNumber
            : $"{vehicle.Contract.ReferenceNumber} - {name}";
    }

    private static object? GetContractCompanyName(Vehicle vehicle, ILanguageService languageService)
    {
        CompanyDetail? detail = vehicle.Contract?.Company?.CompanyDetail;
        if (detail == null)
            return string.Empty;

        return languageService.IsArabic()
            ? detail.NameAr ?? detail.NameEn ?? string.Empty
            : detail.NameEn ?? string.Empty;
    }

    private static VehicleFacility? ResolveCurrentLocation(Vehicle vehicle) =>
        vehicle.CurrentServiceFacility ?? vehicle.AssignedParkingFacility;

    private static object? GetCurrentLocation(Vehicle vehicle, ILanguageService _)
    {
        VehicleFacility? facility = ResolveCurrentLocation(vehicle);
        if (facility == null)
            return string.Empty;

        return $"{facility.FacilityType.ToLocalizedDisplay()} - {facility.Name}";
    }

    private static object? GetDoneBy(Vehicle vehicle, ILanguageService _)
    {
        VehicleFacility? facility = ResolveCurrentLocation(vehicle);
        if (facility == null)
            return string.Empty;

        VehicleFacilityVisit? enterVisit = vehicle.VehicleFacilityVisits
            .FirstOrDefault(vv =>
                vv.Event == VehicleFacilityVisitEvent.ENTERS_LOCATION
                && vv.VehicleFacilityId == facility.Id);

        return enterVisit?.AddedByAdmin?.FullName ?? string.Empty;
    }
}
