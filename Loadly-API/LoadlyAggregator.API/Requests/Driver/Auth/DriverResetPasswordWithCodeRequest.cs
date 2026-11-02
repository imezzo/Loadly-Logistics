using LoadlyAggregator.Domain.Driver.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Driver.Auth;

public class DriverResetPasswordWithCodeRequest
{
    public string Identifier { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;

    public DriverResetPasswordWithCodeDto FromRequest() => new()
    {
        Identifier = string.IsNullOrWhiteSpace(Identifier) ? Login.Trim() : Identifier.Trim(),
        Code = Code,
        NewPassword = NewPassword,
        ConfirmPassword = ConfirmPassword
    };
}
