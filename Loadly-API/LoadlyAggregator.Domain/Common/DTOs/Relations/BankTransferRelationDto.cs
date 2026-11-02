using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.DTOs.Relations;

public class BankTransferRelationDto
{
    public long Id { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? ExternalPaymentReferenceNumber { get; set; }
    public long? PayerId { get; set; }
    public string? PayerType { get; set; }
    public CompanyRelationDto? Company { get; set; }
    public LeadRelationDto? Lead { get; set; }
}
