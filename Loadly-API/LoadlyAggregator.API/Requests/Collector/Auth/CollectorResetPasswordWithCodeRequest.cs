using LoadlyAggregator.Domain.Collector.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Collector.Auth;

public class CollectorResetPasswordWithCodeRequest
{
    public string Identifier { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;

    public CollectorResetPasswordWithCodeDto FromRequest() => new()
    {
        Identifier = Identifier,
        Code = Code,
        NewPassword = NewPassword,
        ConfirmPassword = ConfirmPassword
    };
}
