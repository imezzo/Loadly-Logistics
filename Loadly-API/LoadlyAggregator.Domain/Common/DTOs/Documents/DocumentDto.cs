using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.DTOs.Documents;

public class DocumentDto
{
    public long Id { get; set; }
    public DocumentEntityType EntityId { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public bool Required { get; set; }
    public DocumentStatus Status { get; set; }
    public long? AddedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
