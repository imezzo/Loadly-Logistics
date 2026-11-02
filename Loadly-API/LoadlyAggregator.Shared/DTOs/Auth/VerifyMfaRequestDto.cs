namespace LoadlyAggregator.Shared.DTOs.Auth;

public class VerifyMfaRequestDto
{
    public string Code { get; set; } = string.Empty;
    public string? TempToken { get; set; }
}
