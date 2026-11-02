using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Contractors;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Contractor.Actions.Auth;

public class ContractorGetProfileAction : IAction<long, object>
{
    private readonly IContractorRepository _contractorRepository;
    private readonly IPermissionService _permissionService;
    private readonly IMediaService _mediaService;
    private readonly IMapper _mapper;

    public ContractorGetProfileAction(
        IContractorRepository contractorRepository,
        IPermissionService permissionService,
        IMediaService mediaService,
        IMapper mapper)
    {
        _contractorRepository = contractorRepository;
        _permissionService = permissionService;
        _mediaService = mediaService;
        _mapper = mapper;
    }

    public async Task<object> ExecuteAsync(long userId)
    {
        var contractor = await _contractorRepository.GetByIdWithChildrenOrThrowAsync(userId);

        var documents = _mapper.Map<List<ContractorDocumentDto>>(contractor.ContractorDocuments);
        ContractorHelper.ApplyDocumentFileUrls(contractor.Id, documents, _mediaService);

        return new
        {
            Id = contractor.Id,
            FullName = contractor.FullName,
            UserName = contractor.UserName,
            Email = contractor.Email,
            Phone = contractor.Phone,
            NationalId = contractor.NationalId,
            SecondPhoneNumber = contractor.SecondPhoneNumber,
            UserType = "contractor",
            MfaChannel = (int)contractor.MfaChannel,
            EmailVerified = contractor.EmailVerified,
            PhoneVerified = contractor.PhoneVerified,
            Status = (int)contractor.Status,
            ContractorStatus = (int)contractor.ContractorStatus,
            Permissions = await _permissionService.GetPermissionsGroupedByGroupAsync(contractor),
            Documents = documents.Select(doc => new
            {
                Id = doc.Id,
                DocumentId = doc.DocumentId,
                ContractorId = doc.ContractorId,
                Path = doc.Path,
                FileUrl = doc.FileUrl,
                ExpiryDate = doc.ExpiryDate,
                Status = (int)doc.Status,
                AddedBy = doc.AddedBy,
                CreatedAt = doc.CreatedAt,
                UpdatedAt = doc.UpdatedAt,
                Document = doc.Document != null
                    ? new
                    {
                        Id = doc.Document.Id,
                        EntityId = doc.Document.EntityId,
                        NameEn = doc.Document.NameEn,
                        NameAr = doc.Document.NameAr,
                        Required = doc.Document.Required,
                        Status = doc.Document.Status
                    }
                    : null
            }).ToList(),
            CreatedAt = contractor.CreatedAt,
            UpdatedAt = contractor.UpdatedAt
        };
    }
}
