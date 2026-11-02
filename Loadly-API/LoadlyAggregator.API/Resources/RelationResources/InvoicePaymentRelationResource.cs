namespace LoadlyAggregator.API.Resources.RelationResources;

public class InvoicePaymentRelationResource
{
    public long Id { get; set; }
    public long InvoiceId { get; set; }
    public long BankTransferId { get; set; }
    public decimal Amount { get; set; }
    public long? AddedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public InvoiceRelationResource? Invoice { get; set; }
    public BankTransferRelationResource? BankTransfer { get; set; }
}
