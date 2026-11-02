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

public class CreateContractAction
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
    private readonly IDocumentRepository _documentRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IAdminRepository _adminRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMediaService _mediaService;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateContractAction> _logger;

    public CreateContractAction(
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
        IDocumentRepository documentRepository,
        ICompanyRepository companyRepository,
        IAdminRepository adminRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IMediaService mediaService,
        IMapper mapper,
        ILogger<CreateContractAction> logger)
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
        _documentRepository = documentRepository;
        _companyRepository = companyRepository;
        _adminRepository = adminRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _mediaService = mediaService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task ExecuteAsync(CreateContractDto dto)
    {
        _ = await _companyRepository.GetByIdOrThrowAsync(dto.CompanyId);

        Contract? existingContractToRenew = dto.RenewedFromContractId is { } id
            ? await _contractRepository.GetByIdOrThrowAsync(id)
            : null;

        if (existingContractToRenew is not null)
        {
            if (existingContractToRenew.Status != ContractStatus.ACTIVE && existingContractToRenew.Status != ContractStatus.EXPIRED)
                throw new ValidationException("contracts.contract_not_eligible_for_renewal");
            if (existingContractToRenew.CompanyId != dto.CompanyId || existingContractToRenew.ContractType != dto.ContractType)
                throw new ValidationException("contracts.contract_not_eligible_for_renewal");
        }

        await ValidateSalesRepIfProvidedAsync(dto.SalesRepId);

        var uploadedFilePaths = new List<string>();
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            long? addedBy = _currentUserService.GetCurrentUserId();
            var year = dto.StartDate.Year;
            var maxSequence = await _contractRepository.GetMaxReferenceSequenceForYearAsync(year);
            var sequenceNumber = (maxSequence ?? 0) + 1;
            var referenceNumber = $"CNT-{year}-{sequenceNumber:D6}";
            var monthlyWorkingDays = ContractCalculationHelper.ResolveMonthlyWorkingDays(dto.MonthlyWorkingDays, dto.ContractType);

            var contract = _mapper.Map<Contract>(dto);
            contract.ReferenceNumber = referenceNumber;
            contract.ReferenceSequence = sequenceNumber;
            contract.Status = dto.SubmitForApproval ? ContractStatus.PENDING_APPROVAL : ContractStatus.DRAFT;
            contract.AddedBy = addedBy;
            contract.KmOverageMode = ContractCalculationHelper.NormalizeKmOverageModeForStorage(
                contract.ContractType,
                contract.KmOverageMode);

            await _contractRepository.AddAsync(contract);
            await _contractRepository.SaveChangesAsync();

            await ContractReferenceValidator.EnsureVehicleTypesExistAsync(dto.VehicleTypes, _vehicleTypeRepository);
            await ContractReferenceValidator.EnsureSlotReferencesExistAsync(dto.Slots, _vehicleTypeRepository, _vehicleRepository);
            await AddVehicleTypesAsync(contract, dto.VehicleTypes, monthlyWorkingDays, addedBy);
            await AddSlotsAsync(contract, dto.Slots, addedBy);
            await AddDocumentsAsync(contract, dto.Documents, addedBy, uploadedFilePaths);
            dto.ExtraFees = ContractFuelExtraFeeHelper.AddFuelToContractExtraFees(dto.ExtraFees, dto.ContractType, dto.ConsumptionType);
            await AddExtraFeesAsync(contract, dto.ExtraFees, addedBy);
            await AddDestinationsAsync(contract, dto.Destinations, addedBy);

            if (dto.SubmitForApproval)
                await SubmitContractForApprovalAsync(contract, addedBy);

            await _contractRepository.SaveChangesAsync();

            if (existingContractToRenew != null)
            {
                var previousStatus = existingContractToRenew.Status;
                existingContractToRenew.Status = ContractStatus.RENEWED;
                await _historyRepository.AddHistoryAsync(
                    existingContractToRenew.Id,
                    previousStatus,
                    ContractStatus.RENEWED,
                    addedBy,
                    additionalNotes: null);
                await _contractRepository.SaveChangesAsync();
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
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to remove uploaded file during rollback cleanup: {FilePath}", filePath);
                }
            }
            throw;
        }
    }

    private async Task ValidateSalesRepIfProvidedAsync(long? salesRepId)
    {
        if (!salesRepId.HasValue)
            return;
        _ = await _adminRepository.GetByIdOrThrowAsync(salesRepId.Value);
    }

    private async Task SubmitContractForApprovalAsync(Contract contract, long? addedBy)
    {
        var stages = _approvalPipeline.GetStages();
        foreach (var approval in ContractApprovalHelper.CreateDefaultApprovals(contract.Id, stages))
            await _approvalRepository.AddAsync(approval);
        contract.Status = ContractStatus.PENDING_APPROVAL;
        contract.CurrentApprovalStage = stages[0].Stage;
        await _historyRepository.AddHistoryAsync(
            contract.Id,
            ContractStatus.DRAFT,
            ContractStatus.PENDING_APPROVAL,
            addedBy,
            additionalNotes: null,
            stages[0].Stage);
    }

    private async Task AddVehicleTypesAsync(Contract contract, IReadOnlyList<CreateContractVehicleTypeDto>? vehicleTypes, int? monthlyWorkingDays, long? addedBy)
    {
        if (vehicleTypes is not { Count: > 0 }) return;
        foreach (var item in vehicleTypes)
        {
            if (await _contractVehicleTypeRepository.GetByContractIdAndVehicleTypeIdAsync(contract.Id, item.VehicleTypeId) is not null)
                throw new LogicalException("contracts.vehicle_type_already_exists");
            var contractVehicleType = _mapper.Map<ContractVehicleType>(item);
            contractVehicleType.ContractId = contract.Id;
            contractVehicleType.MonthlyRental = item.MonthlyRental;
            contractVehicleType.TripsPerDay = ContractCalculationHelper.CalculateTripsPerDay(item.TripsPerMonth, monthlyWorkingDays);
            contractVehicleType.DailyValue = ContractCalculationHelper.CalculateDailyValue(item.MonthlyRental, monthlyWorkingDays, contract.ContractType);
            contractVehicleType.AddedBy = addedBy;
            await _contractVehicleTypeRepository.AddAsync(contractVehicleType);
        }
    }

    private async Task AddSlotsAsync(Contract contract, IReadOnlyList<ContractSlotItemDto>? slots, long? addedBy)
    {
        if (slots is not { Count: > 0 }) return;

        ContractSlotPersistenceHelper.EnsureUniqueVehicleAssignments(slots);
        foreach (var item in slots)
        {
            await ContractSlotPersistenceHelper.AssignVehicleToContractAsync(
                contract.Id, item, _vehicleRepository, _contractSlotRepository, slots);
            await _contractSlotRepository.AddAsync(
                ContractSlotPersistenceHelper.Create(item, contract.Id, addedBy, new Dictionary<long, ContractSlot>(), slots));
        }
    }

    private async Task AddDocumentsAsync(Contract contract, IReadOnlyList<DocumentItemDto>? documents, long? addedBy, List<string> uploadedFilePaths)
    {
        if (documents is not { Count: > 0 }) return;

        var documentIds = documents.Select(d => d.DocumentId).ToList();

        if (documentIds.Any(id => id <= 0))
            throw new ValidationException("Each document item must have a valid DocumentId.");

        var duplicateIds = documentIds.GroupBy(id => id).Where(g => g.Count() > 1).Select(g => g.Key).ToList();

        if (duplicateIds.Count > 0)
            throw new ValidationException("contracts.duplicate_document_types", string.Join(", ", duplicateIds));

        foreach (var item in documents)
        {
            var document = await _documentRepository.GetByIdOrThrowAsync(item.DocumentId);
            string? filePath = null;
            if (item.File is { Length: > 0 })
            {
                DocumentFileTypeValidator.Validate(item.File.ContentType);
                var directoryPath = ContractDocumentPathHelper.BuildDirectoryPath(contract.Id, document.NameEn);
                var fileName = await _mediaService.UploadAsync(item.File, directoryPath);
                filePath = fileName;
                uploadedFilePaths.Add($"{directoryPath}/{fileName}");
            }

            var contractDocument = new ContractDocument
            {
                ContractId = contract.Id,
                DocumentId = item.DocumentId,
                Path = filePath,
                ExpiryDate = item.ExpiryDate,
                Status = item.Status,
                AddedBy = addedBy
            };
            await _contractDocumentRepository.AddAsync(contractDocument);
        }
    }

    private async Task AddExtraFeesAsync(Contract contract, IReadOnlyList<ExtraFeeItemDto>? extraFees, long? addedBy)
    {
        if (extraFees is not { Count: > 0 }) return;
        foreach (var item in extraFees)
        {
            var extraFeeTypeId = (int)item.ExtraFeeTypeId;
            if (!Enum.IsDefined(typeof(ExtraFeeTypes), extraFeeTypeId))
                throw new NotFoundException("extra_fees.not_found_with_id", item.ExtraFeeTypeId);
            if (await _contractExtraFeeRepository.GetByContractIdAndExtraFeeTypeIdAsync(contract.Id, extraFeeTypeId) is not null)
                throw new LogicalException("contracts.extra_fee_already_added");
            ExtraFeeCalculationValidator.Validate(item.ExtraFeeTypeId, item.CalculationOption, item.Value, item.Threshold);
            var contractExtraFee = _mapper.Map<ContractExtraFee>(item);
            contractExtraFee.ContractId = contract.Id;
            contractExtraFee.AddedBy = addedBy;
            await _contractExtraFeeRepository.AddAsync(contractExtraFee);
        }
    }

    private async Task AddDestinationsAsync(Contract contract, IReadOnlyList<ContractDestinationItemDto>? destinations, long? addedBy)
    {
        if (destinations is not { Count: > 0 })
            return;

        var seen = new HashSet<(long Start, long End)>();
        foreach (var item in destinations)
        {
            long startWaypointId = await ContractDestinationWaypointResolution.ResolveWaypointIdAsync(
                item.Start,
                _waypointRepository,
                addedBy);
            long endWaypointId = await ContractDestinationWaypointResolution.ResolveWaypointIdAsync(
                item.End,
                _waypointRepository,
                addedBy);

            if (startWaypointId == endWaypointId)
                throw new ValidationException("contracts.contract_route_start_end_must_differ");

            if (!seen.Add((startWaypointId, endWaypointId)))
                throw new LogicalException("contracts.contract_route_duplicate_in_request");

            if (await _contractDestinationRepository.GetByContractIdAndWaypointPairAsync(
                    contract.Id, startWaypointId, endWaypointId) is not null)
                throw new LogicalException("contracts.contract_route_already_added");

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
}
