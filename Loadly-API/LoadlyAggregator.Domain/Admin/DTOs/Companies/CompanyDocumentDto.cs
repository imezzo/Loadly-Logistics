using LoadlyAggregator.Domain.Common.DTOs.Documents;
using LoadlyAggregator.Domain.Common.Enums;
using CompanyDocumentEntity = LoadlyAggregator.Domain.Common.Entities.CompanyDocument;
namespace LoadlyAggregator.Domain.Admin.DTOs.Companies;

public class CompanyDocumentDto
{
    public long Id { get; set; }
    public long DocumentId { get; set; }
    public long CompanyId { get; set; }
    public string? Path { get; set; }
    public string? FileUrl { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DocumentVerificationStatus Status { get; set; }
    public long? AddedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public DocumentDto? Document { get; set; }
    public CompanyDto? Company { get; set; }
    public static CompanyDocumentDto FromEntity(CompanyDocumentEntity entity)
    {
        return new CompanyDocumentDto
        {
            Id = entity.Id,
            DocumentId = entity.DocumentId,
            CompanyId = entity.CompanyId,
            Path = entity.Path,
            FileUrl = entity.Path,
            ExpiryDate = entity.ExpiryDate,
            Status = entity.Status,
            AddedBy = entity.AddedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,

            Document = null,
            Company = null
        };
    }
}
