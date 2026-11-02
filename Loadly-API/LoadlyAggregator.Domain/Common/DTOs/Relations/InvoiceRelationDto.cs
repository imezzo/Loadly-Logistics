using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.DTOs.Relations;

public class InvoiceRelationDto
{
    public long Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public InvoiceType InvoiceType { get; set; }
    public InvoiceStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public LeadRelationDto? Lead { get; set; }
}
