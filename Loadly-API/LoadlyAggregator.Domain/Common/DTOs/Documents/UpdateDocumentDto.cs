using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.DTOs.Documents;
public class UpdateDocumentDto
{
    public long Id { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public bool Required { get; set; } = false;
    public DocumentStatus Status { get; set; } = DocumentStatus.ACTIVE;
}
