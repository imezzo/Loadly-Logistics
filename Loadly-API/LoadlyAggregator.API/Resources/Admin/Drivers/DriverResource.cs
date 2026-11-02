using LoadlyAggregator.API.Resources.Admin;
using LoadlyAggregator.API.Resources.RelationResources;
using LoadlyAggregator.Domain.Admin.DTOs.Drivers;

namespace LoadlyAggregator.API.Resources.Admin.Drivers;

/// <summary>
/// Resource for Driver API responses (both listing and single get)
/// </summary>
public class DriverResource
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
    public int? WorkingStatus { get; set; }
    public long? ContractorId { get; set; }
    public ContractorRelationResource? Contractor { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DriverDetailRelationResource? DriverDetail { get; set; }
    public List<AddressResource> Addresses { get; set; } = new();
    public List<ContactResource> Contacts { get; set; } = new();
    public List<DriverDocumentRelationResource> Documents { get; set; } = new();
    public int CompletedTripsCount { get; set; }
    public VehicleRelationResource? Vehicle { get; set; }

    public static DriverResource FromDto(DriverDto dto) =>
        new DriverResource
        {
            Id = dto.Id,
            FullNameEn = dto.FullNameEn,
            FullNameAr = dto.FullNameAr,
            NationalId = dto.NationalId,
            UserName = dto.UserName,
            Email = dto.Email,
            Phone = dto.Phone,
            MfaChannel = dto.MfaChannel,
            EmailVerified = dto.EmailVerified,
            PhoneVerified = dto.PhoneVerified,
            Status = dto.Status,
            WorkingStatus = dto.WorkingStatus,
            ContractorId = dto.ContractorId,
            Contractor = dto.Contractor != null
                ? new ContractorRelationResource
                {
                    Id = dto.Contractor.Id,
                    FullName = dto.Contractor.FullName,
                    UserName = dto.Contractor.UserName,
                    Email = dto.Contractor.Email,
                    Phone = dto.Contractor.Phone
                }
                : null,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            DriverDetail = dto.DriverDetail != null ? new DriverDetailRelationResource
            {
                Id = dto.DriverDetail.Id,
                DriverId = dto.DriverDetail.DriverId,
                DateOfBirth = dto.DriverDetail.DateOfBirth,
                Gender = dto.DriverDetail.Gender,
                LicenseTypeId = dto.DriverDetail.LicenseTypeId,
                LicenseIssueDate = dto.DriverDetail.LicenseIssueDate,
                LicenseNo = dto.DriverDetail.LicenseNo,
                LicenseExpirationDate = dto.DriverDetail.LicenseExpirationDate,
                VehicleTypeIds = dto.DriverDetail.VehicleTypeIds,
                Age = dto.DriverDetail.Age,
                LicenseCategory = dto.DriverDetail.LicenseCategory,
                SpokenLanguages = dto.DriverDetail.SpokenLanguages,
                EmergencyContactName = dto.DriverDetail.EmergencyContactName,
                EmergencyContactPhone = dto.DriverDetail.EmergencyContactPhone,
                CreatedAt = dto.DriverDetail.CreatedAt,
                UpdatedAt = dto.DriverDetail.UpdatedAt
            } : null,
            Addresses = dto.Addresses.Select(a => AddressResource.FromDto(a)).ToList(),
            Contacts = dto.Contacts.Select(c => ContactResource.FromDto(c)).ToList(),
            Documents = dto.Documents.Select(doc => new DriverDocumentRelationResource
            {
                Id = doc.Id,
                DocumentId = doc.DocumentId,
                DriverId = doc.DriverId,
                Path = doc.Path,
                FileUrl = doc.FileUrl,
                ExpiryDate = doc.ExpiryDate,
                Status = (int)doc.Status,
                AddedBy = doc.AddedBy,
                CreatedAt = doc.CreatedAt,
                UpdatedAt = doc.UpdatedAt,
                Document = doc.Document != null ? new DocumentRelationResource
                {
                    Id = doc.Document.Id,
                    EntityId = doc.Document.EntityId,
                    NameEn = doc.Document.NameEn,
                    NameAr = doc.Document.NameAr,
                    Required = doc.Document.Required,
                    Status = doc.Document.Status
                } : null
            }).ToList(),
            CompletedTripsCount = dto.CompletedTripsCount,
            Vehicle = VehicleRelationResource.FromDto(dto.Vehicle)
        };
}

public class DriverDetailRelationResource
{
    public long Id { get; set; }
    public long DriverId { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public long? LicenseTypeId { get; set; }
    public DateTime? LicenseIssueDate { get; set; }
    public string? LicenseNo { get; set; }
    public DateTime? LicenseExpirationDate { get; set; }
    public string? VehicleTypeIds { get; set; }
    public int? Age { get; set; }
    public int? LicenseCategory { get; set; }
    public List<int>? SpokenLanguages { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class DriverDocumentRelationResource
{
    public long Id { get; set; }
    public long DocumentId { get; set; }
    public long DriverId { get; set; }
    public string? Path { get; set; }
    public string? FileUrl { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int Status { get; set; }
    public long? AddedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DocumentRelationResource? Document { get; set; }
}
