namespace LoadlyAggregator.Domain.Contractor.DTOs.Auth;

public class ContractorListSessionsDto
{
    public long UserId { get; set; }
    public string? CurrentTokenFamilyId { get; set; }
}
