namespace LoadlyAggregator.API.Resources.RelationResources;

public class BankTransferRelationResource
{
    public long Id { get; set; }
    public decimal Amount { get; set; }
    public int PaymentMethod { get; set; }
    public string? ExternalPaymentReferenceNumber { get; set; }
    public long? PayerId { get; set; }
    public string? PayerType { get; set; }
    public CompanyRelationResource? Company { get; set; }
    public LeadRelationResource? Lead { get; set; }
}
