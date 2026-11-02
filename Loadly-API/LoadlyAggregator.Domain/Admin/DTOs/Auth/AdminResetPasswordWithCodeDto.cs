namespace LoadlyAggregator.Domain.Admin.DTOs.Auth;

public class AdminResetPasswordWithCodeDto
{
    public string Identifier { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}
