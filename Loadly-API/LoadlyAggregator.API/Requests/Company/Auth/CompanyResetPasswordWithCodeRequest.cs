using LoadlyAggregator.Domain.Company.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Company.Auth;

public class CompanyResetPasswordWithCodeRequest
{
    public string Identifier { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;

    public CompanyResetPasswordWithCodeDto FromRequest() => new()
    {
        Identifier = Identifier,
        Code = Code,
        NewPassword = NewPassword,
        ConfirmPassword = ConfirmPassword
    };
}
