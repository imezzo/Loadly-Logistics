using LoadlyAggregator.Domain.Admin.DTOs.Contractors;
using LoadlyAggregator.Domain.Admin.Services;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Contractors;

public class CreateContractorAction
{
    private readonly IContractorRepository _contractorRepository;
    private readonly IContractorDocumentRepository _contractorDocumentRepository;
    private readonly IDocumentRepository _documentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMediaService _mediaService;
    private readonly IContractorNotificationService _contractorNotificationService;
    private readonly IPasswordResetTokenService _passwordResetTokenService;
    private readonly IResetPasswordUrlService _resetPasswordUrlService;

    public CreateContractorAction(
        IContractorRepository contractorRepository,
        IContractorDocumentRepository contractorDocumentRepository,
        IDocumentRepository documentRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IMediaService mediaService,
        IContractorNotificationService contractorNotificationService,
        IPasswordResetTokenService passwordResetTokenService,
        IResetPasswordUrlService resetPasswordUrlService)
    {
        _contractorRepository = contractorRepository;
        _contractorDocumentRepository = contractorDocumentRepository;
        _documentRepository = documentRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _mediaService = mediaService;
        _contractorNotificationService = contractorNotificationService;
        _passwordResetTokenService = passwordResetTokenService;
        _resetPasswordUrlService = resetPasswordUrlService;
    }

    public async Task ExecuteAsync(CreateContractorDto dto)
    {
        var contractorDocuments = (await _documentRepository.GetByEntityIdAsync(DocumentEntityType.CONTRACTOR)).ToList();
        ContractorHelper.ValidateDocuments(contractorDocuments, dto.Documents, requireFilesForRequiredDocuments: true);

        var uploadedFilePaths = new List<string>();
        Common.Entities.Contractor? contractor = null;
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            long? addedBy = _currentUserService.GetCurrentUserId();

            contractor = new Common.Entities.Contractor
            {
                FullName = dto.FullName,
                UserName = ContractorHelper.BuildUserNameFromPhone(dto.Phone),
                Email = string.IsNullOrWhiteSpace(dto.Email) ? string.Empty : dto.Email.ToLower(),
                Phone = dto.Phone,
                NationalId = string.IsNullOrWhiteSpace(dto.NationalId) ? null : dto.NationalId,
                SecondPhoneNumber = string.IsNullOrWhiteSpace(dto.SecondPhoneNumber) ? null : dto.SecondPhoneNumber,
                Status = dto.Status,
                Password = string.Empty,
                EmailVerified = false,
                PhoneVerified = false,
                MfaChannel = dto.MfaChannel,
                ContractorStatus = dto.IsActive == true ? ContractorStatus.ACTIVE : ContractorStatus.INACTIVE,
                ApprovedAt = dto.IsActive == true ? DateTime.UtcNow : null,
                ApprovedBy = dto.IsActive == true ? addedBy : null
            };

            await _contractorRepository.AddAsync(contractor);
            await _contractorRepository.SaveChangesAsync();

            await AddDocumentsAsync(contractor, dto.Documents, addedBy, uploadedFilePaths);

            await _contractorRepository.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            await ContractorHelper.CleanupUploadedFilesAsync(_mediaService, uploadedFilePaths);
            throw;
        }

        if (contractor != null)
        {
            var resetPasswordUrl = _resetPasswordUrlService.GenerateResetPasswordUrl(
                "contractor",
                _passwordResetTokenService.GenerateResetToken(contractor.Id, "contractor", channel: CommunicationChannel.Email, expirationHours: 24));
            await _contractorNotificationService.SendContractorWelcomeEmailIfEmailChannelAsync(contractor, resetPasswordUrl);
        }
    }

    private async Task AddDocumentsAsync(Common.Entities.Contractor contractor, IReadOnlyList<ContractorDocumentItemDto>? documents, long? addedBy, List<string> uploadedFilePaths)
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
                Status = item.Status,
                ContractorId = contractor.Id,
                Path = filePath,
                AddedBy = addedBy
            });
        }

        await _contractorDocumentRepository.AddRangeAsync(contractorDocuments);
    }
}
