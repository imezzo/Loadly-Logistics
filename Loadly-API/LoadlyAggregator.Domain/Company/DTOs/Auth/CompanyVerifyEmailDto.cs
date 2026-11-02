namespace LoadlyAggregator.Domain.Company.DTOs.Auth;



public class CompanyVerifyEmailDto
{
    public string Code { get; set; } = string.Empty;
    public long UserId { get; set; }
}
