using Microsoft.Extensions.Logging;
using LoadlyAggregator.Domain.Admin.DTOs.Contractors;
using LoadlyAggregator.Domain.Admin.Services;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Contractors;

public class UpdateContractorAction
{
    private readonly IContractorRepository _contractorRepository;
    private readonly IContractorDocumentRepository _contractorDocumentRepository;
    private readonly IDocumentRepository _documentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMediaService _mediaService;
    private readonly IContractorNotificationService _contractorNotificationService;
    private readonly ILogger<UpdateContractorAction> _logger;

    public UpdateContractorAction(
        IContractorRepository contractorRepository,
        IContractorDocumentRepository contractorDocumentRepository,
        IDocumentRepository documentRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IMediaService mediaService,
        IContractorNotificationService contractorNotificationService,
        ILogger<UpdateContractorAction> logger)
    {
        _contractorRepository = contractorRepository;
        _contractorDocumentRepository = contractorDocumentRepository;
        _documentRepository = documentRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _mediaService = mediaService;
        _contractorNotificationService = contractorNotificationService;
        _logger = logger;
    }

    public async Task ExecuteAsync(UpdateContractorDto dto)
    {
        var contractor = await _contractorRepository.GetByIdOrThrowAsync(dto.Id);
        var previousContractorStatus = contractor.ContractorStatus;

        var contractorDocuments = (await _documentRepository.GetByEntityIdAsync(DocumentEntityType.CONTRACTOR)).ToList();
        if (dto.Documents != null)
            ContractorHelper.ValidateDocuments(contractorDocuments, dto.Documents);

        var uploadedFilePaths = new List<string>();
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            long? addedBy = _currentUserService.GetCurrentUserId();

            contractor.FullName = dto.FullName;
            contractor.UserName = ContractorHelper.BuildUserNameFromPhone(dto.Phone);
            contractor.Email = string.IsNullOrWhiteSpace(dto.Email) ? string.Empty : dto.Email.ToLower();
            contractor.Phone = dto.Phone;
            contractor.NationalId = string.IsNullOrWhiteSpace(dto.NationalId) ? null : dto.NationalId;
            contractor.SecondPhoneNumber = string.IsNullOrWhiteSpace(dto.SecondPhoneNumber) ? null : dto.SecondPhoneNumber;
            contractor.MfaChannel = dto.MfaChannel;
            contractor.Status = dto.Status;
            contractor.ContractorStatus = dto.IsActive == true
                ? ContractorStatus.ACTIVE
                : contractor.ContractorStatus == ContractorStatus.REQUESTED
                    ? ContractorStatus.REQUESTED
                    : ContractorStatus.INACTIVE;
            contractor.ApprovedAt ??= contractor.ContractorStatus == ContractorStatus.ACTIVE ? DateTime.UtcNow : null;
            contractor.ApprovedBy ??= contractor.ContractorStatus == ContractorStatus.ACTIVE ? addedBy : null;

            await _contractorRepository.UpdateAsync(contractor);
            await UpsertDocumentsAsync(contractor, dto.Documents, addedBy, uploadedFilePaths);

            await _contractorRepository.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            await ContractorHelper.CleanupUploadedFilesAsync(_mediaService, uploadedFilePaths);
            throw;
        }

        if (dto.IsActive && previousContractorStatus == ContractorStatus.REQUESTED)
            await _contractorNotificationService.SendContractorApprovedSmsAsync(contractor);
    }

    private async Task UpsertDocumentsAsync(Common.Entities.Contractor contractor, IReadOnlyList<ContractorDocumentItemDto>? documents, long? addedBy, List<string> uploadedFilePaths)
    {
        documents ??= Array.Empty<ContractorDocumentItemDto>();

        foreach (var item in documents.Where(i => i.File is { Length: > 0 }))
            DocumentFileTypeValidator.Validate(item.File!.ContentType);

        var existing = (await _contractorDocumentRepository.GetByContractorIdAsync(contractor.Id)).ToList();
        var requestedIds = documents.Where(d => d.Id.HasValue).Select(d => d.Id!.Value).ToHashSet();
        var documentsToRemove = existing.Where(d => !requestedIds.Contains(d.Id)).ToList();

        if (documentsToRemove.Count > 0)
        {
            var removedDocumentById = await _documentRepository.GetByIdsAsync(documentsToRemove.Select(d => d.DocumentId));
            await RemoveExistingDocumentFilesAsync(contractor.Id, documentsToRemove, removedDocumentById);
            _contractorDocumentRepository.RemoveRange(documentsToRemove);
        }

        if (documents.Count == 0)
            return;

        var existingById = existing.Where(d => requestedIds.Contains(d.Id)).ToDictionary(x => x.Id);
        var documentById = await _documentRepository.GetByIdsAsync(documents.Select(x => x.DocumentId).Distinct());

        var documentsToUpdate = new List<ContractorDocument>();
        var documentsToAdd = new List<ContractorDocument>();

        foreach (var item in documents)
        {
            if (!documentById.TryGetValue(item.DocumentId, out var document))
            {
                throw new NotFoundException("documents.not_found_with_id", item.DocumentId);
            }

            ContractorHelper.ValidateDocumentEntityType(document, item.DocumentId);

            if (item.Id.HasValue)
            {
                if (!existingById.TryGetValue(item.Id.Value, out var existingDoc))
                    throw new NotFoundException("contractor_documents.not_found_with_id", item.Id.Value);

                existingDoc.DocumentId = item.DocumentId;
                existingDoc.ExpiryDate = item.ExpiryDate;
                existingDoc.Status = item.Status;
                existingDoc.AddedBy = addedBy;
                if (item.File is { Length: > 0 })
                {
                    await RemoveExistingDocumentFilesAsync(contractor.Id, new[] { existingDoc }, documentById);
                    existingDoc.Path = await TryUploadDocumentFileAsync(contractor.Id, item, documentById, uploadedFilePaths);
                }
                documentsToUpdate.Add(existingDoc);
            }
            else
            {
                var filePath = await TryUploadDocumentFileAsync(contractor.Id, item, documentById, uploadedFilePaths);
                documentsToAdd.Add(new ContractorDocument
                {
                    DocumentId = item.DocumentId,
                    ExpiryDate = item.ExpiryDate,
                    Status = item.Status,
                    ContractorId = contractor.Id,
                    Path = filePath,
                    AddedBy = addedBy
                });
            }
        }

        if (documentsToUpdate.Count > 0)
            await _contractorDocumentRepository.UpdateRangeAsync(documentsToUpdate);

        if (documentsToAdd.Count > 0)
            await _contractorDocumentRepository.AddRangeAsync(documentsToAdd);
    }

    private async Task RemoveExistingDocumentFilesAsync(
        long contractorId,
        IEnumerable<ContractorDocument> existingDocuments,
        IReadOnlyDictionary<long, Document> documentById)
    {
        foreach (var doc in existingDocuments.Where(d => !string.IsNullOrEmpty(d.Path)))
        {
            var resolvedPath = ResolveDocumentPath(contractorId, doc.DocumentId, doc.Path!, documentById);
            try
            {
                await _mediaService.RemoveAsync(resolvedPath);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to remove document file during contractor update: {FilePath}", resolvedPath);
            }
        }
    }

    private async Task<string?> TryUploadDocumentFileAsync(long contractorId, ContractorDocumentItemDto item, Dictionary<long, Document> documentById, List<string> uploadedFilePaths)
    {
        if (item.File is not { Length: > 0 })
            return null;

        if (!documentById.TryGetValue(item.DocumentId, out var document))
        {
            throw new NotFoundException("documents.not_found_with_id", item.DocumentId);
        }

        var directoryPath = ContractorDocumentPathHelper.BuildDirectoryPath(contractorId, document.NameEn);
        var fileName = await _mediaService.UploadAsync(item.File, directoryPath);
        uploadedFilePaths.Add($"{directoryPath}/{fileName}");
        return fileName;
    }

    private static string ResolveDocumentPath(
        long contractorId,
        long documentId,
        string storedPath,
        IReadOnlyDictionary<long, Document> documentById)
    {
        if (storedPath.Contains('/'))
            return storedPath;

        if (!documentById.TryGetValue(documentId, out var document))
            return storedPath;

        return ContractorDocumentPathHelper.BuildFilePath(contractorId, document.NameEn, storedPath);
    }
}
