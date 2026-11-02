namespace LoadlyAggregator.Domain.Company.DTOs.Auth;

public class CompanyValidatePasswordDto
{
    public long UserId { get; set; }

    public string Password { get; set; } = string.Empty;
}


