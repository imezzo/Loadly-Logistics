using LoadlyAggregator.API.Resources.RelationResources;
using LoadlyAggregator.Domain.Admin.DTOs.Contractors;
using ContractorStatusEnum = LoadlyAggregator.Domain.Common.Enums.ContractorStatus;

namespace LoadlyAggregator.API.Resources.Admin.Contractors;

public class ContractorResource
{
    public long Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? NationalId { get; set; }
    public string? SecondPhoneNumber { get; set; }
    public string? Address { get; set; }
    public int MfaChannel { get; set; }
    public bool EmailVerified { get; set; }
    public bool PhoneVerified { get; set; }
    public int Status { get; set; }
    public int ContractorStatus { get; set; }
    public string ContractorStatusName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int TotalDrivers { get; set; }
    public int TotalVehicles { get; set; }
    public int TotalTrips { get; set; }
    public int TotalCompletedTrips { get; set; }
    public int TotalCompletedPaidTrips { get; set; }
    public int TotalCompletedUnderReviewTrips { get; set; }
    public int TotalCompletedReadyToPayTrips { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<ContractorDocumentResource> Documents { get; set; } = new();

    public static ContractorResource FromDto(ContractorDto dto) =>
        new ContractorResource
        {
            Id = dto.Id,
            FullName = dto.FullName,
            UserName = dto.UserName,
            Email = dto.Email,
            Phone = dto.Phone,
            NationalId = dto.NationalId,
            SecondPhoneNumber = dto.SecondPhoneNumber,
            Address = dto.Address,
            MfaChannel = dto.MfaChannel,
            EmailVerified = dto.EmailVerified,
            PhoneVerified = dto.PhoneVerified,
            Status = dto.Status,
            ContractorStatus = (int)dto.ContractorStatus,
            ContractorStatusName = dto.ContractorStatusName,
            IsActive = dto.ContractorStatus == ContractorStatusEnum.ACTIVE,
            TotalDrivers = dto.TotalDrivers,
            TotalVehicles = dto.TotalVehicles,
            TotalTrips = dto.TotalTrips,
            TotalCompletedTrips = dto.TotalCompletedTrips,
            TotalCompletedPaidTrips = dto.TotalCompletedPaidTrips,
            TotalCompletedUnderReviewTrips = dto.TotalCompletedUnderReviewTrips,
            TotalCompletedReadyToPayTrips = dto.TotalCompletedReadyToPayTrips,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            Documents = dto.Documents.Select(doc => new ContractorDocumentResource
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

public class ContractorDocumentResource
{
    public long Id { get; set; }
    public long DocumentId { get; set; }
    public long ContractorId { get; set; }
    public string? Path { get; set; }
    public string? FileUrl { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int Status { get; set; }
    public long? AddedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DocumentRelationResource? Document { get; set; }
}
