namespace LoadlyAggregator.API.Resources.RelationResources;

public class InvoiceRelationResource
{
    public long Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public int InvoiceType { get; set; }
    public string InvoiceTypeName { get; set; } = string.Empty;
    public int Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public LeadRelationResource? Lead { get; set; }
}
