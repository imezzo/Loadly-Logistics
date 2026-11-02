using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.DTOs.Documents;
public class CreateDocumentDto
{
    public DocumentEntityType EntityId { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public bool Required { get; set; } = false;
    public DocumentStatus Status { get; set; } = DocumentStatus.ACTIVE;
    public long? AddedBy { get; set; }
}
