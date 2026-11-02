using LoadlyAggregator.Domain.Admin.DTOs.Drivers;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Drivers;

public class CreateDriverAction
{
    private readonly IDriverRepository _driverRepository;
    private readonly IContractorRepository _contractorRepository;
    private readonly IDriverDetailRepository _driverDetailRepository;
    private readonly IAddressRepository _addressRepository;
    private readonly IDriverDocumentRepository _driverDocumentRepository;
    private readonly IDriverLicenseTypeRepository _driverLicenseTypeRepository;
    private readonly IDocumentRepository _documentRepository;
    private readonly IGovernerateRepository _governerateRepository;
    private readonly ICityRepository _cityRepository;
    private readonly IDistrictRepository _districtRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationTransport _notificationTransport;
    private readonly IMediaService _mediaService;
    private readonly IPermissionService _permissionService;

    public CreateDriverAction(
        IDriverRepository driverRepository,
        IContractorRepository contractorRepository,
        IDriverDetailRepository driverDetailRepository,
        IAddressRepository addressRepository,
        IDriverDocumentRepository driverDocumentRepository,
        IDriverLicenseTypeRepository driverLicenseTypeRepository,
        IDocumentRepository documentRepository,
        IGovernerateRepository governerateRepository,
        ICityRepository cityRepository,
        IDistrictRepository districtRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        INotificationTransport notificationTransport,
        IMediaService mediaService,
        IPermissionService permissionService)
    {
        _driverRepository = driverRepository;
        _contractorRepository = contractorRepository;
        _driverDetailRepository = driverDetailRepository;
        _addressRepository = addressRepository;
        _driverDocumentRepository = driverDocumentRepository;
        _driverLicenseTypeRepository = driverLicenseTypeRepository;
        _documentRepository = documentRepository;
        _governerateRepository = governerateRepository;
        _cityRepository = cityRepository;
        _districtRepository = districtRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _notificationTransport = notificationTransport;
        _mediaService = mediaService;
        _permissionService = permissionService;
    }

    public async Task ExecuteAsync(CreateDriverDto dto)
    {
        if (dto.ContractorId.HasValue)
        {
            await _contractorRepository.GetByIdOrThrowAsync(dto.ContractorId.Value);
        }

        if (dto.LicenseTypeId.HasValue)
        {
            var licenseType = await _driverLicenseTypeRepository.GetByIdAsync(dto.LicenseTypeId.Value);
            if (licenseType == null)
            {
                throw new NotFoundException("driver_license_types.not_found_with_id", dto.LicenseTypeId.Value);
            }
        }

        if (dto.GovernerateId.HasValue)
        {
            var governerate = await _governerateRepository.GetByIdAsync(dto.GovernerateId.Value);
            if (governerate == null)
            {
                throw new NotFoundException("governerates.not_found_with_id", dto.GovernerateId.Value);
            }
        }

        if (dto.CityId.HasValue)
        {
            var city = await _cityRepository.GetByIdAsync(dto.CityId.Value);
            if (city == null)
            {
                throw new NotFoundException("cities.not_found_with_id", dto.CityId.Value);
            }
        }

        if (dto.DistrictId.HasValue)
        {
            var district = await _districtRepository.GetByIdAsync(dto.DistrictId.Value);
            if (district == null)
            {
                throw new NotFoundException("districts.not_found_with_id", dto.DistrictId.Value);
            }
        }

        var requiredDocuments = await _documentRepository
            .FindAsync(d => d.EntityId == DocumentEntityType.DRIVER && d.Required == true && d.Status == DocumentStatus.ACTIVE);
        if (requiredDocuments != null && requiredDocuments.Any())
        {
            var providedDocumentIds = dto.Documents?.Select(cd => cd.DocumentId).ToList() ?? new List<long>();
            foreach (var requiredDoc in requiredDocuments)
            {
                if (!providedDocumentIds.Contains(requiredDoc.Id))
                {
                    throw new ValidationException("drivers.required_document", requiredDoc.NameEn);
                }
            }
        }

        try
        {
            await _unitOfWork.BeginTransactionAsync();
            long? addedBy = _currentUserService.GetCurrentUserId();

            var driver = new Common.Entities.Driver
            {
                FullName = dto.FullNameEn,
                FullNameAr = dto.FullNameAr,
                NationalId = dto.NationalId,
                UserName = dto.UserName.ToLower(),
                Email = dto.Email.ToLower(),
                Phone = dto.Phone,
                Status = (UserStatus)dto.Status,
                Password = string.Empty,
                EmailVerified = false,
                PhoneVerified = false,
                MfaChannel = (CommunicationChannel)dto.MfaChannel,
                WorkingStatus = dto.WorkingStatus,
                ContractorId = dto.ContractorId
            };

            await _driverRepository.AddAsync(driver);
            await _driverRepository.SaveChangesAsync();

            Address? address = null;
            if (!string.IsNullOrWhiteSpace(dto.StreetName))
            {
                if (!dto.GovernerateId.HasValue)
                {
                    throw new ValidationException("addresses.governerate_id_required");
                }

                if (!dto.CityId.HasValue)
                {
                    throw new ValidationException("addresses.city_id_required");
                }

                address = new Address
                {
                    ModelType = "Driver",
                    ModelId = driver.Id,
                    StreetName = dto.StreetName,
                    BuildingNumber = dto.BuildingNumber,
                    BuildingType = dto.BuildingType,
                    GovernerateId = dto.GovernerateId.Value,
                    CityId = dto.CityId.Value,
                    DistrictId = dto.DistrictId,
                    PostalCode = dto.PostalCode,
                    AddedBy = addedBy,
                    Status = AddressStatus.ACTIVE
                };

                await _addressRepository.AddAsync(address);
            }

            List<DriverDocument> documents = new();
            if (dto.Documents != null && dto.Documents.Any())
            {
                foreach (var docItem in dto.Documents)
                {
                    var document = await _documentRepository.GetByIdAsync(docItem.DocumentId);
                    if (document == null)
                    {
                        throw new NotFoundException("documents.not_found_with_id", docItem.DocumentId);
                    }

                    if (document.EntityId != DocumentEntityType.DRIVER)
                    {
                        throw new ValidationException("documents.invalid_entity_type_for_driver", docItem.DocumentId, document.EntityId);
                    }

                    string? filePath = null;
                    if (docItem.File != null && docItem.File.Length > 0)
                    {
                        DocumentFileTypeValidator.Validate(docItem.File.ContentType);

                        var documentNameEn = document.NameEn.ToLower().Replace(" ", "-");
                        var directoryPath = $"driver/{driver.Id}/{documentNameEn}";
                        var fileName = await _mediaService.UploadAsync(docItem.File, directoryPath);
                        filePath = $"{directoryPath}/{fileName}";
                    }

                    var driverDocument = new DriverDocument
                    {
                        DocumentId = docItem.DocumentId,
                        DriverId = driver.Id,
                        Path = filePath,
                        ExpiryDate = docItem.ExpiryDate,
                        Status = (DocumentVerificationStatus)docItem.Status,
                        AddedBy = addedBy
                    };

                    await _driverDocumentRepository.AddAsync(driverDocument);
                    documents.Add(driverDocument);
                }
            }

            DriverDetail? driverDetail = null;
            if (dto.DateOfBirth.HasValue ||
                !string.IsNullOrWhiteSpace(dto.Gender) ||
                dto.LicenseTypeId.HasValue ||
                dto.LicenseIssueDate.HasValue ||
                !string.IsNullOrWhiteSpace(dto.LicenseNo) ||
                dto.LicenseExpirationDate.HasValue ||
                !string.IsNullOrWhiteSpace(dto.VehicleTypeIds) ||
                dto.Age.HasValue)
            {
                driverDetail = new DriverDetail
                {
                    DriverId = driver.Id,
                    DateOfBirth = dto.DateOfBirth,
                    Gender = dto.Gender,
                    LicenseTypeId = dto.LicenseTypeId,
                    LicenseIssueDate = dto.LicenseIssueDate,
                    LicenseNo = dto.LicenseNo,
                    LicenseExpirationDate = dto.LicenseExpirationDate,
                    VehicleTypeIds = dto.VehicleTypeIds ?? string.Empty,
                    Age = dto.Age,
                    LicenseCategory = dto.LicenseCategory,
                    EmergencyContactName = dto.EmergencyContactName,
                    EmergencyContactPhone = dto.EmergencyContactPhone,
                    SpokenLanguages = dto.SpokenLanguages
                };

                await _driverDetailRepository.AddAsync(driverDetail);
            }

            await _driverRepository.SaveChangesAsync();

            if (dto.RoleIds != null && dto.RoleIds.Any())
            {
                foreach (var roleId in dto.RoleIds)
                {
                    try
                    {
                        await _permissionService.AssignRoleAsync(driver, roleId);
                    }
                    catch (InvalidOperationException ex)
                    {
                        throw new LogicalException("company_users.role_assignment_failed_with_details", roleId, ex.Message);
                    }
                }

                await _permissionService.SyncRolePermissionsToUserAsync(driver);
            }

            await _unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
