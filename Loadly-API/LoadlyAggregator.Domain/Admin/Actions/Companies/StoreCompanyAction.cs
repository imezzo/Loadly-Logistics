using LoadlyAggregator.Domain.Admin.DTOs.Companies;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Companies;

public class StoreCompanyAction
{
    private readonly ICompanyRepository _companyRepository;
    private readonly ICompanyDetailRepository _companyDetailRepository;
    private readonly IAddressRepository _addressRepository;
    private readonly IContactRepository _contactRepository;
    private readonly ICompanyDocumentRepository _companyDocumentRepository;
    private readonly IGovernerateRepository _governerateRepository;
    private readonly ICityRepository _cityRepository;
    private readonly IDistrictRepository _districtRepository;
    private readonly ICommunicationMethodRepository _communicationMethodRepository;
    private readonly IDocumentRepository _documentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPermissionService _permissionService;
    private readonly IMediaService _mediaService;
    private readonly IPasswordResetTokenService _passwordResetTokenService;
    private readonly IResetPasswordUrlService _resetPasswordUrlService;
    private readonly INotificationTransport _notificationTransport;

    public StoreCompanyAction(
        ICompanyRepository companyRepository,
        ICompanyDetailRepository companyDetailRepository,
        IAddressRepository addressRepository,
        IContactRepository contactRepository,
        ICompanyDocumentRepository companyDocumentRepository,
        IGovernerateRepository governerateRepository,
        ICityRepository cityRepository,
        IDistrictRepository districtRepository,
        ICommunicationMethodRepository communicationMethodRepository,
        IDocumentRepository documentRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IPermissionService permissionService,
        IMediaService mediaService,
        IPasswordResetTokenService passwordResetTokenService,
        IResetPasswordUrlService resetPasswordUrlService,
        INotificationTransport notificationTransport)
    {
        _companyRepository = companyRepository;
        _companyDetailRepository = companyDetailRepository;
        _addressRepository = addressRepository;
        _contactRepository = contactRepository;
        _companyDocumentRepository = companyDocumentRepository;
        _governerateRepository = governerateRepository;
        _cityRepository = cityRepository;
        _districtRepository = districtRepository;
        _communicationMethodRepository = communicationMethodRepository;
        _documentRepository = documentRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _permissionService = permissionService;
        _mediaService = mediaService;
        _passwordResetTokenService = passwordResetTokenService;
        _resetPasswordUrlService = resetPasswordUrlService;
        _notificationTransport = notificationTransport;
    }

    public async Task ExecuteAsync(StoreCompanyDto dto)
    {
        // Validate addresses
        if (dto.Addresses != null && dto.Addresses.Any())
        {
            foreach (var addressData in dto.Addresses)
            {
                var governerate = await _governerateRepository.GetByIdAsync(addressData.GovernerateId);
                if (governerate == null)
                {
                    throw new NotFoundException("governerates.not_found_with_id", addressData.GovernerateId);
                }
                var city = await _cityRepository.GetByIdAsync(addressData.CityId);
                if (city == null)
                {
                    throw new NotFoundException("cities.not_found_with_id", addressData.CityId);
                }
                if (addressData.DistrictId.HasValue)
                {
                    var district = await _districtRepository.GetByIdAsync(addressData.DistrictId.Value);
                    if (district == null)
                    {
                        throw new NotFoundException("districts.not_found_with_id", addressData.DistrictId.Value);
                    }
                }
            }
        }

        // Validate contacts
        if (dto.Contacts != null && dto.Contacts.Any())
        {
            foreach (var contactData in dto.Contacts)
            {
                if (contactData.CommunicationMethodId.HasValue)
                {
                    var communicationMethod = await _communicationMethodRepository.GetByIdAsync(contactData.CommunicationMethodId.Value);
                    if (communicationMethod == null)
                    {
                        throw new NotFoundException("communication_methods.not_found_with_id", contactData.CommunicationMethodId.Value);
                    }
                }
            }
        }

        // Validate documents
        if (dto.CompanyDocuments != null && dto.CompanyDocuments.Any())
        {
            foreach (var documentData in dto.CompanyDocuments)
            {
                var document = await _documentRepository.GetByIdAsync(documentData.DocumentId);
                if (document == null)
                {
                    throw new NotFoundException("documents.not_found_with_id", documentData.DocumentId);
                }
                if (documentData.File != null && documentData.File.Length > 0)
                {
                    DocumentFileTypeValidator.Validate(documentData.File.ContentType);
                }
            }
        }

        // Validate required documents for verified companies
        if (dto.VerificationStatus == VerificationStatus.VERIFIED)
        {
            var requiredDocuments = await _documentRepository
                .FindAsync(d => d.EntityId == DocumentEntityType.COMPANY && d.Required == true && d.Status == DocumentStatus.ACTIVE);

            if (requiredDocuments != null && requiredDocuments.Any())
            {
                var providedDocumentIds = dto.CompanyDocuments?.Select(cd => cd.DocumentId).ToList() ?? new List<long>();

                foreach (var requiredDoc in requiredDocuments)
                {
                    if (!providedDocumentIds.Contains(requiredDoc.Id))
                    {
                        throw new ValidationException("companies.required_document_verified", requiredDoc.NameEn);
                    }
                    var providedDoc = dto.CompanyDocuments?.FirstOrDefault(cd => cd.DocumentId == requiredDoc.Id);
                    if (providedDoc != null && (providedDoc.File == null || providedDoc.File.Length == 0))
                    {
                        throw new ValidationException("companies.required_document_file", requiredDoc.NameEn);
                    }
                }
            }
        }

        var uploadedFilePaths = new List<string>();
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            var company = new Common.Entities.Company
            {
                FullName = dto.FullName,
                UserName = dto.UserName.ToLower(),
                Email = dto.Email.ToLower(),
                Phone = dto.Phone,
                Type = CompanyType.COMPANY,
                Status = UserStatus.ACTIVE,
                MfaChannel = (CommunicationChannel)dto.MfaChannel,
                CompanyId = null,
                Password = string.Empty,
                EmailVerified = false,
                PhoneVerified = false
            };

            await _companyRepository.AddAsync(company);
            await _companyRepository.SaveChangesAsync();

            CompanyDetail? companyDetail = null;
            if (!string.IsNullOrWhiteSpace(dto.NameEn) ||
                !string.IsNullOrWhiteSpace(dto.NameAr) ||
                !string.IsNullOrWhiteSpace(dto.RegistrationNumber) ||
                !string.IsNullOrWhiteSpace(dto.TaxId) ||
                !string.IsNullOrWhiteSpace(dto.WebsiteUrl) ||
                dto.IndustryTypeId.HasValue ||
                !string.IsNullOrWhiteSpace(dto.CompanySize) ||
                !string.IsNullOrWhiteSpace(dto.ServiceRequirementsIds) ||
                dto.BusinessHourId.HasValue ||
                dto.CommunicationsMethodId.HasValue ||
                dto.VerificationStatus != VerificationStatus.PENDING ||
                !string.IsNullOrWhiteSpace(dto.QrCode))
            {
                companyDetail = new CompanyDetail
                {
                    CompanyId = company.Id,
                    NameEn = dto.NameEn,
                    NameAr = dto.NameAr ?? string.Empty,
                    RegistrationNumber = dto.RegistrationNumber,
                    TaxId = dto.TaxId,
                    WebsiteUrl = dto.WebsiteUrl,
                    IndustryTypeId = dto.IndustryTypeId,
                    CompanySize = dto.CompanySize,
                    ServiceRequirementsIds = dto.ServiceRequirementsIds,
                    BusinessHourId = dto.BusinessHourId,
                    CommunicationsMethodId = dto.CommunicationsMethodId,
                    VerificationStatus = dto.VerificationStatus,
                    QrCode = dto.QrCode,
                    AddedBy = _currentUserService.GetCurrentUserId()
                };

                await _companyDetailRepository.AddAsync(companyDetail);
            }

            if (dto.Addresses != null && dto.Addresses.Any())
            {
                foreach (var addressData in dto.Addresses)
                {
                    var address = new Address
                    {
                        ModelType = "Company",
                        ModelId = company.Id,
                        StreetName = addressData.StreetName,
                        BuildingNumber = addressData.BuildingNumber,
                        BuildingType = addressData.BuildingType,
                        GovernerateId = addressData.GovernerateId,
                        CityId = addressData.CityId,
                        DistrictId = addressData.DistrictId,
                        PostalCode = addressData.PostalCode,
                        AddedBy = _currentUserService.GetCurrentUserId(),
                        Status = AddressStatus.ACTIVE
                    };

                    await _addressRepository.AddAsync(address);
                }
            }

            if (dto.Contacts != null && dto.Contacts.Any())
            {
                foreach (var contactData in dto.Contacts)
                {
                    var contact = new Contact
                    {
                        ModelType = "Company",
                        ModelId = company.Id,
                        Primary = contactData.Primary,
                        Name = contactData.Name,
                        Email = contactData.Email,
                        Phone = contactData.Phone,
                        Position = contactData.Position,
                        CommunicationMethodId = contactData.CommunicationMethodId,
                        AddedBy = _currentUserService.GetCurrentUserId()
                    };

                    await _contactRepository.AddAsync(contact);
                }
            }

            if (dto.CompanyDocuments != null && dto.CompanyDocuments.Any())
            {
                foreach (var documentData in dto.CompanyDocuments)
                {
                    string? filePath = null;

                    if (documentData.File != null && documentData.File.Length > 0)
                    {
                        var document = await _documentRepository.GetByIdAsync(documentData.DocumentId);
                        var documentNameEn = document!.NameEn.ToLower().Replace(" ", "-");
                        var directoryPath = $"company/{company.Id}/{documentNameEn}";
                        var fileName = await _mediaService.UploadAsync(documentData.File, directoryPath);
                        filePath = fileName;
                        uploadedFilePaths.Add($"{directoryPath}/{fileName}");
                    }

                    var companyDocument = new CompanyDocument
                    {
                        DocumentId = documentData.DocumentId,
                        CompanyId = company.Id,
                        Path = filePath,
                        ExpiryDate = documentData.ExpiryDate,
                        Status = documentData.Status,
                        AddedBy = documentData.AddedBy ?? dto.AddedBy
                    };

                    await _companyDocumentRepository.AddAsync(companyDocument);
                }
            }

            await _companyRepository.SaveChangesAsync();

            await _permissionService.GiveAllGuardPermissionsAsync(company);

            await _unitOfWork.CommitTransactionAsync();

            var resetToken = _passwordResetTokenService.GenerateResetToken(
                company.Id,
                "company",
                channel: CommunicationChannel.Email,
                expirationHours: 24
            );

            var resetUrl = _resetPasswordUrlService.GenerateResetPasswordUrl("company", resetToken);

            try
            {
                var emailBody = $@"
                        <h2>Welcome to Loadly!</h2>
                        <p>Hello {company.FullName},</p>
                        <p>Your <strong>Company</strong> account has been created successfully.</p>
                        <p><strong>Next Steps:</strong></p>
                        <p>To activate your account and set your password, please click the button below:</p>
                        <p style=""text-align: center; margin: 30px 0;"">
                            <a href=""{resetUrl}"" style=""display: inline-block; padding: 12px 30px; background-color: #007bff; color: #ffffff; text-decoration: none; border-radius: 5px; font-weight: bold;"">Set Your Password</a>
                        </p>
                        <p><strong>What happens next?</strong></p>
                        <ul>
                            <li>You'll be redirected to a secure page to set your password</li>
                            <li>Your email will be verified automatically</li>
                            <li>You can then log in to your company account</li>
                        </ul>
                        <p><strong>Important:</strong> This link will expire in 24 hours. If the link expires, please contact support.</p>
                        <p>If you didn't request this account, please contact support immediately.</p>
                    ";

                await _notificationTransport.SendEmailAsync(
                    company.Email,
                    company.FullName,
                    "Welcome to Loadly - Set Your Company Account Password",
                    emailBody,
                    isHtml: true,
                    domain: NotificationDomain.Company,
                    modelId: company.Id,
                    modelType: "Company");
            }
            catch (Exception)
            {
            }
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
                }
            }

            throw;
        }
    }
}
