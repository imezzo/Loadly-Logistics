namespace LoadlyAggregator.Domain.Contractor.DTOs.Auth;



public class ContractorResetPasswordDto
{
    public string Token { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}
