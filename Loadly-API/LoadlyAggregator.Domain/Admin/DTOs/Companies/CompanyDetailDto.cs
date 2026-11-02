namespace LoadlyAggregator.Domain.Admin.DTOs.Companies;

public class CompanyDetailDto
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public string? RegistrationNumber { get; set; }
    public string? TaxId { get; set; }
    public string? WebsiteUrl { get; set; }
    public long? IndustryTypeId { get; set; }
    public string? CompanySize { get; set; }
    public string? ServiceRequirementsIds { get; set; }
    public long? BusinessHourId { get; set; }
    public long? CommunicationsMethodId { get; set; }
    public int VerificationStatus { get; set; }
    public string? QrCode { get; set; }
    public long? AddedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
