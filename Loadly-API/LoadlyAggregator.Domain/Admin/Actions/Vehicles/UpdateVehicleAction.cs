using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Vehicles;

public class UpdateVehicleAction
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IVehicleDetailRepository _vehicleDetailRepository;
    private readonly IVehicleDocumentRepository _vehicleDocumentRepository;
    private readonly IColorRepository _colorRepository;
    private readonly IVehicleCapacityUnitRepository _capacityUnitRepository;
    private readonly IVehicleTypeRepository _vehicleTypeRepository;
    private readonly IContractorRepository _contractorRepository;
    private readonly ISectorRepository _sectorRepository;
    private readonly IDocumentRepository _documentRepository;
    private readonly IVehicleBrandRepository _vehicleBrandRepository;
    private readonly IVehicleBrandModelRepository _vehicleBrandModelRepository;
    private readonly IVehicleInsuranceRepository _vehicleInsuranceRepository;
    private readonly IFuelTypeRepository _fuelTypeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMediaService _mediaService;
    private readonly VehicleAssignmentHistoryRecorder _assignmentHistoryRecorder;
    private readonly VehiclePlateAliasRecorder _vehiclePlateAliasRecorder;

    public UpdateVehicleAction(
        IVehicleRepository vehicleRepository,
        IVehicleDetailRepository vehicleDetailRepository,
        IVehicleDocumentRepository vehicleDocumentRepository,
        IColorRepository colorRepository,
        IVehicleCapacityUnitRepository capacityUnitRepository,
        IVehicleTypeRepository vehicleTypeRepository,
        IContractorRepository contractorRepository,
        ISectorRepository sectorRepository,
        IDocumentRepository documentRepository,
        IVehicleBrandRepository vehicleBrandRepository,
        IVehicleBrandModelRepository vehicleBrandModelRepository,
        IVehicleInsuranceRepository vehicleInsuranceRepository,
        IFuelTypeRepository fuelTypeRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IMediaService mediaService,
        VehicleAssignmentHistoryRecorder assignmentHistoryRecorder,
        VehiclePlateAliasRecorder vehiclePlateAliasRecorder)
    {
        _vehicleRepository = vehicleRepository;
        _vehicleDetailRepository = vehicleDetailRepository;
        _vehicleDocumentRepository = vehicleDocumentRepository;
        _colorRepository = colorRepository;
        _capacityUnitRepository = capacityUnitRepository;
        _vehicleTypeRepository = vehicleTypeRepository;
        _contractorRepository = contractorRepository;
        _sectorRepository = sectorRepository;
        _documentRepository = documentRepository;
        _vehicleBrandRepository = vehicleBrandRepository;
        _vehicleBrandModelRepository = vehicleBrandModelRepository;
        _vehicleInsuranceRepository = vehicleInsuranceRepository;
        _fuelTypeRepository = fuelTypeRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _mediaService = mediaService;
        _assignmentHistoryRecorder = assignmentHistoryRecorder;
        _vehiclePlateAliasRecorder = vehiclePlateAliasRecorder;
    }

    public async Task ExecuteAsync(UpdateVehicleDto dto)
    {
        var vehicle = await _vehicleRepository.GetByIdOrThrowAsync(dto.Id);
        await ValidatePlateUniquenessAsync(dto.PlateNo, dto.PlateLetters, dto.Id);

        await ValidateReferencesAsync(dto);
        await ValidateFuelTypeAsync(dto.FuelTypeId);
        await ValidateContractorAsync(dto.ContractorId);
        await ValidateSectorAsync(dto.SectorId);
        await ValidateVehicleRelationshipsAsync(dto);

        if (dto.Documents?.Any() == true)
        {
            foreach (var docItem in dto.Documents)
            {
                if (docItem.Id.HasValue)
                {
                    var existing = await _vehicleDocumentRepository.GetByIdAsync(docItem.Id.Value) ?? throw new NotFoundException("vehicle_documents.not_found_with_id", docItem.Id.Value);
                    if (existing.VehicleId != vehicle.Id)
                        throw new ValidationException("vehicle_documents.does_not_belong_to_vehicle", docItem.Id.Value);
                }

                var document = await _documentRepository.GetByIdAsync(docItem.DocumentId) ?? throw new NotFoundException("documents.not_found_with_id", docItem.DocumentId);
                if (document.EntityId != DocumentEntityType.VEHICLE)
                    throw new ValidationException(
                        "documents.invalid_entity_type_for_vehicle",
                        docItem.DocumentId,
                        document.EntityId);

                if (docItem.File?.Length > 0)
                {
                    DocumentFileTypeValidator.Validate(docItem.File.ContentType);
                }
            }
        }
        var uploadedFilePaths = new List<string>();
        var vehicleType = await _vehicleTypeRepository.GetByIdOrThrowAsync(dto.VehicleTypeId);
        long? previousHeadLinkOnTail = vehicle.HeadVehicleId;
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            long? addedBy = _currentUserService.GetCurrentUserId();

            if (!PlateMatches(vehicle.PlateNo, vehicle.PlateLetters, dto.PlateNo, dto.PlateLetters))
            {
                await _vehiclePlateAliasRecorder.RecordSupersededPlateAsync(
                    vehicle.Id,
                    vehicle.PlateNo,
                    vehicle.PlateLetters,
                    addedBy);
            }

            // Update vehicle
            vehicle.PlateNo = dto.PlateNo;
            vehicle.PlateLetters = dto.PlateLetters;
            vehicle.MakeYear = dto.MakeYear;
            vehicle.InsuranceId = dto.InsuranceId;
            vehicle.PurchaseDate = dto.PurchaseDate;
            vehicle.InitialOdometerReading = dto.InitialOdometerReading;
            vehicle.VinNo = dto.VinNo;
            vehicle.ColorId = dto.ColorId;
            vehicle.ContractorId = dto.ContractorId;
            vehicle.SectorId = dto.SectorId;
            vehicle.VehicleTypeId = dto.VehicleTypeId;
            vehicle.CapacityUnitId = dto.CapacityUnitId;
            vehicle.BrandModelID = dto.BrandModelID;
            vehicle.HeadVehicleId = dto.HeadVehicleId;
            vehicle.TankCapacity = dto.TankCapacity;
            vehicle.Capacity = dto.Capacity;
            vehicle.FuelTypeId = dto.FuelTypeId;
            vehicle.VehicleTypeSettings = dto.VehicleTypeSettings;
            if (dto.Status.HasValue)
            {
                vehicle.Status = (VehicleStatus)dto.Status.Value;
            }
            _vehicleRepository.Update(vehicle);

            // Update or create vehicle detail
            var existingVehicleDetail = await _vehicleDetailRepository.GetByVehicleIdAsync(vehicle.Id);

            if (!string.IsNullOrWhiteSpace(dto.GovernorateCode) ||
                !string.IsNullOrWhiteSpace(dto.InsuranceDetails) ||
                !string.IsNullOrWhiteSpace(dto.TemperatureRangeSettng) ||
                !string.IsNullOrWhiteSpace(dto.CoolingUnitSpecification) ||
                !string.IsNullOrWhiteSpace(dto.TemperatureMonotoringCapability) ||
                dto.MaximumLoadHeight.HasValue ||
                dto.InternalHeight.HasValue ||
                dto.InternalWidth.HasValue ||
                dto.InternalLength.HasValue ||
                dto.LoadingMechanismType.HasValue ||
                !string.IsNullOrWhiteSpace(dto.VehicleTypeSettings))
            {
                if (existingVehicleDetail == null)
                {
                    var vehicleDetail = new VehicleDetail
                    {
                        VehicleId = vehicle.Id,
                        GovernorateCode = dto.GovernorateCode,
                        InsuranceDetails = dto.InsuranceDetails,
                        TemperatureRangeSettng = dto.TemperatureRangeSettng,
                        CoolingUnitSpecification = dto.CoolingUnitSpecification,
                        TemperatureMonotoringCapability = dto.TemperatureMonotoringCapability,
                        MaximumLoadHeight = dto.MaximumLoadHeight.HasValue ? (decimal)dto.MaximumLoadHeight.Value : null,
                        InternalHeight = dto.InternalHeight.HasValue ? (decimal)dto.InternalHeight.Value : null,
                        InternalWidth = dto.InternalWidth.HasValue ? (decimal)dto.InternalWidth.Value : null,
                        InternalLength = dto.InternalLength.HasValue ? (decimal)dto.InternalLength.Value : null,
                        LoadingMechanismType = dto.LoadingMechanismType,
                    };
                    await _vehicleDetailRepository.AddAsync(vehicleDetail);
                }
                else
                {
                    existingVehicleDetail.GovernorateCode = dto.GovernorateCode;
                    existingVehicleDetail.InsuranceDetails = dto.InsuranceDetails;
                    existingVehicleDetail.TemperatureRangeSettng = dto.TemperatureRangeSettng;
                    existingVehicleDetail.CoolingUnitSpecification = dto.CoolingUnitSpecification;
                    existingVehicleDetail.TemperatureMonotoringCapability = dto.TemperatureMonotoringCapability;
                    existingVehicleDetail.MaximumLoadHeight = dto.MaximumLoadHeight.HasValue ? (decimal)dto.MaximumLoadHeight.Value : null;
                    existingVehicleDetail.InternalHeight = dto.InternalHeight.HasValue ? (decimal)dto.InternalHeight.Value : null;
                    existingVehicleDetail.InternalWidth = dto.InternalWidth.HasValue ? (decimal)dto.InternalWidth.Value : null;
                    existingVehicleDetail.InternalLength = dto.InternalLength.HasValue ? (decimal)dto.InternalLength.Value : null;
                    existingVehicleDetail.LoadingMechanismType = dto.LoadingMechanismType;
                    _vehicleDetailRepository.Update(existingVehicleDetail);
                }
            }

            if (dto.Documents?.Any() == true)
            {
                foreach (var docItem in dto.Documents)
                {
                    string? filePath = null;

                    if (docItem.File?.Length > 0)
                    {
                        var document = await _documentRepository.GetByIdAsync(docItem.DocumentId);
                        var documentNameEn = document!.NameEn.ToLower().Replace(" ", "-");
                        var directoryPath = $"vehicle/{vehicle.Id}/{documentNameEn}";

                        var fileName = await _mediaService.UploadAsync(docItem.File, directoryPath);
                        filePath = fileName;

                        uploadedFilePaths.Add($"{directoryPath}/{fileName}");
                    }

                    if (docItem.Id.HasValue)
                    {
                        var existingVehicleDocument = await _vehicleDocumentRepository.GetByIdAsync(docItem.Id.Value);

                        if (filePath != null && !string.IsNullOrWhiteSpace(existingVehicleDocument!.Path))
                        {
                            try
                            {
                                await _mediaService.RemoveAsync(existingVehicleDocument.Path);
                            }
                            catch
                            {
                                // Ignore cleanup errors (or log)
                            }
                        }

                        existingVehicleDocument!.DocumentId = docItem.DocumentId;
                        if (filePath != null)
                            existingVehicleDocument.Path = filePath;

                        existingVehicleDocument.ExpiryDate = docItem.ExpiryDate;
                        existingVehicleDocument.Status = (DocumentVerificationStatus)docItem.Status;
                        _vehicleDocumentRepository.Update(existingVehicleDocument);
                    }
                    else
                    {
                        var vehicleDocument = new VehicleDocument
                        {
                            DocumentId = docItem.DocumentId,
                            VehicleId = vehicle.Id,
                            Path = filePath,
                            ExpiryDate = docItem.ExpiryDate,
                            Status = (DocumentVerificationStatus)docItem.Status,
                            AddedBy = _currentUserService.GetCurrentUserId(),
                        };

                        await _vehicleDocumentRepository.AddAsync(vehicleDocument);
                    }
                }
            }
            if (vehicleType.VehicleUnitType == VehicleUnitType.Head)
            {
                var currentTailVehicles = await _vehicleRepository.GetTailVehiclesByHeadVehicleIdAsync(dto.Id);
                long? previousTailId = currentTailVehicles?.FirstOrDefault()?.Id;
                long? previousDriverId = vehicle.DriverId;
                var tailVehicleIdsToKeep = dto.TailVehicleIds ?? new List<long>();

                if (currentTailVehicles?.Any() == true)
                {
                    var tailVehicleIdsToRemove = currentTailVehicles
                        .Where(tv => !tailVehicleIdsToKeep.Contains(tv.Id))
                        .Select(tv => tv.Id)
                        .ToList();

                    if (tailVehicleIdsToRemove.Any())
                        await _vehicleRepository.UpdateTailVehiclesHeadAssignmentAsync(tailVehicleIdsToRemove, null);
                }

                if (dto.TailVehicleIds?.Any() == true)
                    await _vehicleRepository.UpdateTailVehiclesHeadAssignmentAsync(dto.TailVehicleIds, dto.Id);

                long? newTailId = tailVehicleIdsToKeep.Count > 0 ? tailVehicleIdsToKeep[0] : null;
                await _assignmentHistoryRecorder.RecordChangeAsync(
                    dto.Id,
                    VehicleAssignmentHistorySource.UpdateVehicle,
                    previousDriverId,
                    previousTailId,
                    previousDriverId,
                    newTailId,
                    addedBy);
            }
            else if (vehicleType.VehicleUnitType == VehicleUnitType.Tail && previousHeadLinkOnTail != dto.HeadVehicleId)
            {
                if (previousHeadLinkOnTail.HasValue)
                {
                    var oldHead = await _vehicleRepository.GetByIdOrThrowAsync(previousHeadLinkOnTail.Value);
                    await _assignmentHistoryRecorder.RecordChangeAsync(
                        previousHeadLinkOnTail.Value,
                        VehicleAssignmentHistorySource.UpdateVehicle,
                        oldHead.DriverId,
                        dto.Id,
                        oldHead.DriverId,
                        null,
                        addedBy);
                }

                if (dto.HeadVehicleId.HasValue)
                {
                    var newHead = await _vehicleRepository.GetByIdOrThrowAsync(dto.HeadVehicleId.Value);
                    var currentTailsOnNewHead = await _vehicleRepository.GetTailVehiclesByHeadVehicleIdAsync(dto.HeadVehicleId.Value);
                    long? previousTailOnNewHead = currentTailsOnNewHead.FirstOrDefault(t => t.Id != dto.Id)?.Id;
                    await _assignmentHistoryRecorder.RecordChangeAsync(
                        dto.HeadVehicleId.Value,
                        VehicleAssignmentHistorySource.UpdateVehicle,
                        newHead.DriverId,
                        previousTailOnNewHead,
                        newHead.DriverId,
                        dto.Id,
                        addedBy);
                }
            }

            await _vehicleRepository.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            foreach (var filePath in uploadedFilePaths)
            {
                try
                {
                    await _mediaService.RemoveAsync(filePath);
                }
                catch
                {
                    // Log error but continue cleanup
                }
            }
            throw;
        }
    }

    private static bool PlateMatches(string currentNo, string currentLetters, string newNo, string newLetters) =>
        string.Equals(currentNo.Trim(), newNo.Trim(), StringComparison.OrdinalIgnoreCase)
        && string.Equals(currentLetters.Trim(), newLetters.Trim(), StringComparison.OrdinalIgnoreCase);

    private async Task ValidatePlateUniquenessAsync(string plateNo, string plateLetters, long currentVehicleId)
    {
        var existing = await _vehicleRepository.GetByPlateNoAndLettersAsync(plateNo, plateLetters);
        if (existing != null && existing.Id != currentVehicleId)
            throw new ValidationException("vehicles.plate_no_and_letters_already_exists", plateNo, plateLetters);
    }

    private async Task ValidateFuelTypeAsync(long? fuelTypeId)
    {
        if (!fuelTypeId.HasValue)
            return;
        _ = await _fuelTypeRepository.GetByIdAsync(fuelTypeId.Value)
            ?? throw new NotFoundException("fuel_types.not_found_with_id", fuelTypeId.Value);
    }

    private async Task ValidateReferencesAsync(UpdateVehicleDto dto)
    {
        _ = await _vehicleTypeRepository.GetByIdOrThrowAsync(dto.VehicleTypeId);

        if (dto.ColorId.HasValue)
        {
            var color = await _colorRepository.GetByIdAsync(dto.ColorId.Value) ?? throw new NotFoundException("colors.not_found_with_id", dto.ColorId.Value);
        }

        if (dto.CapacityUnitId.HasValue)
        {
            var capacityUnit = await _capacityUnitRepository.GetByIdAsync(dto.CapacityUnitId.Value) ?? throw new NotFoundException("vehicle_capacity_units.not_found_with_id", dto.CapacityUnitId.Value);
        }

        if (dto.BrandModelID.HasValue)
        {
            var brandModel = await _vehicleBrandModelRepository.GetByIdAsync(dto.BrandModelID.Value) ?? throw new NotFoundException("vehicle_brand_models.not_found_with_id", dto.BrandModelID.Value);
        }

        if (dto.InsuranceId.HasValue)
        {
            var insurance = await _vehicleInsuranceRepository.GetByIdAsync(dto.InsuranceId.Value);
            if (insurance == null)
                throw new NotFoundException("vehicle_insurances.not_found_with_id", dto.InsuranceId.Value);
        }
    }

    private async Task ValidateVehicleRelationshipsAsync(UpdateVehicleDto dto)
    {
        var vehicleType = await _vehicleTypeRepository.GetByIdOrThrowAsync(dto.VehicleTypeId);
        if (dto.HeadVehicleId.HasValue)
        {
            if (vehicleType.VehicleUnitType != VehicleUnitType.Tail)
                throw new LogicalException("vehicles.only_tail_can_have_head");
            await ValidateHeadVehicleAsync(dto.HeadVehicleId.Value, dto.Id);
        }

        if (dto.TailVehicleIds?.Any() == true)
        {
            if (vehicleType.VehicleUnitType != VehicleUnitType.Head)
                throw new LogicalException("vehicles.only_head_can_have_tail");
            await ValidateTailVehiclesAsync(dto.TailVehicleIds, dto.Id);
        }
    }

    private async Task ValidateHeadVehicleAsync(long headVehicleId, long currentVehicleId)
    {
        var headVehicle = await _vehicleRepository.GetByIdAsync(headVehicleId) ?? throw new NotFoundException("vehicles.head_vehicle_not_found_with_id", headVehicleId);
        var headVehicleType = await _vehicleTypeRepository.GetByIdAsync(headVehicle.VehicleTypeId);
        if (headVehicleType?.VehicleUnitType != VehicleUnitType.Head)
            throw new NotFoundException("vehicles.head_vehicle_is_not_head", headVehicleId);

        var currentTailVehicles = await _vehicleRepository.GetTailVehiclesByHeadVehicleIdAsync(headVehicleId);
        var currentTailCount = currentTailVehicles?.Count(tv => tv.Id != currentVehicleId) ?? 0;

        var isAlreadyAssignedToThisHead = currentTailVehicles?.Any(tv => tv.Id == currentVehicleId) ?? false;
        var totalTailCount = isAlreadyAssignedToThisHead ? currentTailCount : currentTailCount + 1;

        if (totalTailCount > 1)
            throw new ValidationException("vehicles.head_vehicle_tail_limit_when_adding", headVehicleId, currentTailCount, totalTailCount);
    }

    private async Task ValidateTailVehiclesAsync(List<long> tailVehicleIds, long currentVehicleId)
    {
        if (tailVehicleIds.Distinct().Count() > 1)
            throw new ValidationException("vehicles.head_vehicle_max_tail_exceeded", tailVehicleIds.Count);

        var tailVehicles = await _vehicleRepository.GetVehiclesByIdsAsync(tailVehicleIds);

        if (tailVehicles.Count != tailVehicleIds.Count)
        {
            var foundIds = tailVehicles.Select(v => v.Id).ToList();
            var missingIds = tailVehicleIds.Except(foundIds).ToList();
            throw new NotFoundException("vehicles.tail_vehicles_not_found_with_ids", string.Join(", ", missingIds));
        }

        foreach (var tailVehicle in tailVehicles)
        {
            if (tailVehicle.VehicleType?.VehicleUnitType != VehicleUnitType.Tail)
                throw new ValidationException("vehicles.tail_vehicle_is_not_tail", tailVehicle.Id);
            if (tailVehicle.HeadVehicleId.HasValue && tailVehicle.HeadVehicleId != currentVehicleId)
                throw new ValidationException("vehicles.tail_vehicle_already_assigned", tailVehicle.Id);
        }
    }

    private async Task ValidateContractorAsync(long? contractorId)
    {
        if (!contractorId.HasValue) return;

        _ = await _contractorRepository.GetByIdOrThrowAsync(contractorId.Value);
    }

    private async Task ValidateSectorAsync(long? sectorId)
    {
        if (!sectorId.HasValue) return;

        _ = await _sectorRepository.GetByIdOrThrowAsync(sectorId.Value);
    }
}
