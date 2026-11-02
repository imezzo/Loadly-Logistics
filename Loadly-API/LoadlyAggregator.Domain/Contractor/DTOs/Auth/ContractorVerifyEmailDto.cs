namespace LoadlyAggregator.Domain.Contractor.DTOs.Auth;



public class ContractorVerifyEmailDto
{
    public string Code { get; set; } = string.Empty;
    public long UserId { get; set; }
}
