using LoadlyAggregator.Domain.Admin.DTOs.Contractors;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Contractor.DTOs.Auth;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Contractor.Actions.Auth;

public class RegisterContractorAction
{
    private readonly IContractorRepository _contractorRepository;
    private readonly IContractorDocumentRepository _contractorDocumentRepository;
    private readonly IDocumentRepository _documentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediaService _mediaService;

    public RegisterContractorAction(
        IContractorRepository contractorRepository,
        IContractorDocumentRepository contractorDocumentRepository,
        IDocumentRepository documentRepository,
        IUnitOfWork unitOfWork,
        IMediaService mediaService)
    {
        _contractorRepository = contractorRepository;
        _contractorDocumentRepository = contractorDocumentRepository;
        _documentRepository = documentRepository;
        _unitOfWork = unitOfWork;
        _mediaService = mediaService;
    }

    public async Task ExecuteAsync(RegisterContractorDto dto)
    {
        var contractorDocuments = (await _documentRepository.GetByEntityIdAsync(DocumentEntityType.CONTRACTOR)).ToList();
        ContractorHelper.ValidateDocuments(contractorDocuments, dto.Documents, requireFilesForRequiredDocuments: true);

        var uploadedFilePaths = new List<string>();
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var contractor = new Common.Entities.Contractor
            {
                FullName = dto.FullName,
                UserName = ContractorHelper.BuildUserNameFromPhone(dto.Phone),
                Email = string.Empty,
                Phone = dto.Phone,
                NationalId = string.IsNullOrWhiteSpace(dto.NationalId) ? null : dto.NationalId,
                Address = string.IsNullOrWhiteSpace(dto.Address) ? null : dto.Address.Trim(),
                Status = UserStatus.INACTIVE,
                Password = string.Empty,
                EmailVerified = false,
                PhoneVerified = false,
                MfaChannel = CommunicationChannel.None,
                ContractorStatus = ContractorStatus.REQUESTED
            };

            await _contractorRepository.AddAsync(contractor);
            await _contractorRepository.SaveChangesAsync();

            await AddDocumentsAsync(contractor, dto.Documents, uploadedFilePaths);

            await _contractorRepository.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            await ContractorHelper.CleanupUploadedFilesAsync(_mediaService, uploadedFilePaths);
            throw;
        }
    }

    private async Task AddDocumentsAsync(
        Common.Entities.Contractor contractor,
        IReadOnlyList<ContractorDocumentItemDto>? documents,
        List<string> uploadedFilePaths)
    {
        if (documents == null || documents.Count <= 0)
            return;

        var documentIds = documents.Select(d => d.DocumentId).ToList();
        var documentById = await _documentRepository.GetByIdsAsync(documentIds);
        var contractorDocuments = new List<ContractorDocument>(documents.Count);

        foreach (var item in documents)
        {
            if (!documentById.TryGetValue(item.DocumentId, out var document))
            {
                throw new NotFoundException("documents.not_found_with_id", item.DocumentId);
            }

            ContractorHelper.ValidateDocumentEntityType(document, item.DocumentId);

            string? filePath = null;
            if (item.File is { Length: > 0 })
            {
                DocumentFileTypeValidator.Validate(item.File.ContentType);
                var directoryPath = ContractorDocumentPathHelper.BuildDirectoryPath(contractor.Id, document.NameEn);
                var fileName = await _mediaService.UploadAsync(item.File, directoryPath);
                filePath = fileName;
                uploadedFilePaths.Add($"{directoryPath}/{fileName}");
            }

            contractorDocuments.Add(new ContractorDocument
            {
                DocumentId = item.DocumentId,
                ExpiryDate = item.ExpiryDate,
                Status = DocumentVerificationStatus.PENDING,
                ContractorId = contractor.Id,
                Path = filePath,
                AddedBy = null
            });
        }

        await _contractorDocumentRepository.AddRangeAsync(contractorDocuments);
    }
}
