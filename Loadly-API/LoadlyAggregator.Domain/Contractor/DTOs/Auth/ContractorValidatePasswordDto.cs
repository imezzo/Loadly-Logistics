namespace LoadlyAggregator.Domain.Contractor.DTOs.Auth;

public class ContractorValidatePasswordDto
{
    public long UserId { get; set; }

    public string Password { get; set; } = string.Empty;
}


