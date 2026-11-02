namespace LoadlyAggregator.Domain.Contractor.DTOs.Auth;

public class ContractorRevokeSessionDto
{
    public long UserId { get; set; }
    public long SessionId { get; set; }
}
