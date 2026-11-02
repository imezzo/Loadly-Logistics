using LoadlyAggregator.Domain.Common.DTOs.Addresses;
using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.DTOs.Contacts;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Models;

namespace LoadlyAggregator.Domain.Admin.DTOs.Companies;

public class CompanyDto
{
    public long Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public int MfaChannel { get; set; }
    public bool EmailVerified { get; set; }
    public bool PhoneVerified { get; set; }
    public int Status { get; set; }
    public int Type { get; set; }
    public long? CompanyId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public CompanyDetailDto? CompanyDetail { get; set; }
    public List<AddressDto> Addresses { get; set; } = new();
    public List<ContactDto> Contacts { get; set; } = new();
    public List<CompanyDocumentDto> CompanyDocuments { get; set; } = new();
}

public class StoreCompanyDto
{
    public string FullName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public int MfaChannel { get; set; } = (int)CommunicationChannel.None;
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public string? RegistrationNumber { get; set; }
    public string? TaxId { get; set; }
    public string? WebsiteUrl { get; set; }
    public long? IndustryTypeId { get; set; }
    public string? CompanySize { get; set; }
    public string? ServiceRequirementsIds { get; set; }
    public long? BusinessHourId { get; set; }
    public long? CommunicationsMethodId { get; set; }
    public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.PENDING;
    public string? QrCode { get; set; }
    public long? AddedBy { get; set; }
    public List<AddressDataDto> Addresses { get; set; } = new List<AddressDataDto>();
    public List<ContactDataDto> Contacts { get; set; } = new List<ContactDataDto>();
    public List<CompanyDocumentDataDto> CompanyDocuments { get; set; } = new List<CompanyDocumentDataDto>();
}

public class GetCompanyDto
{
    public long Id { get; set; }
}

public class ListCompaniesDto : BasePagedRequest
{
    public int? Status { get; set; }
}

public class UpdateCompanyDto
{
    public long Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public string? RegistrationNumber { get; set; }
    public string? TaxId { get; set; }
    public string? WebsiteUrl { get; set; }
    public long? IndustryTypeId { get; set; }
    public string? CompanySize { get; set; }
    public string? ServiceRequirementsIds { get; set; }
    public long? BusinessHourId { get; set; }
    public long? CommunicationsMethodId { get; set; }
    public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.PENDING;
    public string? QrCode { get; set; }
    public List<AddressDataDto>? Addresses { get; set; }
    public List<ContactDataDto>? Contacts { get; set; }
    public List<CompanyDocumentDataDto>? CompanyDocuments { get; set; }
}

public class DeleteCompanyDto
{
    public long Id { get; set; }
}

public class AddressDataDto
{
    public long? Id { get; set; }
    public string StreetName { get; set; } = string.Empty;
    public string? BuildingNumber { get; set; }
    public string? BuildingType { get; set; }
    public long GovernerateId { get; set; }
    public long CityId { get; set; }
    public long? DistrictId { get; set; }
    public string PostalCode { get; set; } = string.Empty;
    public long? AddedBy { get; set; }
}

public class ContactDataDto
{
    public long? Id { get; set; }
    public bool Primary { get; set; } = false;
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Position { get; set; }
    public long? CommunicationMethodId { get; set; }
    public long? AddedBy { get; set; }
}

public class CompanyDocumentDataDto
{
    public long? Id { get; set; }
    public long DocumentId { get; set; }
    public required FileUpload File { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DocumentVerificationStatus Status { get; set; } = DocumentVerificationStatus.PENDING;
    public long? AddedBy { get; set; }
}
