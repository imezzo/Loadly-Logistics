using System.Globalization;
using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Trips;

public class BulkImportTripsAction
{
    private static readonly CultureInfo ImportParseCulture = CultureInfo.InvariantCulture;

    private static readonly string[] TripExcelHeaders =
    {
        "DestinationId", "StartDate", "EndDate", "FinancialDate", "ContractDistanceKm", "ActualCargoWeightTons",
        "CargoPrice", "SpotPrice", "ContractorPrice", "HelpersCount", "ContactPersonName", "ContactPersonMobile",
        "PlateNo", "PlateLetters", "FromAddress", "ToAddress"
    };

    private readonly ITripRepository _tripRepository;
    private readonly IContractRepository _contractRepository;
    private readonly IContractDestinationRepository _contractDestinationRepository;
    private readonly IContractVehicleTypeRepository _contractVehicleTypeRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ITripContractDestinationRepository _tripContractDestinationRepository;
    private readonly IExcelParseService _excelParseService;
    private readonly ITranslationService _translationService;
    private readonly TripVehicleAssignmentSnapshotRecorder _TripVehicleAssignmentSnapshotRecorder;

    public BulkImportTripsAction(
        ITripRepository tripRepository,
        IContractRepository contractRepository,
        IContractDestinationRepository contractDestinationRepository,
        IContractVehicleTypeRepository contractVehicleTypeRepository,
        IVehicleRepository vehicleRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ITripContractDestinationRepository tripContractDestinationRepository,
        IExcelParseService excelParseService,
        ITranslationService translationService,
        TripVehicleAssignmentSnapshotRecorder TripVehicleAssignmentSnapshotRecorder)
    {
        _tripRepository = tripRepository;
        _contractRepository = contractRepository;
        _contractDestinationRepository = contractDestinationRepository;
        _contractVehicleTypeRepository = contractVehicleTypeRepository;
        _vehicleRepository = vehicleRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _tripContractDestinationRepository = tripContractDestinationRepository;
        _excelParseService = excelParseService;
        _translationService = translationService;
        _TripVehicleAssignmentSnapshotRecorder = TripVehicleAssignmentSnapshotRecorder;
    }

    public async Task<BulkImportTripsResultDto> ExecuteAsync(ImportTripsFromExcelDto dto)
    {
        var contract = await LoadActiveContractOrThrowAsync(dto.ContractId);
        var dataRows = await ParseAndExtractDataRowsAsync(dto);

        var lookups = await PreloadReferenceDataAsync(dto.ContractId, dataRows);

        var (trips, tripDestinations, errors) = BuildTripsAndCollectErrors(dataRows, contract, lookups);
        DetectIntraFileOverlaps(trips, errors);

        if (errors.Count > 0)
            throw new ValidationException("validation.bulk_import_row_errors", string.Join("; ", errors));

        return await PersistTripsAsync(trips, tripDestinations);
    }

    // -- Phase 1: contract preflight ----------------------------------------------------------

    private async Task<Contract> LoadActiveContractOrThrowAsync(long contractId)
    {
        var contract = await _contractRepository.GetByIdAsync(contractId)
            ?? throw new NotFoundException("contracts.not_found_with_id", contractId);
        TripValidator.ValidateContractIsActive(contract);
        return contract;
    }

    // -- Phase 2: parse file, validate headers, collect data rows -----------------------------

    private async Task<List<DataRow>> ParseAndExtractDataRowsAsync(ImportTripsFromExcelDto dto)
    {
        await using var stream = dto.OpenStream();
        var (actualHeaders, rows) = await _excelParseService.ParseAsync(stream, TripExcelHeaders);

        if (!ExcelHeaderMatcher.Matches(actualHeaders, TripExcelHeaders))
            throw new ValidationException("validation.excel_headers_mismatch");

        var dataRows = rows?
            .Select((row, index) => new DataRow(row, index))
            .Where(x => !string.IsNullOrWhiteSpace(Cell(x.Row, "StartDate")) || !string.IsNullOrWhiteSpace(Cell(x.Row, "EndDate")))
            .ToList() ?? new List<DataRow>();

        if (dataRows.Count == 0)
            throw new ValidationException("validation.excel_min_one_data_row");

        return dataRows;
    }

    // -- Phase 3: batch-load reference data referenced across all rows ------------------------

    private async Task<ImportLookups> PreloadReferenceDataAsync(long contractId, IReadOnlyList<DataRow> dataRows)
    {
        var destinationsById = await PreloadDestinationsAsync(contractId, dataRows);
        var vehiclesByPlate = await PreloadVehiclesAsync(dataRows);
        var contractVehicleTypeByVehicleTypeId = await PreloadContractVehicleTypesAsync(contractId, vehiclesByPlate);

        return new ImportLookups(destinationsById, vehiclesByPlate, contractVehicleTypeByVehicleTypeId);
    }

    private async Task<Dictionary<long, ContractDestination>> PreloadDestinationsAsync(
        long contractId,
        IReadOnlyList<DataRow> dataRows)
    {
        var destinationIds = dataRows
            .Select(x => Cell(x.Row, "DestinationId"))
            .Select(s => long.TryParse(s, out var id) && id > 0 ? id : (long?)null)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct()
            .ToList();

        return await _contractDestinationRepository.GetMapByContractAndIdsAsync(contractId, destinationIds);
    }

    private async Task<Dictionary<(string PlateNo, string PlateLetters), Vehicle>> PreloadVehiclesAsync(
        IReadOnlyList<DataRow> dataRows)
    {
        var requestedPlates = dataRows
            .Select(x => (PlateNo: Cell(x.Row, "PlateNo"), PlateLetters: Cell(x.Row, "PlateLetters")))
            .Where(p => !string.IsNullOrWhiteSpace(p.PlateNo) && !string.IsNullOrWhiteSpace(p.PlateLetters))
            .Select(p => (PlateNo: p.PlateNo.Trim(), PlateLetters: p.PlateLetters.Trim()))
            .Distinct()
            .ToList();

        return requestedPlates.Count == 0
            ? new Dictionary<(string PlateNo, string PlateLetters), Vehicle>()
            : await _vehicleRepository.GetMapByPlatesAsync(requestedPlates);
    }

    private async Task<Dictionary<long, ContractVehicleType>> PreloadContractVehicleTypesAsync(
        long contractId,
        IReadOnlyDictionary<(string PlateNo, string PlateLetters), Vehicle> vehiclesByPlate)
    {
        // For heads with multiple tails of different types, include every tail's type so the
        // per-row resolver can fail cleanly on ambiguity rather than silently picking one.
        var candidateVehicleTypeIds = vehiclesByPlate.Values
            .SelectMany(v => v.TailVehicles is { Count: > 0 }
                ? v.TailVehicles.Select(t => t.VehicleTypeId)
                : new[] { v.VehicleTypeId })
            .Distinct()
            .ToHashSet();

        if (candidateVehicleTypeIds.Count == 0)
            return new Dictionary<long, ContractVehicleType>();

        var cvts = await _contractVehicleTypeRepository.FindAsync(cvt =>
            cvt.ContractId == contractId && candidateVehicleTypeIds.Contains(cvt.VehicleTypeId));

        return cvts.ToDictionary(cvt => cvt.VehicleTypeId, cvt => cvt);
    }

    // -- Phase 4: per-row validation + Trip construction --------------------------------------

    private (List<Trip> Trips, List<ContractDestination?> TripDestinations, List<string> Errors) BuildTripsAndCollectErrors(
        IReadOnlyList<DataRow> dataRows,
        Contract contract,
        ImportLookups lookups)
    {
        long? dispatcherId = _currentUserService.GetCurrentUserId();
        var trips = new List<Trip>();
        var tripDestinations = new List<ContractDestination?>();
        var errors = new List<string>();

        foreach (var dataRow in dataRows)
        {
            var result = TryBuildTripFromRow(dataRow, contract, dispatcherId, lookups);
            if (result.Error is { } error)
                errors.Add(error);
            else if (result.Trip is { } trip)
            {
                trips.Add(trip);
                tripDestinations.Add(result.TripDestination);
            }
        }

        return (trips, tripDestinations, errors);
    }

    private RowResult TryBuildTripFromRow(
        DataRow dataRow,
        Contract contract,
        long? dispatcherId,
        ImportLookups lookups)
    {
        var row = dataRow.Row;
        var excelRowNumber = dataRow.ExcelRowNumber;

        if (!TryParseImportSheetDate(Cell(row, "StartDate"), out var startDate)
            || !TryParseImportSheetDate(Cell(row, "EndDate"), out var endDate))
        {
            return RowResult.Failure(RowErrorForReasonKey(excelRowNumber, "trips.import_start_end_dates_invalid"));
        }

        startDate = DateTimeHelper.ToUtcNullable(startDate)!.Value;
        endDate = DateTimeHelper.ToUtcNullable(endDate)!.Value;

        DateOnly? financialDate = null;
        if (ContractCalculationHelper.IsMonthlyContract(contract.ContractType))
        {
            var financialDateRaw = Cell(row, "FinancialDate");
            if (!string.IsNullOrWhiteSpace(financialDateRaw))
            {
                if (!TryParseImportSheetDateOnly(financialDateRaw, out var parsedFinancialDate))
                    return RowResult.Failure(RowErrorForReasonKey(excelRowNumber, "trips.import_financial_date_invalid"));

                financialDate = parsedFinancialDate;
            }
        }

        if (!TryResolveDestination(row, lookups.DestinationsById, out var destinationId, out var destinationErrorKey))
            return RowResult.Failure(RowErrorForReasonKey(excelRowNumber, destinationErrorKey!));

        if (!TryResolveVehicleAssignment(row, lookups, startDate, endDate, excelRowNumber,
                out var headVehicle, out var contractVehicleType, out var assignmentError))
        {
            return RowResult.Failure(assignmentError!);
        }

        var contractDistanceKm = ParseOptionalDecimal(row, "ContractDistanceKm");
        var actualCargoWeightTons = ParseOptionalDecimal(row, "ActualCargoWeightTons");
        var cargoPrice = ParseOptionalDecimal(row, "CargoPrice");
        var spotPrice = ParseOptionalDecimal(row, "SpotPrice");
        var contractorPrice = ParseOptionalDecimal(row, "ContractorPrice");
        var helpersCount = ParseOptionalInt(row, "HelpersCount");

        try
        {
            TripValidator.ValidateDateRange(startDate, endDate);
            //TripValidator.ValidateTripStartDateNotInPast(startDate);
            TripValidator.ValidateTripDatesWithinContractRange(contract, startDate, endDate);
            TripValidator.ValidateActualCargoWeightTons(contract, actualCargoWeightTons);
            TripValidator.ValidateCargoPrice(contract, cargoPrice);
            TripValidator.ValidateSpotPrice(contract, spotPrice);
            TripValidator.ValidateContractDistanceKm(contract, contractDistanceKm);
            TripValidator.ValidateDestinationRequired(contract, destinationId);
            TripValidator.ValidateFinancialDate(contract, financialDate);
        }
        catch (Exception ex) when (ex is ValidationException or LogicalException)
        {
            return RowResult.Failure(RowErrorForLocalizedException(excelRowNumber, ex));
        }

        var tripDestination = destinationId is long destinationKey
            && lookups.DestinationsById.TryGetValue(destinationKey, out var contractDestination)
                ? contractDestination
                : null;

        var waypoints = BuildWaypointsForTrip(row, tripDestination);
        if (waypoints.Count > 0)
        {
            try
            {
                TripValidator.ValidateWaypoints(waypoints.ConvertAll(w => new TripWaypointDto
                {
                    Sequence = w.Sequence,
                    StopType = w.StopType,
                    Latitude = w.Latitude,
                    Longitude = w.Longitude,
                    AddressName = w.AddressName,
                    GooglePlaceId = w.GooglePlaceId
                }));
            }
            catch (Exception ex) when (ex is ValidationException or LogicalException)
            {
                return RowResult.Failure(RowErrorForLocalizedException(excelRowNumber, ex));
            }
        }

        var canBeScheduled = TripValidator.HasRequiredAssignmentAttributes(
            headVehicle?.DriverId,
            headVehicle?.Id ?? 0,
            startDate,
            endDate);

        var contactPersonName = Cell(row, "ContactPersonName");
        var contactPersonMobile = Cell(row, "ContactPersonMobile");

        var numberOfDays = TripContractDestinationHelper.ResolveNumberOfDays(startDate, endDate, tripDestination?.NumberOfDays);
        var resolvedFinancialDate = TripFinancialDateResolver.Resolve(contract, startDate, financialDate);

        var contractSlotId = TripContractSlotHelper.ResolveContractSlotIdForTrip(
            contract,
            headVehicle?.Id,
            resolvedFinancialDate,
            numberOfDays,
            startDate);

        try
        {
            TripValidator.ValidateSubmittedContractorPrice(
                contract,
                TripStatus.Pending,
                contractorPrice,
                actualCargoWeightTons,
                contractVehicleType,
                tripDestination,
                contractSlotId);
        }
        catch (Exception ex) when (ex is ValidationException or LogicalException)
        {
            return RowResult.Failure(RowErrorForLocalizedException(excelRowNumber, ex));
        }

        return RowResult.Success(new Trip
        {
            ContractId = contract.Id,
            NumberOfDays = numberOfDays,
            ContractVehicleTypeId = contractVehicleType?.Id,
            ContractSlotId = contractSlotId,
            VehicleId = headVehicle?.Id,
            DriverId = headVehicle?.DriverId,
            StartDate = startDate,
            EndDate = endDate,
            FinancialDate = resolvedFinancialDate,
            Status = canBeScheduled ? TripStatus.Scheduled : TripStatus.Pending,
            TripDocumentStatus = TripDocumentStatus.Pending,
            DispatcherId = dispatcherId,
            ContractDistanceKm = contractDistanceKm,
            ActualCargoWeightTons = actualCargoWeightTons,
            CargoPrice = cargoPrice,
            SpotPrice = spotPrice,
            ContractorPrice = contractorPrice,
            HelpersCount = helpersCount,
            ContactPersonName = string.IsNullOrWhiteSpace(contactPersonName) ? null : contactPersonName,
            ContactPersonMobile = string.IsNullOrWhiteSpace(contactPersonMobile) ? null : contactPersonMobile,
            IsBackfill = false,
            Waypoints = waypoints.Count > 0 ? waypoints : null
        }, tripDestination);
    }

    private static bool TryResolveDestination(
        IReadOnlyDictionary<string, string> row,
        IReadOnlyDictionary<long, ContractDestination> destinationsById,
        out long? destinationId,
        out string? errorKey)
    {
        destinationId = null;
        errorKey = null;

        var destRaw = Cell(row, "DestinationId");
        if (string.IsNullOrWhiteSpace(destRaw))
            return true;

        if (!long.TryParse(destRaw, out var destId) || destId <= 0 || !destinationsById.ContainsKey(destId))
        {
            errorKey = "trips.import_destination_invalid_for_contract";
            return false;
        }

        destinationId = destId;
        return true;
    }

    private bool TryResolveVehicleAssignment(
        IReadOnlyDictionary<string, string> row,
        ImportLookups lookups,
        DateTime startDate,
        DateTime endDate,
        int excelRowNumber,
        out Vehicle? headVehicle,
        out ContractVehicleType? contractVehicleType,
        out string? error)
    {
        headVehicle = null;
        contractVehicleType = null;
        error = null;

        var plateNoRaw = Cell(row, "PlateNo");
        var plateLettersRaw = Cell(row, "PlateLetters");

        // No plate provided -> trip stays unassigned (Pending). This is a valid import shape.
        if (string.IsNullOrWhiteSpace(plateNoRaw) || string.IsNullOrWhiteSpace(plateLettersRaw))
            return true;

        var plateKey = (PlateNo: plateNoRaw.Trim().ToLower(), PlateLetters: plateLettersRaw.Trim().ToLower());
        if (!lookups.VehiclesByPlate.TryGetValue(plateKey, out var vehicleFromPlate))
        {
            error = RowErrorForReasonKey(excelRowNumber, "trips.import_vehicle_not_found_for_plate");
            return false;
        }

        if (!TripContractVehicleTypeHelper.TryResolveFromCurrentlyAttachedTail(vehicleFromPlate, out var vehicleTypeId))
        {
            error = RowErrorForReasonKey(excelRowNumber, "trips.import_head_has_ambiguous_tail_types");
            return false;
        }

        if (!lookups.ContractVehicleTypeByVehicleTypeId.TryGetValue(vehicleTypeId, out contractVehicleType))
        {
            error = RowErrorForReasonKey(excelRowNumber, "trips.import_contract_vehicle_type_not_found");
            return false;
        }

        headVehicle = vehicleFromPlate;
        try
        {
            TripValidator.ValidateHeadVehicle(headVehicle);
            var headDriver = headVehicle.Driver
                ?? throw new NotFoundException("drivers.not_found_with_id", headVehicle.DriverId!.Value);
            TripValidator.ValidateIsActiveDriver(headDriver);
            TripValidator.ValidateNoOverlappingHeadVehicleTrips(_tripRepository, headVehicle.Id, startDate, endDate);
            TripValidator.ValidateNoOverlappingDriverTrips(_tripRepository, headDriver.Id, startDate, endDate);
            return true;
        }
        catch (Exception ex) when (ex is ValidationException or LogicalException or NotFoundException)
        {
            error = RowErrorForLocalizedException(excelRowNumber, ex);
            return false;
        }
    }

    // -- Phase 5: surface within-file vehicle/driver collisions -------------------------------

    private void DetectIntraFileOverlaps(IReadOnlyList<Trip> trips, List<string> errors)
    {
        for (int i = 0; i < trips.Count; i++)
        {
            for (int j = i + 1; j < trips.Count; j++)
            {
                AppendOverlapIfPresent(trips[i], trips[j], i + 1, j + 1, t => t.VehicleId, "trips.import_overlap_head_vehicle", errors);
                AppendOverlapIfPresent(trips[i], trips[j], i + 1, j + 1, t => t.DriverId, "trips.import_overlap_driver", errors);
            }
        }
    }

    private void AppendOverlapIfPresent(
        Trip earlier,
        Trip later,
        int earlierFileIndex,
        int laterFileIndex,
        Func<Trip, long?> idSelector,
        string translationKey,
        List<string> errors)
    {
        var earlierId = idSelector(earlier);
        var laterId = idSelector(later);
        if (!earlierId.HasValue || !laterId.HasValue || earlierId != laterId)
            return;

        if (!earlier.StartDate.HasValue || !earlier.EndDate.HasValue
            || !later.StartDate.HasValue || !later.EndDate.HasValue)
            return;

        if (!TripValidator.HasOverlappingDateRanges(
                earlier.StartDate.Value, earlier.EndDate.Value,
                later.StartDate.Value, later.EndDate.Value))
            return;

        errors.Add(_translationService.GetTranslation(translationKey, earlierId.Value, earlierFileIndex, laterFileIndex));
    }

    // -- Phase 6: transactional save ----------------------------------------------------------

    private async Task<BulkImportTripsResultDto> PersistTripsAsync(List<Trip> trips, List<ContractDestination?> tripDestinations)
    {
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            foreach (var trip in trips.OrderBy(t => t.StartDate!.Value))
                await _tripRepository.AddAsync(trip);

            await _tripRepository.SaveChangesAsync();

            long addedBy = _currentUserService.GetCurrentUserId() ?? 0;
            for (int i = 0; i < trips.Count; i++)
            {
                ContractDestination? destination = tripDestinations[i];
                if (destination == null)
                    continue;

                await _tripContractDestinationRepository.AddAsync(new TripContractDestination
                {
                    TripId = trips[i].Id,
                    DestinationId = destination.Id,
                    OutboundDistanceKm = destination.ExpectedKms ?? 0,
                    AddedBy = addedBy,
                });
            }

            await _tripContractDestinationRepository.SaveChangesAsync();

            foreach (var trip in trips.Where(t => t.VehicleId is > 0))
            {
                var headVehicle = await _vehicleRepository.GetByIdAsync(trip.VehicleId!.Value);
                await _TripVehicleAssignmentSnapshotRecorder.SyncForTripAsync(
                    trip.Id,
                    headVehicle,
                    trip.TailVehicleId,
                    tailVehicle: null,
                    freezeSnapshot: false,
                    addedBy: _currentUserService.GetCurrentUserId());
            }

            await _tripRepository.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return new BulkImportTripsResultDto
            {
                Created = trips.Count
            };
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    // -- Cell-level parsing / waypoint helpers ------------------------------------------------
    private static List<TripWaypoint> BuildWaypointsForTrip(
        IReadOnlyDictionary<string, string> row,
        ContractDestination? destination) =>
            destination is not null
                ? BuildWaypointsFromContractDestination(destination)
                : BuildWaypointsFromRowCells(row);

    private static List<TripWaypoint> BuildWaypointsFromContractDestination(ContractDestination destination)
    {
        var waypoints = new List<TripWaypoint>();
        if (destination.StartWaypointNavigation is { } start)
            waypoints.Add(ToTripWaypoint(start, waypoints.Count, TripWaypointStopType.Pickup));

        if (destination.EndWaypointNavigation is { } end)
            waypoints.Add(ToTripWaypoint(end, waypoints.Count, TripWaypointStopType.Dropoff));

        return waypoints;
    }

    private static List<TripWaypoint> BuildWaypointsFromRowCells(IReadOnlyDictionary<string, string> row)
    {
        var waypoints = new List<TripWaypoint>();
        var fromAddress = Cell(row, "FromAddress");
        var toAddress = Cell(row, "ToAddress");

        if (!string.IsNullOrWhiteSpace(fromAddress))
        {
            waypoints.Add(new TripWaypoint
            {
                Sequence = waypoints.Count,
                StopType = TripWaypointStopType.Pickup,
                AddressName = fromAddress
            });
        }

        if (!string.IsNullOrWhiteSpace(toAddress))
        {
            waypoints.Add(new TripWaypoint
            {
                Sequence = waypoints.Count,
                StopType = TripWaypointStopType.Dropoff,
                AddressName = toAddress
            });
        }

        return waypoints;
    }

    private static TripWaypoint ToTripWaypoint(Waypoint source, int sequence, TripWaypointStopType stopType) =>
        new()
        {
            Sequence = sequence,
            StopType = stopType,
            AddressName = source.AddressName,
            GooglePlaceId = source.GooglePlaceId,
            Latitude = source.Latitude,
            Longitude = source.Longitude
        };

    private static string Cell(IReadOnlyDictionary<string, string> row, string key) =>
        row.TryGetValue(key, out var raw) ? (raw ?? string.Empty).Trim() : string.Empty;

    private static bool TryParseImportSheetDate(string? raw, out DateTime dateTime)
    {
        dateTime = default;
        return !string.IsNullOrWhiteSpace(raw)
               && DateTime.TryParse(raw.Trim(), ImportParseCulture, DateTimeStyles.AllowWhiteSpaces, out dateTime);
    }

    private static bool TryParseImportSheetDateOnly(string? raw, out DateOnly date)
    {
        date = default;
        if (string.IsNullOrWhiteSpace(raw))
            return false;

        if (DateOnly.TryParse(raw.Trim(), ImportParseCulture, DateTimeStyles.AllowWhiteSpaces, out date))
            return true;

        if (!DateTime.TryParse(raw.Trim(), ImportParseCulture, DateTimeStyles.AllowWhiteSpaces, out var dateTime))
            return false;

        date = DateOnly.FromDateTime(dateTime);
        return true;
    }

    private static decimal? ParseOptionalDecimal(IReadOnlyDictionary<string, string> row, string column)
    {
        var raw = Cell(row, column);
        return !string.IsNullOrWhiteSpace(raw)
               && decimal.TryParse(raw, NumberStyles.Number, ImportParseCulture, out var value)
            ? value
            : (decimal?)null;
    }

    private static int? ParseOptionalInt(IReadOnlyDictionary<string, string> row, string column)
    {
        var raw = Cell(row, column);
        return !string.IsNullOrWhiteSpace(raw)
               && int.TryParse(raw, NumberStyles.Integer, ImportParseCulture, out var value)
            ? value
            : (int?)null;
    }

    // -- Localized row error helpers ----------------------------------------------------------

    private string RowErrorForReasonKey(int excelRowNumber, string reasonKey, params object[] reasonArgs)
    {
        var rowLabel = _translationService.GetTranslation("trips.import_row_label", excelRowNumber);
        var reason = _translationService.GetTranslation(reasonKey, reasonArgs);
        return _translationService.GetTranslation("trips.import_row_with_reason", rowLabel, reason);
    }

    private string RowErrorForLocalizedException(int excelRowNumber, Exception ex)
    {
        var rowLabel = _translationService.GetTranslation("trips.import_row_label", excelRowNumber);
        var reason = TranslateExceptionMessage(ex);
        return _translationService.GetTranslation("trips.import_row_with_reason", rowLabel, reason);
    }

    private string TranslateExceptionMessage(Exception ex) =>
        ex is ILocalizedException localized
            ? _translationService.GetTranslation(localized.MessageKey, localized.Args)
            : _translationService.GetTranslation(ex.Message);

    // -- Internal value types ------------------------------------------------------------------

    private readonly record struct DataRow(IReadOnlyDictionary<string, string> Row, int Index)
    {
        public int ExcelRowNumber => Index + 2; // +1 for header row, +1 for 1-based indexing
    }

    private readonly record struct ImportLookups(
        IReadOnlyDictionary<long, ContractDestination> DestinationsById,
        IReadOnlyDictionary<(string PlateNo, string PlateLetters), Vehicle> VehiclesByPlate,
        IReadOnlyDictionary<long, ContractVehicleType> ContractVehicleTypeByVehicleTypeId);

    private readonly record struct RowResult(Trip? Trip, ContractDestination? TripDestination, string? Error)
    {
        public static RowResult Success(Trip trip, ContractDestination? tripDestination) => new(trip, tripDestination, null);
        public static RowResult Failure(string error) => new(null, null, error);
    }
}
