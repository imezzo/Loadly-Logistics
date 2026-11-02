using LoadlyAggregator.Domain.Common.DTOs.Documents;

namespace LoadlyAggregator.API.Resources.Admin.Documents;

/// <summary>
/// Resource for Document API responses (both listing and single get)
/// </summary>
public class DocumentResource
{
    public long Id { get; set; }
    public int EntityId { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public bool Required { get; set; }
    public int Status { get; set; }
    public long? AddedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public static DocumentResource FromDto(DocumentDto dto) =>
        new()
        {
            Id = dto.Id,
            EntityId = (int)dto.EntityId,
            NameEn = dto.NameEn,
            NameAr = dto.NameAr,
            Required = dto.Required,
            Status = (int)dto.Status,
            AddedBy = dto.AddedBy,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt
        };
}
