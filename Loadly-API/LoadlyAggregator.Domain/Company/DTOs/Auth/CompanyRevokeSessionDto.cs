namespace LoadlyAggregator.Domain.Company.DTOs.Auth;

public class CompanyRevokeSessionDto
{
    public long UserId { get; set; }
    public long SessionId { get; set; }
}
