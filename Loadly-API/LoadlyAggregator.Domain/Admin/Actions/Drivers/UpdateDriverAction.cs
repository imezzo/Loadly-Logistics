using System.Linq;
using LoadlyAggregator.Domain.Admin.DTOs.Drivers;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Drivers;

public class UpdateDriverAction
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
    private readonly IMediaService _mediaService;

    public UpdateDriverAction(
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
        IMediaService mediaService)
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
        _mediaService = mediaService;
    }

    public async Task ExecuteAsync(UpdateDriverDto dto)
    {
        var driver = await _driverRepository.GetByIdOrThrowAsync(dto.Id);

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
        }

        var (existingDocumentsList, _) = await _driverDocumentRepository.ListAsync(driverId: driver.Id);
        var existingDocuments = existingDocumentsList.ToList();

        if (dto.Documents != null && dto.Documents.Any())
        {
            foreach (var docItem in dto.Documents)
            {
                if (docItem.Id.HasValue)
                {
                    var existingDoc = existingDocuments.FirstOrDefault(d => d.Id == docItem.Id.Value);
                    if (existingDoc == null)
                    {
                        throw new NotFoundException("driver_documents.not_found_with_id", docItem.Id.Value);
                    }
                    if (existingDoc.DriverId != dto.Id)
                    {
                        throw new ValidationException("drivers.document_does_not_belong", docItem.Id.Value);
                    }
                }

                var document = await _documentRepository.GetByIdAsync(docItem.DocumentId);
                if (document == null)
                {
                    throw new NotFoundException("documents.not_found_with_id", docItem.DocumentId);
                }

                if (document.EntityId != DocumentEntityType.DRIVER)
                {
                    throw new ValidationException("documents.invalid_entity_type_for_driver", docItem.DocumentId, document.EntityId);
                }

                if (docItem.File != null && docItem.File.Length > 0)
                {
                    DocumentFileTypeValidator.Validate(docItem.File.ContentType);
                }
            }
        }

        var uploadedFilePaths = new List<string>();
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var driverDetail = await _driverDetailRepository.GetByDriverIdAsync(driver.Id);
            var addresses = await _addressRepository.GetByModelAsync("Driver", driver.Id);
            var address = addresses.FirstOrDefault();

            long? addedBy = _currentUserService.GetCurrentUserId();

            driver.FullName = dto.FullNameEn;
            driver.FullNameAr = dto.FullNameAr;
            driver.NationalId = dto.NationalId;
            driver.UserName = dto.UserName.ToLower();
            driver.Email = dto.Email.ToLower();
            driver.Phone = dto.Phone;
            driver.Status = (UserStatus)dto.Status;
            driver.WorkingStatus = dto.WorkingStatus;
            driver.ContractorId = dto.ContractorId;
            await _driverRepository.UpdateAsync(driver);

            if (driverDetail == null)
            {
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
                        SpokenLanguages = dto.SpokenLanguages,
                    };

                    await _driverDetailRepository.AddAsync(driverDetail);
                }
            }
            else
            {
                driverDetail.DateOfBirth = dto.DateOfBirth;
                driverDetail.Gender = dto.Gender;
                driverDetail.LicenseTypeId = dto.LicenseTypeId;
                driverDetail.LicenseIssueDate = dto.LicenseIssueDate;
                driverDetail.LicenseNo = dto.LicenseNo;
                driverDetail.LicenseExpirationDate = dto.LicenseExpirationDate;
                driverDetail.VehicleTypeIds = dto.VehicleTypeIds ?? string.Empty;
                driverDetail.Age = dto.Age;
                driverDetail.LicenseCategory = dto.LicenseCategory;
                driverDetail.EmergencyContactName = dto.EmergencyContactName;
                driverDetail.EmergencyContactPhone = dto.EmergencyContactPhone;
                driverDetail.SpokenLanguages = dto.SpokenLanguages;
                await _driverDetailRepository.UpdateAsync(driverDetail);
            }

            if (!string.IsNullOrWhiteSpace(dto.StreetName))
            {
                if (address == null)
                {
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
                else
                {
                    address.StreetName = dto.StreetName;
                    address.BuildingNumber = dto.BuildingNumber;
                    address.BuildingType = dto.BuildingType;
                    address.GovernerateId = dto.GovernerateId.Value;
                    address.CityId = dto.CityId.Value;
                    address.DistrictId = dto.DistrictId;
                    address.PostalCode = dto.PostalCode;
                    await _addressRepository.UpdateAsync(address);
                }
            }

            if (dto.Documents != null && dto.Documents.Any())
            {
                foreach (var docItem in dto.Documents)
                {
                    string? filePath = null;

                    if (docItem.File != null && docItem.File.Length > 0)
                    {
                        var document = await _documentRepository.GetByIdAsync(docItem.DocumentId);
                        var documentNameEn = document!.NameEn.ToLower().Replace(" ", "-");
                        var directoryPath = $"driver/{driver.Id}/{documentNameEn}";
                        var fileName = await _mediaService.UploadAsync(docItem.File, directoryPath);
                        filePath = fileName;
                        uploadedFilePaths.Add($"{directoryPath}/{fileName}");
                    }

                    if (docItem.Id.HasValue)
                    {
                        var existingDoc = existingDocuments.FirstOrDefault(d => d.Id == docItem.Id.Value);

                        if (filePath != null && !string.IsNullOrWhiteSpace(existingDoc!.Path))
                        {
                            try
                            {
                                await _mediaService.RemoveAsync(existingDoc.Path);
                            }
                            catch
                            {
                                // Log but don't throw - file cleanup failure shouldn't prevent update
                            }
                        }

                        existingDoc!.DocumentId = docItem.DocumentId;
                        if (filePath != null)
                        {
                            existingDoc.Path = filePath;
                        }
                        existingDoc.ExpiryDate = docItem.ExpiryDate;
                        existingDoc.Status = (DocumentVerificationStatus)docItem.Status;
                        await _driverDocumentRepository.UpdateAsync(existingDoc);
                    }
                    else
                    {
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
                    }
                }

                var requestedIds = dto.Documents
                    .Where(d => d.Id.HasValue)
                    .Select(d => d.Id!.Value)
                    .ToList();

                var documentsToDelete = existingDocuments
                    .Where(d => !requestedIds.Contains(d.Id))
                    .ToList();

                foreach (var docToDelete in documentsToDelete)
                {
                    _driverDocumentRepository.Remove(docToDelete);
                }
            }

            await _driverRepository.SaveChangesAsync();

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
                    // Log but don't throw - file cleanup failure shouldn't prevent rollback
                }
            }

            throw;
        }
    }
}
