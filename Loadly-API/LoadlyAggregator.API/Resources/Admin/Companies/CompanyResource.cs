using LoadlyAggregator.API.Resources.Admin;
using LoadlyAggregator.API.Resources.RelationResources;
using LoadlyAggregator.Domain.Admin.DTOs.Companies;

namespace LoadlyAggregator.API.Resources.Admin.Companies;

/// <summary>
/// Resource for Company API responses (both listing and single get)
/// </summary>
public class CompanyResource
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
    public CompanyDetailRelationResource? CompanyDetail { get; set; }
    public List<AddressResource> Addresses { get; set; } = new();
    public List<ContactResource> Contacts { get; set; } = new();
    public List<CompanyDocumentRelationResource> CompanyDocuments { get; set; } = new();

    public static CompanyResource FromDto(CompanyDto dto) =>
        new CompanyResource
        {
            Id = dto.Id,
            FullName = dto.FullName,
            UserName = dto.UserName,
            Email = dto.Email,
            Phone = dto.Phone,
            MfaChannel = dto.MfaChannel,
            EmailVerified = dto.EmailVerified,
            PhoneVerified = dto.PhoneVerified,
            Status = dto.Status,
            Type = dto.Type,
            CompanyId = dto.CompanyId,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            CompanyDetail = dto.CompanyDetail != null ? new CompanyDetailRelationResource
            {
                Id = dto.CompanyDetail.Id,
                CompanyId = dto.CompanyDetail.CompanyId,
                NameEn = dto.CompanyDetail.NameEn,
                NameAr = dto.CompanyDetail.NameAr,
                RegistrationNumber = dto.CompanyDetail.RegistrationNumber,
                TaxId = dto.CompanyDetail.TaxId,
                WebsiteUrl = dto.CompanyDetail.WebsiteUrl,
                IndustryTypeId = dto.CompanyDetail.IndustryTypeId,
                CompanySize = dto.CompanyDetail.CompanySize,
                ServiceRequirementsIds = dto.CompanyDetail.ServiceRequirementsIds,
                BusinessHourId = dto.CompanyDetail.BusinessHourId,
                CommunicationsMethodId = dto.CompanyDetail.CommunicationsMethodId,
                VerificationStatus = dto.CompanyDetail.VerificationStatus,
                QrCode = dto.CompanyDetail.QrCode,
                AddedBy = dto.CompanyDetail.AddedBy,
                CreatedAt = dto.CompanyDetail.CreatedAt,
                UpdatedAt = dto.CompanyDetail.UpdatedAt
            } : null,
            Addresses = dto.Addresses.Select(a => AddressResource.FromDto(a)).ToList(),
            Contacts = dto.Contacts.Select(c => ContactResource.FromDto(c)).ToList(),
            CompanyDocuments = dto.CompanyDocuments.Select(doc => new CompanyDocumentRelationResource
            {
                Id = doc.Id,
                DocumentId = doc.DocumentId,
                CompanyId = doc.CompanyId,
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
            }).ToList()
        };
}

public class CompanyDetailRelationResource
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
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
    public int VerificationStatus { get; set; }
    public string? QrCode { get; set; }
    public long? AddedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CompanyDocumentRelationResource
{
    public long Id { get; set; }
    public long DocumentId { get; set; }
    public long CompanyId { get; set; }
    public string? Path { get; set; }
    public string? FileUrl { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int Status { get; set; }
    public long? AddedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DocumentRelationResource? Document { get; set; }
}
