using LoadlyAggregator.Domain.Contractor.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Contractor.Auth;

public class ContractorResetPasswordWithCodeRequest
{
    public string Identifier { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;

    public ContractorResetPasswordWithCodeDto FromRequest() => new()
    {
        Identifier = Identifier,
        Code = Code,
        NewPassword = NewPassword,
        ConfirmPassword = ConfirmPassword
    };
}
