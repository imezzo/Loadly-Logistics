using LoadlyAggregator.Domain.Admin.DTOs.Drivers;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Contractor.DTOs.Drivers;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Contractor.Actions.Drivers;

public class ContractorCreateDriverAction
{
    private readonly IDriverRepository _driverRepository;
    private readonly IDriverDocumentRepository _driverDocumentRepository;
    private readonly IDocumentRepository _documentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMediaService _mediaService;

    public ContractorCreateDriverAction(
        IDriverRepository driverRepository,
        IDriverDocumentRepository driverDocumentRepository,
        IDocumentRepository documentRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IMediaService mediaService)
    {
        _driverRepository = driverRepository;
        _driverDocumentRepository = driverDocumentRepository;
        _documentRepository = documentRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _mediaService = mediaService;
    }

    public async Task<long> ExecuteAsync(ContractorCreateDriverDto dto)
    {
        var requiredDocuments = await _documentRepository.GetRequiredActiveByEntityAsync(DocumentEntityType.CONTRACTOR_DRIVER);
        var providedDocumentIds = dto.Documents?.Select(d => d.DocumentId).ToList() ?? [];
        foreach (var requiredDoc in requiredDocuments)
        {
            if (!providedDocumentIds.Contains(requiredDoc.Id))
                throw new ValidationException("drivers.required_document", requiredDoc.NameEn);

            var providedDoc = dto.Documents?.FirstOrDefault(d => d.DocumentId == requiredDoc.Id);
            if (providedDoc?.File is not { Length: > 0 })
                throw new ValidationException("drivers.required_document_file", requiredDoc.NameEn);
        }

        string userName = await ResolveUniqueUserNameAsync(dto.Phone);
        string email = await ResolveUniqueEmailAsync(dto.Phone);

        var uploadedFilePaths = new List<string>();
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            long? addedBy = _currentUserService.GetCurrentUserId();

            var driver = new Common.Entities.Driver
            {
                FullNameAr = dto.FullNameAr.Trim(),
                UserName = userName,
                Email = email,
                Phone = dto.Phone.Trim(),
                Status = UserStatus.ACTIVE,
                Password = string.Empty,
                EmailVerified = false,
                PhoneVerified = false,
                MfaChannel = CommunicationChannel.None,
                WorkingStatus = DriverWorkingStatus.Available,
                ContractorId = dto.ContractorId
            };

            await _driverRepository.AddAsync(driver);
            await _driverRepository.SaveChangesAsync();

            await AddDocumentsAsync(driver, dto.Documents, addedBy, uploadedFilePaths);

            await _driverRepository.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
            return driver.Id;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            await ContractorHelper.CleanupUploadedFilesAsync(_mediaService, uploadedFilePaths);
            throw;
        }
    }

    private async Task AddDocumentsAsync(
        Common.Entities.Driver driver,
        IReadOnlyList<CreateDriverDocumentItemDto>? documents,
        long? addedBy,
        List<string> uploadedFilePaths)
    {
        if (documents is not { Count: > 0 })
            return;

        var documentIds = documents.Select(d => d.DocumentId).ToList();
        var documentById = await _documentRepository.GetByIdsAsync(documentIds);
        var driverDocuments = new List<DriverDocument>(documents.Count);

        foreach (var item in documents)
        {
            if (!documentById.TryGetValue(item.DocumentId, out var document))
                throw new NotFoundException("documents.not_found_with_id", item.DocumentId);

            if (document.EntityId != DocumentEntityType.CONTRACTOR_DRIVER)
                throw new ValidationException("documents.invalid_entity_type_for_driver", item.DocumentId, document.EntityId);

            string? filePath = null;
            if (item.File is { Length: > 0 })
            {
                DocumentFileTypeValidator.Validate(item.File.ContentType);
                var documentNameEn = document.NameEn.ToLower().Replace(" ", "-");
                var directoryPath = $"driver/{driver.Id}/{documentNameEn}";
                var fileName = await _mediaService.UploadAsync(item.File, directoryPath);
                filePath = fileName;
                uploadedFilePaths.Add($"{directoryPath}/{fileName}");
            }

            driverDocuments.Add(new DriverDocument
            {
                DocumentId = item.DocumentId,
                DriverId = driver.Id,
                Path = filePath,
                ExpiryDate = item.ExpiryDate,
                Status = DocumentVerificationStatus.PENDING,
                AddedBy = addedBy
            });
        }

        await _driverDocumentRepository.AddRangeAsync(driverDocuments);
    }

    private async Task<string> ResolveUniqueUserNameAsync(string phone)
    {
        var baseName = new string(phone.Where(char.IsDigit).ToArray());
        if (string.IsNullOrWhiteSpace(baseName))
            baseName = $"driver{DateTime.UtcNow.Ticks}";

        var candidate = baseName.ToLowerInvariant();
        var suffix = 0;
        while (await _driverRepository.ExistsByUserNameAsync(candidate))
        {
            suffix++;
            candidate = $"{baseName}{suffix}".ToLowerInvariant();
        }

        return candidate;
    }

    private async Task<string> ResolveUniqueEmailAsync(string phone)
    {
        var digits = new string(phone.Where(char.IsDigit).ToArray());
        if (string.IsNullOrWhiteSpace(digits))
            digits = DateTime.UtcNow.Ticks.ToString();

        var candidate = $"{digits}@contractor-driver.local";
        var suffix = 0;
        while (await _driverRepository.ExistsByEmailAsync(candidate))
        {
            suffix++;
            candidate = $"{digits}{suffix}@contractor-driver.local";
        }

        return candidate.ToLowerInvariant();
    }
}
