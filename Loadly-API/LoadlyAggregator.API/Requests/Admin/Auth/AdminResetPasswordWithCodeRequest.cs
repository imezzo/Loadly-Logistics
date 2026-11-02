using LoadlyAggregator.Domain.Admin.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Admin.Auth;

public class AdminResetPasswordWithCodeRequest
{
    public string Identifier { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;

    public AdminResetPasswordWithCodeDto FromRequest() => new()
    {
        Identifier = Identifier,
        Code = Code,
        NewPassword = NewPassword,
        ConfirmPassword = ConfirmPassword
    };
}
