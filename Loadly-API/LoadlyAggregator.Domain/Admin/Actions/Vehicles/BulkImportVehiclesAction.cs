using System.Globalization;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Vehicles;

public class BulkImportVehiclesAction
{
    private static readonly string[] VehicleExcelHeaders =
    {
        "Vehicle category", "vehicle types", "Plate Letters", "Plate Numbers",
        "Vehicle brand model", "Model Year", "Capacity unit",
        "The capacity", "Vehicle color", "VIN number", "Initial odometer reading"
    };

    private readonly IVehicleRepository _vehicleRepository;
    private readonly IColorRepository _colorRepository;
    private readonly IVehicleCapacityUnitRepository _capacityUnitRepository;
    private readonly IVehicleBrandModelRepository _vehicleBrandModelRepository;
    private readonly IVehicleTypeRepository _vehicleTypeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IExcelParseService _excelParseService;
    private readonly VehiclePlateAliasRecorder _vehiclePlateAliasRecorder;

    public BulkImportVehiclesAction(
        IVehicleRepository vehicleRepository,
        IColorRepository colorRepository,
        IVehicleCapacityUnitRepository capacityUnitRepository,
        IVehicleBrandModelRepository vehicleBrandModelRepository,
        IVehicleTypeRepository vehicleTypeRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IExcelParseService excelParseService,
        VehiclePlateAliasRecorder vehiclePlateAliasRecorder)
    {
        _vehicleRepository = vehicleRepository;
        _colorRepository = colorRepository;
        _capacityUnitRepository = capacityUnitRepository;
        _vehicleBrandModelRepository = vehicleBrandModelRepository;
        _vehicleTypeRepository = vehicleTypeRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _excelParseService = excelParseService;
        _vehiclePlateAliasRecorder = vehiclePlateAliasRecorder;
    }

    public async Task<BulkImportVehiclesResultDto> ExecuteAsync(ImportVehiclesFromExcelDto dto)
    {
        await using var stream = dto.OpenStream();
        var (actualHeaders, rows) = await _excelParseService.ParseAsync(stream, VehicleExcelHeaders);

        if (actualHeaders == null || actualHeaders.Count != VehicleExcelHeaders.Length ||
            !actualHeaders.Zip(VehicleExcelHeaders, (a, e) => string.Equals(NormalizeHeader(a), NormalizeHeader(e), StringComparison.OrdinalIgnoreCase)).All(x => x))
            throw new ValidationException("validation.excel_headers_mismatch");

        if (rows == null || rows.Count == 0)
            throw new ValidationException("validation.excel_min_one_data_row");

        var plates = CollectPlates(rows);
        var colorNames = Collect(rows, "Vehicle color").Select(s => s.ToLowerInvariant()).Distinct().ToList();
        var capacityUnitNames = Collect(rows, "Capacity unit").Select(s => s.ToLowerInvariant()).Distinct().ToList();
        var brandModelNames = Collect(rows, "Vehicle brand model").Select(s => s.ToLowerInvariant()).Distinct().ToList();

        var parsedVehicleTypeKeys = ParseVehicleTypeKeys(rows);
        var distinctKeys = parsedVehicleTypeKeys.Values.Distinct().ToList();

        var vehicleTypeByKey = distinctKeys.Count == 0
            ? new Dictionary<(VehicleTypeCategory, string), VehicleType>()
            : await _vehicleTypeRepository.GetMapByCategoryAndNamesAsync(distinctKeys);

        var missingKeys = distinctKeys
            .Where(k => !vehicleTypeByKey.ContainsKey(k))
            .Select(k => $"{k.Item2} ({k.Item1})")
            .ToList();

        if (missingKeys.Count > 0)
            throw new ValidationException(
                "validation.no_active_vehicle_types_for",
                string.Join(", ", missingKeys));

        var existingByPlate = plates.Count == 0
            ? new Dictionary<(string PlateNo, string PlateLetters), Vehicle>()
            : await _vehicleRepository.GetMapByPlatesAsync(plates);

        var colorIdByName = colorNames.Count == 0
            ? new Dictionary<string, long>()
            : await _colorRepository.GetIdMapByNamesAsync(colorNames);

        var capacityUnitIdByName = capacityUnitNames.Count == 0
            ? new Dictionary<string, long>()
            : await _capacityUnitRepository.GetIdMapByNamesAsync(capacityUnitNames);

        var brandModelIdByName = brandModelNames.Count == 0
            ? new Dictionary<string, long>()
            : await _vehicleBrandModelRepository.GetIdMapByNamesAsync(brandModelNames);

        ValidateRows(rows);

        var result = new BulkImportVehiclesResultDto();

        try
        {
            await _unitOfWork.BeginTransactionAsync();
            var addedBy = _currentUserService.GetCurrentUserId();

            var vehiclesToUpdate = new List<Vehicle>();

            for (var i = 0; i < rows.Count; i++)
            {
                var row = rows[i];
                var plateKey = GetPlateKey(row);
                if (!existingByPlate.TryGetValue(plateKey, out var vehicle))
                    continue;

                if (parsedVehicleTypeKeys.TryGetValue(i, out var updateKey) &&
                    vehicleTypeByKey.TryGetValue(updateKey, out var updateVehicleType))
                    vehicle.VehicleTypeId = updateVehicleType.Id;

                var previousPlateNo = vehicle.PlateNo;
                var previousPlateLetters = vehicle.PlateLetters;
                ApplyImportRowToExistingVehicle(vehicle, row, colorIdByName, capacityUnitIdByName, brandModelIdByName);
                if (!string.Equals(previousPlateNo, vehicle.PlateNo, StringComparison.OrdinalIgnoreCase)
                    || !string.Equals(previousPlateLetters, vehicle.PlateLetters, StringComparison.OrdinalIgnoreCase))
                {
                    await _vehiclePlateAliasRecorder.RecordSupersededPlateAsync(
                        vehicle.Id,
                        previousPlateNo,
                        previousPlateLetters,
                        addedBy);
                }

                vehiclesToUpdate.Add(vehicle);
            }

            var newVehicleRowPairs = new List<(Vehicle Vehicle, Dictionary<string, string> Row)>();
            for (var i = 0; i < rows.Count; i++)
            {
                var row = rows[i];
                var plateKey = GetPlateKey(row);
                if (existingByPlate.ContainsKey(plateKey))
                    continue;

                var vehicleTypeId = parsedVehicleTypeKeys.TryGetValue(i, out var createKey) &&
                                    vehicleTypeByKey.TryGetValue(createKey, out var createVehicleType)
                    ? createVehicleType.Id
                    : vehicleTypeByKey.Values.First().Id;

                var vehicle = CreateVehicleFromImportRow(
                    row, vehicleTypeId, addedBy, colorIdByName, capacityUnitIdByName, brandModelIdByName);
                newVehicleRowPairs.Add((vehicle, row));
            }

            if (vehiclesToUpdate.Count > 0) await _vehicleRepository.UpdateRangeAsync(vehiclesToUpdate);
            if (newVehicleRowPairs.Count > 0) await _vehicleRepository.AddRangeAsync(newVehicleRowPairs.Select(x => x.Vehicle));

            await _vehicleRepository.SaveChangesAsync();
            result.Updated = vehiclesToUpdate.Count;

            if (newVehicleRowPairs.Count > 0)
            {
                result.Created = newVehicleRowPairs.Count;
            }

            await _unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }

        return result;
    }

    private static Vehicle CreateVehicleFromImportRow(
        Dictionary<string, string> row,
        long vehicleTypeId,
        long? addedBy,
        IReadOnlyDictionary<string, long> colorIdByName,
        IReadOnlyDictionary<string, long> capacityUnitIdByName,
        IReadOnlyDictionary<string, long> brandModelIdByName)
    {
        var vin = Get(row, "VIN number");
        var vehicle = new Vehicle
        {
            PlateNo = Get(row, "Plate Numbers"),
            PlateLetters = Get(row, "Plate Letters"),
            VinNo = string.IsNullOrWhiteSpace(vin) ? null : vin.Trim(),
            VehicleTypeId = vehicleTypeId,
            Status = VehicleStatus.Available,
            AddedBy = addedBy
        };

        ApplyImportRowToExistingVehicle(vehicle, row, colorIdByName, capacityUnitIdByName, brandModelIdByName);
        return vehicle;
    }

    private static void ApplyImportRowToExistingVehicle(
        Vehicle vehicle,
        Dictionary<string, string> row,
        IReadOnlyDictionary<string, long> colorIdByName,
        IReadOnlyDictionary<string, long> capacityUnitIdByName,
        IReadOnlyDictionary<string, long> brandModelIdByName)
    {
        var plateLetters = Get(row, "Plate Letters");
        if (!string.IsNullOrEmpty(plateLetters))
            vehicle.PlateLetters = plateLetters;

        var plateNo = Get(row, "Plate Numbers");
        if (!string.IsNullOrEmpty(plateNo))
            vehicle.PlateNo = plateNo;

        var brandModel = TrimLower(Get(row, "Vehicle brand model"));
        if (!string.IsNullOrEmpty(brandModel) && brandModelIdByName.TryGetValue(brandModel, out var brandModelId))
            vehicle.BrandModelID = brandModelId;

        var makeYear = TryParseInt(Get(row, "Model Year"));
        if (makeYear.HasValue)
            vehicle.MakeYear = makeYear.Value;

        var capacityUnit = TrimLower(Get(row, "Capacity unit"));
        if (!string.IsNullOrEmpty(capacityUnit) && capacityUnitIdByName.TryGetValue(capacityUnit, out var capacityUnitId))
            vehicle.CapacityUnitId = capacityUnitId;

        var capacity = TryParseDecimal(Get(row, "The capacity"));
        if (capacity.HasValue)
            vehicle.Capacity = capacity.Value;

        var color = TrimLower(Get(row, "Vehicle color"));
        if (!string.IsNullOrEmpty(color) && colorIdByName.TryGetValue(color, out var colorId))
            vehicle.ColorId = colorId;

        var odometer = TryParseDecimal(Get(row, "Initial odometer reading"));
        if (odometer.HasValue)
            vehicle.InitialOdometerReading = odometer.Value;

        var vin = Get(row, "VIN number");
        if (!string.IsNullOrWhiteSpace(vin))
            vehicle.VinNo = vin.Trim();
    }

    private static Dictionary<int, (VehicleTypeCategory Category, string Name)> ParseVehicleTypeKeys(
        List<Dictionary<string, string>> rows)
    {
        var result = new Dictionary<int, (VehicleTypeCategory, string)>();
        var invalidReasons = new List<string>();

        for (var i = 0; i < rows.Count; i++)
        {
            var rowLabel = $"Row {i + 1}";
            var rawCategory = Get(rows[i], "Vehicle category");
            var rawTypeName = Get(rows[i], "vehicle types");

            if (string.IsNullOrWhiteSpace(rawCategory))
            {
                invalidReasons.Add($"{rowLabel}: Vehicle category is required");
                continue;
            }

            if (string.IsNullOrWhiteSpace(rawTypeName))
            {
                invalidReasons.Add($"{rowLabel}: vehicle types is required");
                continue;
            }

            var normalizedCategory = rawCategory.Trim().Replace(" ", "");
            if (!Enum.TryParse<VehicleTypeCategory>(normalizedCategory, ignoreCase: true, out var category))
            {
                var validValues = string.Join(", ", Enum.GetNames<VehicleTypeCategory>());
                invalidReasons.Add($"{rowLabel}: Invalid vehicle category '{rawCategory.Trim()}'. Valid values: {validValues}");
                continue;
            }

            result[i] = (category, rawTypeName.Trim().ToLowerInvariant());
        }

        if (invalidReasons.Count > 0)
            throw new ValidationException("validation.bulk_import_row_errors", string.Join("; ", invalidReasons));

        return result;
    }

    private static void ValidateRows(List<Dictionary<string, string>> rows)
    {
        var invalidReasons = new List<string>();
        var batchPlates = new HashSet<(string PlateNo, string PlateLetters)>();

        for (var rowIndex = 0; rowIndex < rows.Count; rowIndex++)
        {
            var row = rows[rowIndex];
            var rowLabel = $"Row {rowIndex + 1}";
            var plateNo = Get(row, "Plate Numbers");
            var plateLetters = Get(row, "Plate Letters");

            if (string.IsNullOrWhiteSpace(plateNo))
            {
                invalidReasons.Add($"{rowLabel}: Plate Numbers is required");
                continue;
            }

            if (string.IsNullOrWhiteSpace(plateLetters))
            {
                invalidReasons.Add($"{rowLabel}: Plate Letters is required");
                continue;
            }

            var plateKey = GetPlateKey(row);
            if (!batchPlates.Add(plateKey))
                invalidReasons.Add($"{rowLabel}: Duplicate plate in file");
        }

        if (invalidReasons.Count > 0)
            throw new ValidationException("validation.bulk_import_row_errors", string.Join("; ", invalidReasons));
    }

    private static List<(string PlateNo, string PlateLetters)> CollectPlates(List<Dictionary<string, string>> rows)
    {
        var set = new HashSet<(string PlateNo, string PlateLetters)>();
        foreach (var row in rows)
        {
            var plateKey = GetPlateKey(row);
            if (!string.IsNullOrWhiteSpace(plateKey.PlateNo) && !string.IsNullOrWhiteSpace(plateKey.PlateLetters))
                set.Add(plateKey);
        }
        return set.ToList();
    }

    private static (string PlateNo, string PlateLetters) GetPlateKey(IReadOnlyDictionary<string, string> row) =>
        (TrimLower(Get(row, "Plate Numbers")), TrimLower(Get(row, "Plate Letters")));

    private static List<string> Collect(List<Dictionary<string, string>> rows, string key)
    {
        var set = new HashSet<string>();
        foreach (var row in rows)
        {
            var v = Get(row, key);
            if (!string.IsNullOrWhiteSpace(v)) set.Add(v.Trim());
        }
        return set.ToList();
    }

    private static string Get(IReadOnlyDictionary<string, string> row, string key) =>
        row.TryGetValue(key, out var v) ? (v ?? "").Trim() : "";

    private static string TrimLower(string s) => string.IsNullOrWhiteSpace(s) ? "" : s.Trim().ToLowerInvariant();

    private static string NormalizeHeader(string? value) =>
        string.IsNullOrEmpty(value) ? string.Empty : value.Trim().Replace('\u00A0', ' ');

    private static int? TryParseInt(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : (int.TryParse(value.Trim(), out var i) ? i : null);

    private static decimal? TryParseDecimal(string value) =>
        string.IsNullOrWhiteSpace(value)
            ? null
            : decimal.TryParse(value.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var d)
                ? d
                : null;
}
