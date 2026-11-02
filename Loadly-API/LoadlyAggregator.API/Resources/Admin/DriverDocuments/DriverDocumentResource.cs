using LoadlyAggregator.API.Resources.RelationResources;
using LoadlyAggregator.Domain.Admin.DTOs.Drivers;
using LoadlyAggregator.Domain.Common.DTOs.Documents;

namespace LoadlyAggregator.API.Resources.Admin.DriverDocuments;

/// <summary>
/// Resource for DriverDocument API responses (both listing and single get)
/// </summary>
public class DriverDocumentResource
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

    public static DriverDocumentResource FromDto(DriverDocumentDto dto, DocumentDto? document = null) =>
        new DriverDocumentResource
        {
            Id = dto.Id,
            DocumentId = dto.DocumentId,
            DriverId = dto.DriverId,
            Path = dto.Path,
            FileUrl = dto.FileUrl,
            ExpiryDate = dto.ExpiryDate,
            Status = (int)dto.Status,
            AddedBy = dto.AddedBy,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            Document = document != null ? new DocumentRelationResource
            {
                Id = document.Id,
                EntityId = document.EntityId,
                NameEn = document.NameEn,
                NameAr = document.NameAr,
                Required = document.Required,
                Status = document.Status
            } : null
        };
}
