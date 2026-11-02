using System.Linq;
using LoadlyAggregator.Domain.Admin.DTOs.Companies;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Companies;

public class UpdateCompanyAction
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
    private readonly IMediaService _mediaService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCompanyAction(
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
        IMediaService mediaService,
        IUnitOfWork unitOfWork)
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
        _mediaService = mediaService;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(UpdateCompanyDto dto)
    {
        var company = await _companyRepository.GetByIdOrThrowAsync(dto.Id);

        // Validate addresses
        if (dto.Addresses != null && dto.Addresses.Any())
        {
            foreach (var addressData in dto.Addresses)
            {
                if (addressData.Id.HasValue)
                {
                    var existingAddress = await _addressRepository.GetByIdAsync(addressData.Id.Value);
                    if (existingAddress == null)
                    {
                        throw new NotFoundException("addresses.not_found_with_id", addressData.Id.Value);
                    }
                    if (existingAddress.ModelId != dto.Id || existingAddress.ModelType != "Company")
                    {
                        throw new ValidationException("addresses.does_not_belong_to_company", addressData.Id.Value);
                    }
                }
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
                if (contactData.Id.HasValue)
                {
                    var existingContact = await _contactRepository.GetByIdAsync(contactData.Id.Value);
                    if (existingContact == null)
                    {
                        throw new NotFoundException("contacts.not_found_with_id", contactData.Id.Value);
                    }
                    if (existingContact.ModelId != dto.Id || existingContact.ModelType != "Company")
                    {
                        throw new ValidationException("contacts.does_not_belong_to_company", contactData.Id.Value);
                    }
                }
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
                if (documentData.Id.HasValue)
                {
                    var existingCompanyDocument = await _companyDocumentRepository.GetByIdAsync(documentData.Id.Value);
                    if (existingCompanyDocument == null)
                    {
                        throw new NotFoundException("company_documents.not_found_with_id", documentData.Id.Value);
                    }
                    if (existingCompanyDocument.CompanyId != dto.Id)
                    {
                        throw new ValidationException("company_documents.does_not_belong_to_company", documentData.Id.Value);
                    }
                }

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

        var uploadedFilePaths = new List<string>();
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            // Update company
            company.FullName = dto.FullName;
            company.UserName = dto.UserName.ToLower();
            company.Email = dto.Email.ToLower();
            company.Phone = dto.Phone;
            _companyRepository.Update(company);

            // Update or create company detail
            CompanyDetail? companyDetail = null;
            var existingCompanyDetail = await _companyDetailRepository.GetByCompanyIdAsync(company.Id);
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
                if (existingCompanyDetail == null)
                {
                    companyDetail = new CompanyDetail
                    {
                        CompanyId = company.Id,
                        NameEn = dto.NameEn,
                        NameAr = dto.NameAr,
                        RegistrationNumber = dto.RegistrationNumber ?? string.Empty,
                        TaxId = dto.TaxId,
                        WebsiteUrl = dto.WebsiteUrl,
                        IndustryTypeId = dto.IndustryTypeId,
                        CompanySize = dto.CompanySize,
                        ServiceRequirementsIds = dto.ServiceRequirementsIds,
                        BusinessHourId = dto.BusinessHourId,
                        CommunicationsMethodId = dto.CommunicationsMethodId,
                        VerificationStatus = dto.VerificationStatus,
                        QrCode = dto.QrCode
                    };
                    await _companyDetailRepository.AddAsync(companyDetail);
                }
                else
                {
                    companyDetail = existingCompanyDetail;
                    companyDetail.NameEn = dto.NameEn;
                    companyDetail.NameAr = dto.NameAr;
                    companyDetail.RegistrationNumber = dto.RegistrationNumber;
                    companyDetail.TaxId = dto.TaxId;
                    companyDetail.WebsiteUrl = dto.WebsiteUrl;
                    companyDetail.IndustryTypeId = dto.IndustryTypeId;
                    companyDetail.CompanySize = dto.CompanySize;
                    companyDetail.ServiceRequirementsIds = dto.ServiceRequirementsIds;
                    companyDetail.BusinessHourId = dto.BusinessHourId;
                    companyDetail.CommunicationsMethodId = dto.CommunicationsMethodId;
                    companyDetail.VerificationStatus = dto.VerificationStatus;
                    companyDetail.QrCode = dto.QrCode;
                    _companyDetailRepository.Update(companyDetail);
                }
            }

            // Update addresses
            if (dto.Addresses != null && dto.Addresses.Any())
            {
                foreach (var addressData in dto.Addresses)
                {
                    if (addressData.Id.HasValue)
                    {
                        var existingAddress = await _addressRepository.GetByIdAsync(addressData.Id.Value);
                        existingAddress!.StreetName = addressData.StreetName;
                        existingAddress.BuildingNumber = addressData.BuildingNumber;
                        existingAddress.BuildingType = addressData.BuildingType;
                        existingAddress.GovernerateId = addressData.GovernerateId;
                        existingAddress.CityId = addressData.CityId;
                        existingAddress.DistrictId = addressData.DistrictId;
                        existingAddress.PostalCode = addressData.PostalCode;
                        _addressRepository.Update(existingAddress);
                    }
                    else
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
                            AddedBy = addressData.AddedBy,
                            Status = AddressStatus.ACTIVE
                        };
                        await _addressRepository.AddAsync(address);
                    }
                }
            }

            // Update contacts
            if (dto.Contacts != null && dto.Contacts.Any())
            {
                foreach (var contactData in dto.Contacts)
                {
                    if (contactData.Id.HasValue)
                    {
                        var existingContact = await _contactRepository.GetByIdAsync(contactData.Id.Value);
                        existingContact!.Primary = contactData.Primary;
                        existingContact.Name = contactData.Name;
                        existingContact.Email = contactData.Email;
                        existingContact.Phone = contactData.Phone;
                        existingContact.Position = contactData.Position;
                        existingContact.CommunicationMethodId = contactData.CommunicationMethodId;
                        _contactRepository.Update(existingContact);
                    }
                    else
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
                            AddedBy = contactData.AddedBy
                        };
                        await _contactRepository.AddAsync(contact);
                    }
                }
            }

            // Update company documents
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
                    if (documentData.Id.HasValue)
                    {
                        var existingCompanyDocument = await _companyDocumentRepository.GetByIdAsync(documentData.Id.Value);

                        if (filePath != null && !string.IsNullOrWhiteSpace(existingCompanyDocument!.Path))
                        {
                            try
                            {
                                await _mediaService.RemoveAsync(existingCompanyDocument.Path);
                            }
                            catch
                            {
                                // Log error but continue
                            }
                        }
                        existingCompanyDocument!.DocumentId = documentData.DocumentId;
                        if (filePath != null)
                        {
                            existingCompanyDocument.Path = filePath;
                        }
                        existingCompanyDocument.ExpiryDate = documentData.ExpiryDate;
                        existingCompanyDocument.Status = documentData.Status;
                        _companyDocumentRepository.Update(existingCompanyDocument);
                    }
                    else
                    {
                        var companyDocument = new CompanyDocument
                        {
                            DocumentId = documentData.DocumentId,
                            CompanyId = company.Id,
                            Path = filePath,
                            ExpiryDate = documentData.ExpiryDate,
                            Status = documentData.Status,
                            AddedBy = documentData.AddedBy
                        };
                        await _companyDocumentRepository.AddAsync(companyDocument);
                    }
                }
            }

            await _companyRepository.SaveChangesAsync();

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
}
