using LoadlyAggregator.Domain.Common.DTOs.Addresses;
using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.DTOs.Contacts;
using LoadlyAggregator.Domain.Common.DTOs.Relations;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Models;

namespace LoadlyAggregator.Domain.Admin.DTOs.Drivers;

public class DriverDto
{
    public long Id { get; set; }
    public string FullNameEn { get; set; } = string.Empty;
    public string FullNameAr { get; set; } = string.Empty;
    public string NationalId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public int MfaChannel { get; set; }
    public bool EmailVerified { get; set; }
    public bool PhoneVerified { get; set; }
    public int Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int? WorkingStatus { get; set; }
    public long? ContractorId { get; set; }
    public ContractorRelationDto? Contractor { get; set; }
    public DriverDetailDto? DriverDetail { get; set; }
    public List<AddressDto> Addresses { get; set; } = new();
    public List<ContactDto> Contacts { get; set; } = new();
    public List<DriverDocumentDto> Documents { get; set; } = new();
    public int CompletedTripsCount { get; set; }
    public VehicleRelationDto? Vehicle { get; set; }
}

public class CreateDriverDto
{
    public string FullNameEn { get; set; } = string.Empty;
    public string FullNameAr { get; set; } = string.Empty;
    public string NationalId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public int Status { get; set; } = 1;
    public int MfaChannel { get; set; } = (int)CommunicationChannel.None;
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public long? LicenseTypeId { get; set; }
    public DateTime? LicenseIssueDate { get; set; }
    public string? LicenseNo { get; set; }
    public DateTime? LicenseExpirationDate { get; set; }
    public string? VehicleTypeIds { get; set; }
    public int? Age { get; set; }
    public string? StreetName { get; set; }
    public string? BuildingNumber { get; set; }
    public string? BuildingType { get; set; }
    public long? GovernerateId { get; set; }
    public long? CityId { get; set; }
    public long? DistrictId { get; set; }
    public string? PostalCode { get; set; } = string.Empty;
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public DriverWorkingStatus WorkingStatus { get; set; } = DriverWorkingStatus.Available;
    public LicenseCategory LicenseCategory { get; set; } = LicenseCategory.PrivateLicense;
    public List<SpokenLanguage>? SpokenLanguages { get; set; }
    public List<CreateDriverDocumentItemDto>? Documents { get; set; }
    public List<long>? RoleIds { get; set; }
    public long? ContractorId { get; set; }
}

public class GetDriverDto
{
    public long Id { get; set; }
}

public class ListDriversDto : BasePagedRequest
{
    public int? Status { get; set; }
    public List<long>? ContractorIds { get; set; }
    public bool? HasContractor { get; set; }
    public bool? IsAvailable { get; set; }
    public LicenseCategory? LicenseCategory { get; set; }
    public List<DriverWorkingStatus>? WorkingStatuses { get; set; }
    public DateTime? TripsStartDateFrom { get; set; }
    public DateTime? TripsEndDateTo { get; set; }
    public bool? OrderByCompletedTripsDescending { get; set; }
}

public class UpdateDriverDto
{
    public long Id { get; set; }
    public string FullNameEn { get; set; } = string.Empty;
    public string FullNameAr { get; set; } = string.Empty;
    public string NationalId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public int Status { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public long? LicenseTypeId { get; set; }
    public DateTime? LicenseIssueDate { get; set; }
    public string? LicenseNo { get; set; }
    public DateTime? LicenseExpirationDate { get; set; }
    public string? VehicleTypeIds { get; set; }
    public int? Age { get; set; }
    public string? StreetName { get; set; }
    public string? BuildingNumber { get; set; }
    public string? BuildingType { get; set; }
    public long? GovernerateId { get; set; }
    public long? CityId { get; set; }
    public long? DistrictId { get; set; }
    public string? PostalCode { get; set; } = string.Empty;
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public DriverWorkingStatus WorkingStatus { get; set; } = DriverWorkingStatus.Available;
    public LicenseCategory LicenseCategory { get; set; } = LicenseCategory.PrivateLicense;
    public List<SpokenLanguage>? SpokenLanguages { get; set; }
    public List<UpdateDriverDocumentItemDto>? Documents { get; set; }
    public long? ContractorId { get; set; }
}

public class DeleteDriverDto
{
    public long Id { get; set; }
}

public class CreateDriverDocumentItemDto
{
    public long DocumentId { get; set; }
    public FileUpload? File { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int Status { get; set; } = 0;
}

public class UpdateDriverDocumentItemDto
{
    public long? Id { get; set; }
    public long DocumentId { get; set; }
    public FileUpload? File { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int Status { get; set; }
}
