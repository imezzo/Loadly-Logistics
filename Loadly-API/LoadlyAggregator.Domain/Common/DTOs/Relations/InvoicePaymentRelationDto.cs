namespace LoadlyAggregator.Domain.Common.DTOs.Relations;

public class InvoicePaymentRelationDto
{
    public long Id { get; set; }
    public long InvoiceId { get; set; }
    public long BankTransferId { get; set; }
    public decimal Amount { get; set; }
    public long? AddedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public InvoiceRelationDto? Invoice { get; set; }
    public BankTransferRelationDto? BankTransfer { get; set; }
}
