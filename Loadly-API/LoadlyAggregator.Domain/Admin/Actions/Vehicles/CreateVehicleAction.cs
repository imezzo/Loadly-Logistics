using AutoMapper.Configuration.Annotations;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Vehicles;

public class CreateVehicleAction
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

    public CreateVehicleAction(
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
        VehicleAssignmentHistoryRecorder assignmentHistoryRecorder)
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
    }

    public async Task ExecuteAsync(CreateVehicleDto dto)
    {
        await ValidatePlateUniquenessAsync(dto.PlateNo, dto.PlateLetters);
        var vehicleType = await ValidateAndGetVehicleTypeAsync(dto);
        await ValidateReferencesAsync(dto);
        await ValidateFuelTypeAsync(dto.FuelTypeId);
        await ValidateVehicleRelationshipsAsync(dto, vehicleType);
        await ValidateContractorAsync(dto.ContractorId);
        await ValidateSectorAsync(dto.SectorId);

        if (dto.Documents?.Any() == true)
        {
            foreach (var docItem in dto.Documents)
            {
                var document = await _documentRepository.GetByIdAsync(docItem.DocumentId)
                    ?? throw new NotFoundException("documents.not_found_with_id", docItem.DocumentId);

                if (document.EntityId != DocumentEntityType.VEHICLE)
                    throw new ValidationException("documents.invalid_entity_type_for_vehicle", docItem.DocumentId, document.EntityId);

                if (docItem.File?.Length > 0)
                    DocumentFileTypeValidator.Validate(docItem.File.ContentType);
            }
        }

        var uploadedFilePaths = new List<string>();
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            var addedBy = _currentUserService.GetCurrentUserId();

            // Create vehicle
            var vehicle = new Vehicle
            {
                PlateNo = dto.PlateNo,
                PlateLetters = dto.PlateLetters,
                MakeYear = dto.MakeYear,
                InsuranceId = dto.InsuranceId,
                PurchaseDate = dto.PurchaseDate,
                InitialOdometerReading = dto.InitialOdometerReading,
                VinNo = dto.VinNo,
                ColorId = dto.ColorId,
                CapacityUnitId = dto.CapacityUnitId,
                ContractorId = dto.ContractorId,
                SectorId = dto.SectorId,
                VehicleTypeId = dto.VehicleTypeId,
                BrandModelID = dto.BrandModelID,
                HeadVehicleId = dto.HeadVehicleId,
                TankCapacity = dto.TankCapacity,
                Capacity = dto.Capacity,
                FuelTypeId = dto.FuelTypeId,
                VehicleTypeSettings = dto.VehicleTypeSettings,
                AddedBy = addedBy,
                Status = dto.Status.HasValue ? (VehicleStatus)dto.Status.Value : VehicleStatus.Available
            };

            await _vehicleRepository.AddAsync(vehicle);
            await _vehicleRepository.SaveChangesAsync();

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

                    var vehicleDocument = new VehicleDocument
                    {
                        DocumentId = docItem.DocumentId,
                        VehicleId = vehicle.Id,
                        Path = filePath,
                        ExpiryDate = docItem.ExpiryDate,
                        Status = (DocumentVerificationStatus)docItem.Status,
                        AddedBy = addedBy
                    };

                    await _vehicleDocumentRepository.AddAsync(vehicleDocument);
                }
            }

            await _vehicleRepository.SaveChangesAsync();

            if (dto.TailVehicleIds?.Any() == true)
            {
                await _vehicleRepository.UpdateTailVehiclesHeadAssignmentAsync(dto.TailVehicleIds, vehicle.Id);
                await _vehicleRepository.SaveChangesAsync();

                if (vehicleType.VehicleUnitType == VehicleUnitType.Head)
                {
                    long? newTailId = dto.TailVehicleIds[0];
                    await _assignmentHistoryRecorder.RecordChangeAsync(
                        vehicle.Id,
                        VehicleAssignmentHistorySource.CreateVehicle,
                        null,
                        null,
                        null,
                        newTailId,
                        addedBy);
                    await _vehicleRepository.SaveChangesAsync();
                }
            }

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
                catch { }
            }
            throw;
        }
    }

    private async Task ValidatePlateUniquenessAsync(string plateNo, string plateLetters)
    {
        var existing = await _vehicleRepository.GetByPlateNoAndLettersAsync(plateNo, plateLetters);
        if (existing != null)
            throw new ValidationException("vehicles.plate_no_and_letters_already_exists", plateNo, plateLetters);
    }

    private async Task<VehicleType> ValidateAndGetVehicleTypeAsync(CreateVehicleDto dto)
    {
        if (dto.ColorId.HasValue)
        {
            _ = await _colorRepository.GetByIdAsync(dto.ColorId.Value) ?? throw new NotFoundException("colors.not_found_with_id", dto.ColorId.Value);
        }

        if (dto.CapacityUnitId.HasValue)
        {
            _ = await _capacityUnitRepository.GetByIdAsync(dto.CapacityUnitId.Value) ?? throw new NotFoundException("vehicle_capacity_units.not_found_with_id", dto.CapacityUnitId.Value);
        }

        var vehicleType = await _vehicleTypeRepository.GetByIdAsync(dto.VehicleTypeId) ?? throw new NotFoundException("vehicle_types.not_found_with_id", dto.VehicleTypeId);
        return vehicleType;
    }

    private async Task ValidateFuelTypeAsync(long? fuelTypeId)
    {
        if (!fuelTypeId.HasValue)
            return;
        _ = await _fuelTypeRepository.GetByIdAsync(fuelTypeId.Value)
            ?? throw new NotFoundException("fuel_types.not_found_with_id", fuelTypeId.Value);
    }

    private async Task ValidateReferencesAsync(CreateVehicleDto dto)
    {
        if (dto.BrandModelID.HasValue)
        {
            var brandModel = await _vehicleBrandModelRepository.GetByIdAsync(dto.BrandModelID.Value) ?? throw new NotFoundException("vehicle_brand_models.not_found_with_id", dto.BrandModelID.Value);
        }

        if (dto.InsuranceId.HasValue)
        {
            var insurance = await _vehicleInsuranceRepository.GetByIdAsync(dto.InsuranceId.Value) ?? throw new NotFoundException("vehicle_insurances.not_found_with_id", dto.InsuranceId.Value);
        }
    }

    private async Task ValidateVehicleRelationshipsAsync(CreateVehicleDto dto, VehicleType vehicleType)
    {
        if (dto.HeadVehicleId.HasValue)
        {
            if (vehicleType.VehicleUnitType != VehicleUnitType.Tail)
                throw new LogicalException("vehicles.only_tail_can_have_head");

            await ValidateHeadVehicleAsync(dto.HeadVehicleId.Value);
        }

        if (dto.TailVehicleIds?.Any() == true)
        {
            if (vehicleType.VehicleUnitType != VehicleUnitType.Head)
                throw new LogicalException("vehicles.only_head_can_have_tail");

            await ValidateTailVehiclesAsync(dto.TailVehicleIds);
        }
    }

    private async Task ValidateHeadVehicleAsync(long headVehicleId)
    {
        var headVehicle = await _vehicleRepository.GetByIdAsync(headVehicleId) ?? throw new NotFoundException("vehicles.head_vehicle_not_found_with_id", headVehicleId);
        var headVehicleType = await _vehicleTypeRepository.GetByIdAsync(headVehicle.VehicleTypeId);
        if (headVehicleType?.VehicleUnitType != VehicleUnitType.Head)
            throw new NotFoundException("vehicles.head_vehicle_is_not_head", headVehicleId);

        var currentTailVehicles = await _vehicleRepository.GetTailVehiclesByHeadVehicleIdAsync(headVehicleId);
        var currentTailCount = currentTailVehicles?.Count ?? 0;

        if (currentTailCount >= 1)
            throw new ValidationException("vehicles.head_vehicle_tail_limit_reached", headVehicleId, currentTailCount);
    }

    private async Task ValidateTailVehiclesAsync(List<long> tailVehicleIds)
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
            if (tailVehicle.HeadVehicleId.HasValue)
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
