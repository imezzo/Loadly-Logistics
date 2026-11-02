namespace LoadlyAggregator.Domain.Contractor.DTOs.Auth;



public class ContractorVerifyPhoneDto
{
    public string Code { get; set; } = string.Empty;
    public long UserId { get; set; }
}
