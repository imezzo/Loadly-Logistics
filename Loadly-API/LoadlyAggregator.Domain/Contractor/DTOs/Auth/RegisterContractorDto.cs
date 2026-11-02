using LoadlyAggregator.Domain.Admin.DTOs.Contractors;

namespace LoadlyAggregator.Domain.Contractor.DTOs.Auth;

public class RegisterContractorDto
{
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? NationalId { get; set; }
    public string? Address { get; set; }
    public List<ContractorDocumentItemDto>? Documents { get; set; }
}
