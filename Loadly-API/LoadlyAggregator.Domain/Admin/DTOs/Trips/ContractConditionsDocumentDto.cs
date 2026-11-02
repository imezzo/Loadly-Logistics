namespace LoadlyAggregator.Domain.Admin.DTOs.Trips;

public class ContractConditionsDocumentDto
{
    public long DocumentId { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public string? FileUrl { get; set; }
}
