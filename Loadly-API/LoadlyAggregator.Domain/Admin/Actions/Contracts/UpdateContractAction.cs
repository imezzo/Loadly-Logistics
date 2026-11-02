using AutoMapper;
using Microsoft.Extensions.Logging;
using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Admin.Helpers;
using LoadlyAggregator.Domain.Admin.Services;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Contracts;

public class UpdateContractAction
{
    private readonly IContractRepository _contractRepository;
    private readonly IContractApprovalPipeline _approvalPipeline;
    private readonly IContractApprovalRepository _approvalRepository;
    private readonly IContractStatusHistoryRepository _historyRepository;
    private readonly IContractVehicleTypeRepository _contractVehicleTypeRepository;
    private readonly IContractSlotRepository _contractSlotRepository;
    private readonly IContractDocumentRepository _contractDocumentRepository;
    private readonly IContractExtraFeeRepository _contractExtraFeeRepository;
    private readonly IContractDestinationRepository _contractDestinationRepository;
    private readonly IWaypointRepository _waypointRepository;
    private readonly IVehicleTypeRepository _vehicleTypeRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly ITripRepository _tripRepository;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IDocumentRepository _documentRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IAdminRepository _adminRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMediaService _mediaService;
    private readonly IContractStateMachine _stateMachine;
    private readonly IPermissionService _permissionService;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateContractAction> _logger;

    public UpdateContractAction(
        IContractRepository contractRepository,
        IContractApprovalPipeline approvalPipeline,
        IContractApprovalRepository approvalRepository,
        IContractStatusHistoryRepository historyRepository,
        IContractVehicleTypeRepository contractVehicleTypeRepository,
        IContractSlotRepository contractSlotRepository,
        IContractDocumentRepository contractDocumentRepository,
        IContractExtraFeeRepository contractExtraFeeRepository,
        IContractDestinationRepository contractDestinationRepository,
        IWaypointRepository waypointRepository,
        IVehicleTypeRepository vehicleTypeRepository,
        IVehicleRepository vehicleRepository,
        ITripRepository tripRepository,
        IInvoiceRepository invoiceRepository,
        IDocumentRepository documentRepository,
        ICompanyRepository companyRepository,
        IAdminRepository adminRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IMediaService mediaService,
        IContractStateMachine stateMachine,
        IPermissionService permissionService,
        IMapper mapper,
        ILogger<UpdateContractAction> logger)
    {
        _contractRepository = contractRepository;
        _approvalPipeline = approvalPipeline;
        _approvalRepository = approvalRepository;
        _historyRepository = historyRepository;
        _contractVehicleTypeRepository = contractVehicleTypeRepository;
        _contractSlotRepository = contractSlotRepository;
        _contractDocumentRepository = contractDocumentRepository;
        _contractExtraFeeRepository = contractExtraFeeRepository;
        _contractDestinationRepository = contractDestinationRepository;
        _waypointRepository = waypointRepository;
        _vehicleTypeRepository = vehicleTypeRepository;
        _vehicleRepository = vehicleRepository;
        _tripRepository = tripRepository;
        _invoiceRepository = invoiceRepository;
        _documentRepository = documentRepository;
        _companyRepository = companyRepository;
        _adminRepository = adminRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _mediaService = mediaService;
        _stateMachine = stateMachine;
        _permissionService = permissionService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task ExecuteAsync(UpdateContractDto dto)
    {
        var contract = await _contractRepository.GetByIdOrThrowAsync(dto.Id);
        await ValidateUpdateContractPermissionByStatusAsync(contract.Status);
        EnsureCanUpdateOrSubmit(true, dto.SubmitForApproval);
        await ValidateEditableReferencesAsync(dto);

        var uploadedFilePaths = new List<string>();
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var addedBy = _currentUserService.GetCurrentUserId();
            await ApplyCommonUpdatesAsync(contract, dto, true);
            await ApplyEditablePartsAsync(contract, dto, addedBy, uploadedFilePaths);

            _contractRepository.Update(contract);
            await _contractRepository.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            await CleanupUploadedFilesAsync(uploadedFilePaths);
            throw;
        }
    }

    private static void EnsureCanUpdateOrSubmit(bool isEditable, bool submitForApproval)
    {
        if (isEditable || !submitForApproval)
            return;
        throw new ValidationException("contracts.only_draft_can_be_submitted");
    }

    private async Task ValidateUpdateContractPermissionByStatusAsync(ContractStatus status)
    {
        var currentUserId = _currentUserService.GetCurrentUserId()
            ?? throw new UnauthorizedException("auth.user_not_authenticated");
        var admin = await _adminRepository.GetByIdOrThrowAsync(currentUserId);

        string? requiredPermission = status switch
        {
            ContractStatus.ACTIVE => "edit_contract:active",
            _ => null
        };

        if (requiredPermission == null)
            return;

        if (!await _permissionService.HasPermissionToAsync(admin, requiredPermission))
            throw new ForbiddenException("contracts.not_authorized");
    }

    /// <summary>
    /// Updates applied to every contract regardless of status. When ACTIVE, only these fields (Price, MonthlyWorkingDays, PaymentMethod, BillingCycle, PaymentDueDays, vehicle type fields including TripsPerMonth/TripsPerDay/MaxKms) are updated.
    /// </summary>
    private async Task ApplyCommonUpdatesAsync(Contract contract, UpdateContractDto dto, bool isEditable)
    {
        contract.Price = dto.Price;
        contract.CargoCommission = dto.CargoCommission ?? contract.CargoCommission;
        contract.MonthlyWorkingDays = dto.MonthlyWorkingDays ?? contract.MonthlyWorkingDays;
        contract.PaymentMethod = dto.PaymentMethod;
        contract.BillingCycle = dto.BillingCycle;
        contract.PaymentDueDays = dto.PaymentDueDays;
        if (!isEditable)
        {
            var existingVehicleTypes = (await _contractVehicleTypeRepository.GetByContractIdAsync(contract.Id)).ToDictionary(x => x.VehicleTypeId);
            foreach (var item in dto.VehicleTypes!)
            {
                if (!existingVehicleTypes.TryGetValue(item.VehicleTypeId, out var existingEntity))
                    continue;

                existingEntity.ConsumptionRate = item.ConsumptionRate;
                existingEntity.TripsPerMonth = item.TripsPerMonth;
                var monthlyWorkingDays = ContractCalculationHelper.ResolveMonthlyWorkingDays(contract.MonthlyWorkingDays, contract.ContractType);
                existingEntity.TripsPerDay = ContractCalculationHelper.CalculateTripsPerDay(item.TripsPerMonth, monthlyWorkingDays);
                existingEntity.MaxKms = item.MaxKms;
                existingEntity.MonthlyRental = item.MonthlyRental;
                existingEntity.DailyValue = ContractCalculationHelper.CalculateDailyValue(item.MonthlyRental, monthlyWorkingDays, contract.ContractType);
                existingEntity.OverConsumption = item.OverConsumption;
                existingEntity.OverWeightPricePerTon = item.OverWeightPricePerTon;
                existingEntity.ContractorOverWeightPricePerTon = item.ContractorOverWeightPricePerTon;
                existingEntity.ExtraWorkingDayFees = item.ExtraWorkingDayFees;
                _contractVehicleTypeRepository.Update(existingEntity);
            }
        }
    }

    private async Task ApplyEditablePartsAsync(Contract contract, UpdateContractDto dto, long? addedBy, List<string> uploadedFilePaths)
    {
        _mapper.Map(dto, contract);
        // Explicitly switch the company relation to ensure FK persists correctly.
        contract.CompanyId = dto.CompanyId;
        contract.KmOverageMode = ContractCalculationHelper.NormalizeKmOverageModeForStorage(
            contract.ContractType,
            contract.KmOverageMode);
        await ReplaceVehicleTypesAsync(
            contract,
            dto.VehicleTypes,
            ContractCalculationHelper.ResolveMonthlyWorkingDays(contract.MonthlyWorkingDays, contract.ContractType),
            addedBy);
        await ReplaceDocumentsAsync(contract, dto.Documents, addedBy, uploadedFilePaths);
        dto.ExtraFees = ContractFuelExtraFeeHelper.AddFuelToContractExtraFees(dto.ExtraFees, dto.ContractType, dto.ConsumptionType);
        await ReplaceExtraFeesAsync(contract, dto.ExtraFees, addedBy);
        await ReplaceDestinationsAsync(contract, dto.Destinations, addedBy);
        await ReplaceSlotsAsync(contract, dto.Slots, addedBy);

        if (dto.SubmitForApproval)
            await SubmitForApprovalAsync(contract, addedBy);
    }

    private async Task CleanupUploadedFilesAsync(List<string> uploadedFilePaths)
    {
        foreach (var filePath in uploadedFilePaths)
        {
            try
            {
                await _mediaService.RemoveAsync(filePath);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to remove uploaded file during rollback cleanup: {FilePath}", filePath);
            }
        }
    }

    private async Task ValidateEditableReferencesAsync(UpdateContractDto dto)
    {
        _ = await _companyRepository.GetByIdOrThrowAsync(dto.CompanyId);

        if (dto.SalesRepId.HasValue)
            _ = await _adminRepository.GetByIdOrThrowAsync(dto.SalesRepId.Value);

        await ContractReferenceValidator.EnsureVehicleTypesExistAsync(dto.VehicleTypes, _vehicleTypeRepository);
        await ContractReferenceValidator.EnsureSlotReferencesExistAsync(dto.Slots, _vehicleTypeRepository, _vehicleRepository);
    }

    private async Task SubmitForApprovalAsync(Contract contract, long? addedBy)
    {
        _stateMachine.ValidateTransition(contract.Status, ContractStatus.PENDING_APPROVAL);

        var stages = _approvalPipeline.GetStages();
        var firstStage = stages[0].Stage;

        if (await _approvalRepository.HasApprovalsForContractAsync(contract.Id))
        {
            var existingApprovals = await _approvalRepository.GetApprovalsByContractIdAsync(contract.Id);
            foreach (var approval in existingApprovals)
            {
                approval.Status = ApprovalStatus.PENDING;
                approval.ApproverId = null;
                approval.ActionDate = null;
                approval.Comments = null;
                _approvalRepository.Update(approval);
            }
        }
        else
        {
            foreach (var approval in ContractApprovalHelper.CreateDefaultApprovals(contract.Id, stages))
                await _approvalRepository.AddAsync(approval);
        }

        contract.Status = ContractStatus.PENDING_APPROVAL;
        contract.CurrentApprovalStage = firstStage;
        await _historyRepository.AddHistoryAsync(
            contract.Id,
            ContractStatus.DRAFT,
            ContractStatus.PENDING_APPROVAL,
            addedBy,
            additionalNotes: null,
            firstStage);
    }


    private async Task ReplaceVehicleTypesAsync(Contract contract, IReadOnlyList<ContractVehicleTypeDto>? vehicleTypes, int? monthlyWorkingDays, long? addedBy)
    {
        var items = vehicleTypes ?? Array.Empty<ContractVehicleTypeDto>();
        var existing = await _contractVehicleTypeRepository.GetByContractIdAsync(contract.Id);
        var existingByVehicleTypeId = existing.ToDictionary(x => x.VehicleTypeId);
        var dtoVehicleTypeIds = items.Select(x => x.VehicleTypeId).ToHashSet();

        foreach (var e in existing.Where(e => !dtoVehicleTypeIds.Contains(e.VehicleTypeId)))
            _contractVehicleTypeRepository.Remove(e);

        foreach (var item in items)
        {
            if (existingByVehicleTypeId.TryGetValue(item.VehicleTypeId, out var existingEntity))
            {
                _mapper.Map(item, existingEntity);
                existingEntity.MonthlyRental = item.MonthlyRental;
                existingEntity.TripsPerDay = ContractCalculationHelper.CalculateTripsPerDay(item.TripsPerMonth, monthlyWorkingDays);
                existingEntity.DailyValue = ContractCalculationHelper.CalculateDailyValue(item.MonthlyRental, monthlyWorkingDays, contract.ContractType);
                existingEntity.AddedBy = addedBy;
                _contractVehicleTypeRepository.Update(existingEntity);
            }
            else
            {
                var contractVehicleType = _mapper.Map<ContractVehicleType>(item);
                contractVehicleType.ContractId = contract.Id;
                contractVehicleType.MonthlyRental = item.MonthlyRental;
                contractVehicleType.TripsPerDay = ContractCalculationHelper.CalculateTripsPerDay(item.TripsPerMonth, monthlyWorkingDays);
                contractVehicleType.DailyValue = ContractCalculationHelper.CalculateDailyValue(item.MonthlyRental, monthlyWorkingDays, contract.ContractType);
                contractVehicleType.AddedBy = addedBy;
                await _contractVehicleTypeRepository.AddAsync(contractVehicleType);
            }
        }
    }

    private async Task ReplaceDocumentsAsync(Contract contract, IReadOnlyList<DocumentItemDto>? documents, long? addedBy, List<string> uploadedFilePaths)
    {
        var items = documents ?? Array.Empty<DocumentItemDto>();
        var documentById = await LoadDocumentsByIdAsync(items);
        foreach (var item in items.Where(i => i.File is { Length: > 0 }))
            DocumentFileTypeValidator.Validate(item.File!.ContentType);

        var existing = (await _contractDocumentRepository.GetByContractIdAsync(contract.Id)).ToList();
        var existingByDocumentId = existing.ToDictionary(x => x.DocumentId);
        var dtoDocumentIds = items.Select(x => x.DocumentId).ToHashSet();
        var orphans = existing.Where(e => !dtoDocumentIds.Contains(e.DocumentId)).ToList();

        await RemoveExistingDocumentFilesAsync(contract.Id, orphans);
        foreach (var doc in orphans)
            _contractDocumentRepository.Remove(doc);

        foreach (var item in items)
        {
            if (existingByDocumentId.TryGetValue(item.DocumentId, out var existingDoc))
            {
                _mapper.Map(item, existingDoc);
                existingDoc.AddedBy = addedBy;
                if (item.File is { Length: > 0 })
                {
                    await RemoveExistingDocumentFilesAsync(contract.Id, new[] { existingDoc });
                    var newPath = await TryUploadDocumentFileAsync(contract.Id, item, documentById, uploadedFilePaths);
                    existingDoc.Path = newPath;
                }
                _contractDocumentRepository.Update(existingDoc);
            }
            else
            {
                var filePath = await TryUploadDocumentFileAsync(contract.Id, item, documentById, uploadedFilePaths);
                var contractDocument = _mapper.Map<ContractDocument>(item);
                contractDocument.ContractId = contract.Id;
                contractDocument.Path = filePath;
                contractDocument.AddedBy = addedBy;
                await _contractDocumentRepository.AddAsync(contractDocument);
            }
        }
    }

    private async Task<Dictionary<long, Document>> LoadDocumentsByIdAsync(IReadOnlyList<DocumentItemDto> items)
    {
        var documentById = new Dictionary<long, Document>();
        foreach (var item in items)
        {
            var document = await _documentRepository.GetByIdOrThrowAsync(item.DocumentId);
            documentById[item.DocumentId] = document;
        }
        return documentById;
    }

    private async Task RemoveExistingDocumentFilesAsync(long contractId, IEnumerable<ContractDocument> existingDocuments)
    {
        foreach (var doc in existingDocuments.Where(d => !string.IsNullOrEmpty(d.Path)))
        {
            var resolvedPath = await ResolveDocumentPathAsync(contractId, doc.DocumentId, doc.Path!);
            try
            {
                await _mediaService.RemoveAsync(resolvedPath);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to remove document file during contract update: {FilePath}", resolvedPath);
            }
        }
    }

    private async Task<string?> TryUploadDocumentFileAsync(long contractId, DocumentItemDto item, Dictionary<long, Document> documentById, List<string> uploadedFilePaths)
    {
        if (item.File is not { Length: > 0 })
            return null;

        var document = documentById[item.DocumentId];
        var directoryPath = ContractDocumentPathHelper.BuildDirectoryPath(contractId, document.NameEn);
        var fileName = await _mediaService.UploadAsync(item.File, directoryPath);
        uploadedFilePaths.Add($"{directoryPath}/{fileName}");
        return fileName;
    }

    private async Task ReplaceExtraFeesAsync(Contract contract, IReadOnlyList<ExtraFeeItemDto>? extraFees, long? addedBy)
    {
        var items = extraFees ?? Array.Empty<ExtraFeeItemDto>();
        var existing = await _contractExtraFeeRepository.GetByContractIdAsync(contract.Id);
        var existingByExtraFeeTypeId = existing.ToDictionary(x => x.ExtraFeeTypeId);
        var dtoExtraFeeTypeIds = items.Select(x => (int)x.ExtraFeeTypeId).ToHashSet();

        foreach (var e in existing.Where(e => !dtoExtraFeeTypeIds.Contains(e.ExtraFeeTypeId)))
            _contractExtraFeeRepository.Remove(e);

        foreach (var item in items)
        {
            var extraFeeTypeId = (int)item.ExtraFeeTypeId;
            if (!Enum.IsDefined(typeof(ExtraFeeTypes), extraFeeTypeId))
                throw new NotFoundException("extra_fees.not_found_with_id", item.ExtraFeeTypeId);
            ExtraFeeCalculationValidator.Validate(item.ExtraFeeTypeId, item.CalculationOption, item.Value, item.Threshold);

            if (existingByExtraFeeTypeId.TryGetValue(extraFeeTypeId, out var existingEntity))
            {
                _mapper.Map(item, existingEntity);
                existingEntity.AddedBy = addedBy;
                _contractExtraFeeRepository.Update(existingEntity);
            }
            else
            {
                var contractExtraFee = _mapper.Map<ContractExtraFee>(item);
                contractExtraFee.ContractId = contract.Id;
                contractExtraFee.AddedBy = addedBy;
                await _contractExtraFeeRepository.AddAsync(contractExtraFee);
            }
        }
    }

    private async Task ReplaceDestinationsAsync(Contract contract, IReadOnlyList<ContractDestinationItemDto>? destinations, long? addedBy)
    {
        var items = destinations ?? Array.Empty<ContractDestinationItemDto>();
        var resolved = new List<(ContractDestinationItemDto Item, long StartId, long EndId)>(items.Count);
        foreach (var item in items)
        {
            long startId = await ContractDestinationWaypointResolution.ResolveWaypointIdAsync(
                item.Start,
                _waypointRepository,
                addedBy);
            long endId = await ContractDestinationWaypointResolution.ResolveWaypointIdAsync(
                item.End,
                _waypointRepository,
                addedBy
            );
            resolved.Add((item, startId, endId));
        }

        var existing = await _contractDestinationRepository.GetByContractIdAsync(contract.Id);
        var existingByPair = existing.ToDictionary(x => (x.StartWaypointId, x.EndWaypointId));
        var dtoPairs = resolved.Select(x => (x.StartId, x.EndId)).ToHashSet();

        foreach (var entity in existing.Where(entity =>
                     !dtoPairs.Contains((entity.StartWaypointId, entity.EndWaypointId))))
        {
            if (entity.Status == ContractDestinationStatus.ARCHIVED)
                continue;

            entity.Status = ContractDestinationStatus.ARCHIVED;
            _contractDestinationRepository.Update(entity);
        }

        var seen = new HashSet<(long Start, long End)>();
        foreach (var (item, startWaypointId, endWaypointId) in resolved)
        {
            if (startWaypointId == endWaypointId)
                throw new ValidationException("contracts.contract_route_start_end_must_differ");

            if (!seen.Add((startWaypointId, endWaypointId)))
                throw new LogicalException("contracts.contract_route_duplicate_in_request");

            if (existingByPair.TryGetValue((startWaypointId, endWaypointId), out var existingEntity))
            {
                existingEntity.NameAr = item.NameAr;
                existingEntity.NameEn = item.NameEn;
                existingEntity.ExpectedKms = item.ExpectedKms;
                existingEntity.ReturnExpectedKms = item.ReturnExpectedKms;
                existingEntity.NumberOfDays = item.NumberOfDays;
                existingEntity.PricePerTon = item.PricePerTon;
                existingEntity.FixedTripPrice = item.FixedTripPrice;
                existingEntity.ContractorPricePerTon = item.ContractorPricePerTon;
                existingEntity.ContractorFixedTripPrice = item.ContractorFixedTripPrice;
                existingEntity.Description = item.Description;
                existingEntity.Status = ContractDestinationStatus.ACTIVE;
                existingEntity.AddedBy = addedBy;
                _contractDestinationRepository.Update(existingEntity);
                continue;
            }

            var contractDestination = new ContractDestination
            {
                ContractId = contract.Id,
                StartWaypointId = startWaypointId,
                EndWaypointId = endWaypointId,
                NameAr = item.NameAr,
                NameEn = item.NameEn,
                ExpectedKms = item.ExpectedKms,
                ReturnExpectedKms = item.ReturnExpectedKms,
                NumberOfDays = item.NumberOfDays,
                PricePerTon = item.PricePerTon,
                FixedTripPrice = item.FixedTripPrice,
                ContractorPricePerTon = item.ContractorPricePerTon,
                ContractorFixedTripPrice = item.ContractorFixedTripPrice,
                Description = item.Description,
                Status = ContractDestinationStatus.ACTIVE,
                AddedBy = addedBy
            };
            await _contractDestinationRepository.AddAsync(contractDestination);
        }
    }

    private async Task ReplaceSlotsAsync(Contract contract, IReadOnlyList<ContractSlotItemDto>? slots, long? addedBy)
    {
        var items = slots ?? Array.Empty<ContractSlotItemDto>();
        ContractSlotPersistenceHelper.EnsureUniqueVehicleAssignments(items);

        var existing = await _contractSlotRepository.GetByContractIdAsync(contract.Id);
        var existingById = existing.ToDictionary(x => x.Id);
        var dtoIds = items.Where(x => x.Id is > 0).Select(x => x.Id!.Value).ToHashSet();
        var archivedSlotIds = existing
            .Where(entity => !dtoIds.Contains(entity.Id) && entity.Status != ContractSlotStatus.ARCHIVED)
            .Select(entity => entity.Id)
            .ToHashSet();

        foreach (var entity in existing.Where(entity => !dtoIds.Contains(entity.Id)))
        {
            if (entity.Status == ContractSlotStatus.ARCHIVED)
                continue;

            await ContractSlotPersistenceHelper.ReleaseVehicleFromContractAsync(
                contract.Id, entity.VehicleId, _vehicleRepository);
            ContractSlotPersistenceHelper.MarkArchived(entity);
            _contractSlotRepository.Update(entity);
        }

        // Release all detached vehicles before assign — same request can swap vehicles across slots.
        foreach (var item in items.Where(x => x.Id is > 0))
        {
            if (!existingById.TryGetValue(item.Id!.Value, out var existingEntity))
                throw new NotFoundException("contract_slots.not_found_with_id", item.Id!.Value);

            if (ContractSlotPersistenceHelper.ShouldReleaseVehicle(existingEntity.VehicleId, item))
            {
                await ContractSlotPersistenceHelper.ReleaseVehicleFromContractAsync(
                    contract.Id, existingEntity.VehicleId, _vehicleRepository);
            }
        }

        foreach (var item in items.Where(x => x.Id is > 0))
        {
            var id = item.Id!.Value;
            if (!existingById.TryGetValue(id, out var existingEntity))
                throw new NotFoundException("contract_slots.not_found_with_id", id);

            await ContractSlotPersistenceHelper.UpdateAsync(
                existingEntity,
                item,
                contract.Id,
                addedBy,
                _tripRepository,
                _invoiceRepository);
            _contractSlotRepository.Update(existingEntity);
        }

        foreach (var item in items)
        {
            await ContractSlotPersistenceHelper.AssignVehicleToContractAsync(
                contract.Id, item, _vehicleRepository, _contractSlotRepository, items, archivedSlotIds);

            if (item.Id is > 0)
                continue;

            ContractSlotPersistenceHelper.ValidateReplacedSlotReference(item, contract.Id, existingById, items);
            await _contractSlotRepository.AddAsync(
                ContractSlotPersistenceHelper.Create(item, contract.Id, addedBy, existingById, items));
        }
    }

    private async Task<string> ResolveDocumentPathAsync(long contractId, long documentId, string storedPath)
    {
        if (storedPath.Contains('/'))
            return storedPath;

        var document = await _documentRepository.GetByIdAsync(documentId);
        if (document == null)
            return storedPath;
        return ContractDocumentPathHelper.BuildFilePath(contractId, document.NameEn, storedPath);
    }
}
